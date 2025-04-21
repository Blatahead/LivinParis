using LivinParisWebApp.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace LivinParisWebApp.Pages.Client
{
    public class DetailsCommandeModel : PageModel
    {
        private readonly IConfiguration _config;
        public DetailsCommandeModel(IConfiguration config)
        {
            _config = config;
        }

        private const string SessionKey = "LignesCommandeTemp";
        public List<PlatDisponibleDTO> PlatsDisponibles { get; set; } = new();

        [BindProperty]
        public List<LigneCommandeTemp> Lignes { get; set; } = new();
        [BindProperty]
        public int NombreDeLignesSouhaitees { get; set; }

        public async Task OnGetAsync()
        {
            var data = HttpContext.Session.GetString(SessionKey);
            if (data != null)
                Lignes = JsonConvert.DeserializeObject<List<LigneCommandeTemp>>(data) ?? new();

            await ChargerPlatsDisponiblesAsync();
        }

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

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                PlatsDisponibles.Add(new PlatDisponibleDTO
                {
                    Id = Convert.ToInt32(reader["Num_plat"]),
                    Nom = reader["Nom_plat"]?.ToString() ?? ""
                });
            }
        }

        public IActionResult OnPostLivrerCommande()
        {
            var panier = HttpContext.Session.GetObjectFromJson<List<int>>("PanierClient") ?? new();

            // Liste de tous les plats cochés dans toutes les lignes
            var tousPlatsCoches = Lignes.SelectMany(l => l.Plats).ToList();

            // Vérif : chaque plat du panier est bien utilisé au moins une fois
            var tousInclus = panier.All(p => tousPlatsCoches.Contains(p));

            // Vérif : aucun plat n'est coché plusieurs fois
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

            // OK sauvegarde
            HttpContext.Session.SetObject("LignesCommandeTemp", Lignes);
            return RedirectToPage("/Client/LivraisonClient");
        }


        [BindProperty(SupportsGet = true)]
        public int ligneIndex { get; set; }

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

        public async Task<IActionResult> OnPostChoisirNombreLignesAsync()
        {
            var lignes = new List<LigneCommandeTemp>();

            for (int i = 0; i < NombreDeLignesSouhaitees; i++)
            {
                lignes.Add(new LigneCommandeTemp());
            }

            // Si des données postées existent, les réinjecter
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

        public IActionResult OnPostRetour()
        {
            return RedirectToPage("/ClientPanel");
        }
    }

    public class LigneCommandeTemp
    {
        public DateTime DateLivraison { get; set; }
        public string LieuLivraison { get; set; }
        public List<int> Plats { get; set; } = new();
    }
}
