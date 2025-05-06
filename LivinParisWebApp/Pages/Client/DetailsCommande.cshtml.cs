using LivinParisWebApp.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace LivinParisWebApp.Pages.Client
{
    public class DetailsCommandeModel : PageModel
    {
        #region Attributs
        private readonly IConfiguration _config;
        private const string SessionKey = "LignesCommandeTemp";
        #endregion

        #region Constructeur
        public DetailsCommandeModel(IConfiguration config)
        {
            _config = config;
        }
        #endregion

        #region Proprietes
        public List<PlatDisponibleDTO> PlatsDisponibles { get; set; } = new();

        [BindProperty]
        public List<LigneCommandeTemp> Lignes { get; set; } = new();
        [BindProperty]
        public int NombreDeLignesSouhaitees { get; set; }
        [BindProperty(SupportsGet = true)]
        public int ligneIndex { get; set; }
        #endregion

        #region Methodes
        /// <summary>
        /// Lancement de la page details commande cote client
        /// </summary>
        /// <returns></returns>
        public async Task OnGetAsync()
        {
            var data = HttpContext.Session.GetString(SessionKey);
            if (data != null)
                Lignes = JsonConvert.DeserializeObject<List<LigneCommandeTemp>>(data) ?? new();

            await ChargerPlatsDisponiblesAsync();
        }

        /// <summary>
        /// load les plats dispo
        /// </summary>
        /// <returns></returns>
        private async Task ChargerPlatsDisponiblesAsync()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return;

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            var getClientIdCmd = new MySqlCommand("SELECT Id_Client FROM Client_ WHERE Id_Utilisateur = @userId", conn);
            getClientIdCmd.Parameters.AddWithValue("@userId", userId);
            var idClient = Convert.ToInt32(await getClientIdCmd.ExecuteScalarAsync());

            var cmd = new MySqlCommand(@"SELECT p.Num_plat, p.Nom_plat 
        FROM Panier pa 
        JOIN Plat p ON pa.Num_plat = p.Num_plat 
        WHERE pa.Id_Client = @idClient", conn);

            cmd.Parameters.AddWithValue("@idClient", idClient);

            var plats = new List<int>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                int id = Convert.ToInt32(reader["Num_plat"]);
                plats.Add(id);
                PlatsDisponibles.Add(new PlatDisponibleDTO
                {
                    Id = id,
                    Nom = reader["Nom_plat"]?.ToString() ?? ""
                });
            }
            HttpContext.Session.SetObject("PanierClient", plats);
        }

        /// <summary>
        /// Au clic sur le bouton livrer la commande
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostLivrerCommande()
        {
            var panier = HttpContext.Session.GetObjectFromJson<List<int>>("PanierClient") ?? new();

            var tousPlatsCoches = Lignes.SelectMany(l => l.Plats).ToList();

            var tousInclus = panier.All(p => tousPlatsCoches.Contains(p));

            var doublons = tousPlatsCoches
                .GroupBy(p => p)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (!tousInclus)
            {
                TempData["Erreur"] = "Tous les plats du panier doivent être répartis dans les lignes de commande.";
                return Page();
            }

            if (doublons.Any())
            {
                TempData["Erreur"] = "Un même plat ne peut être sélectionné que dans une seule ligne.";
                return Page();
            }

            HttpContext.Session.SetObject("LignesCommandeTemp", Lignes);
            return RedirectToPage("/Client/LivraisonClient");
        }

        /// <summary>
        /// load le panier client
        /// </summary>
        /// <returns></returns>
        private async Task ChargerPanierClientDansSession()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return;

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            var getClientIdCmd = new MySqlCommand("SELECT Id_Client FROM Client_ WHERE Id_Utilisateur = @userId", conn);
            getClientIdCmd.Parameters.AddWithValue("@userId", userId);
            var idClient = Convert.ToInt32(await getClientIdCmd.ExecuteScalarAsync());

            var cmd = new MySqlCommand("SELECT Num_plat FROM Panier WHERE Id_Client = @idClient", conn);
            cmd.Parameters.AddWithValue("@idClient", idClient);

            var ids = new List<int>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ids.Add(reader.GetInt32(0));
            }

            HttpContext.Session.SetObject("PanierClient", ids);
        }
        
        /// <summary>
        /// clic sur bouton supprimer du panier
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostSupprimerLigne()
        {
            var lignes = Lignes ?? new List<LigneCommandeTemp>();

            if (ligneIndex >= 0 && ligneIndex < lignes.Count)
            {
                lignes.RemoveAt(ligneIndex);
            }

            HttpContext.Session.SetString(SessionKey, JsonConvert.SerializeObject(lignes));
            Lignes = lignes;
            return RedirectToPage();
        }

        /// <summary>
        /// utiliser l'addresse du client dans l'input
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostUtiliserAdresse()
        {
            var lignes = Lignes ?? new List<LigneCommandeTemp>();

            if (ligneIndex >= 0 && ligneIndex < lignes.Count)
            {
                string adresse = await GetAdresseUtilisateur();
                lignes[ligneIndex].LieuLivraison = adresse;
            }

            HttpContext.Session.SetString(SessionKey, JsonConvert.SerializeObject(lignes));
            Lignes = lignes;
            await ChargerPlatsDisponiblesAsync();
            return Page();
        }

        /// <summary>
        /// récupération de l'adresse du user associé
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetAdresseUtilisateur()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return "";

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            var getClientCmd = new MySqlCommand("SELECT Id_Client FROM Client_ WHERE Id_Utilisateur = @uid", conn);
            getClientCmd.Parameters.AddWithValue("@uid", userId);
            var idClientObj = await getClientCmd.ExecuteScalarAsync();

            if (idClientObj == null) return "";
            int idClient = Convert.ToInt32(idClientObj);

            var partCmd = new MySqlCommand("SELECT Adresse_particulier FROM Particulier WHERE Id_Client = @id", conn);
            partCmd.Parameters.AddWithValue("@id", idClient);
            var partAdresse = await partCmd.ExecuteScalarAsync();
            if (partAdresse != null) return partAdresse.ToString();

            var entCmd = new MySqlCommand("SELECT Adresse_entreprise FROM Entreprise WHERE Id_Client = @id", conn);
            entCmd.Parameters.AddWithValue("@id", idClient);
            var entAdresse = await entCmd.ExecuteScalarAsync();
            if (entAdresse != null) return entAdresse.ToString();

            return "";
        }

        /// <summary>
        /// au clic pour choisir le nb de lignes de commandes
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostChoisirNombreLignesAsync()
        {
            var lignes = new List<LigneCommandeTemp>();

            for (int i = 0; i < NombreDeLignesSouhaitees; i++)
            {
                lignes.Add(new LigneCommandeTemp());
            }

            if (Lignes != null && Lignes.Count > 0)
            {
                for (int i = 0; i < Math.Min(lignes.Count, Lignes.Count); i++)
                {
                    lignes[i].DateLivraison = Lignes[i].DateLivraison;
                    lignes[i].LieuLivraison = Lignes[i].LieuLivraison;
                    lignes[i].Plats = Lignes[i].Plats ?? new();
                }
            }

            HttpContext.Session.SetString(SessionKey, JsonConvert.SerializeObject(lignes));
            await ChargerPlatsDisponiblesAsync();
            Lignes = lignes;

            return Page();
        }

        /// <summary>
        /// au clic sur le bouton retour
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostRetour()
        {
            return RedirectToPage("/ClientPanel");
        }
    }
    #endregion

    public class LigneCommandeTemp
    {
        public DateTime DateLivraison { get; set; }
        public string LieuLivraison { get; set; }
        public List<int> Plats { get; set; } = new();
    }
}