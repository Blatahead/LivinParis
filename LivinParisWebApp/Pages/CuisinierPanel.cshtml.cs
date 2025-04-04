using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace LivinParisWebApp.Pages
{
    public class CuisinierPanelModel : PageModel
    {
        private readonly IConfiguration _config;
        public CuisinierPanelModel(IConfiguration config)
        {
            _config = config;
        }

        public string ProchaineLivraison { get; set; }
        public int NbCommandesEnCours { get; set; }
        public string MoyenneNotation { get; set; } = "En cours de développement";

        public PlatDuJourDto PlatDuJour { get; set; }
        public List<PlatDispoDto> PlatsDisponibles { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return RedirectToPage("/Login");

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            // 1. Récupérer l'ID cuisinier
            int cuisinierId = 0;
            var getIdCmd = new MySqlCommand("SELECT Id_Cuisinier, Liste_de_plats, Liste_commandes, Liste_commandes_pretes FROM Cuisinier WHERE Id_Utilisateur = @UserId", conn);
            getIdCmd.Parameters.AddWithValue("@UserId", userId);
            using var reader = await getIdCmd.ExecuteReaderAsync();
            string? commandes = null, pretes = null, plats = null;

            if (await reader.ReadAsync())
            {
                if (!reader.IsDBNull(reader.GetOrdinal("Id_Cuisinier")))
                {
                    cuisinierId = Convert.ToInt32(reader["Id_Cuisinier"]);
                }
                else
                {
                    // Gérer l'erreur ou mettre un fallback
                    ModelState.AddModelError("", "ID cuisinier introuvable.");
                    return Page();
                }

                plats = reader["Liste_de_plats"] as string;
                commandes = reader["Liste_commandes"] as string;
                pretes = reader["Liste_commandes_pretes"] as string;
            }
            reader.Close();

            // 2. Prochaine livraison (commande prête ?)
            ProchaineLivraison = !string.IsNullOrEmpty(pretes) ? DateTime.Now.AddMinutes(30).ToString("dd/MM/yy à HH:mm") : "Aucune";

            // 3. Nb de commandes
            NbCommandesEnCours = string.IsNullOrEmpty(commandes) ? 0 : commandes.Split(',').Length;

            // 4. Plat du jour
            var platCmd = new MySqlCommand(@"
    SELECT Nom_platJ, prix_platJ, Nombre_de_personneJ, 
           Nationalité_platJ, Régime_alimentaire_platJ
    FROM Plat_du_jour
    WHERE id_Cuisinier = @Cid
    ORDER BY Date_fabrication_platJ DESC
    LIMIT 1", conn);
            platCmd.Parameters.AddWithValue("@Cid", cuisinierId);

            using var platReader = await platCmd.ExecuteReaderAsync();
            if (await platReader.ReadAsync())
            {
                PlatDuJour = new PlatDuJourDto
                {
                    Nom = platReader["Nom_platJ"]?.ToString(),
                    Prix = platReader["prix_platJ"]?.ToString(),
                    NbPersonnes = platReader["Nombre_de_personneJ"] is DBNull
                        ? 0
                        : Convert.ToInt32(platReader["Nombre_de_personneJ"]),
                    Nationalite = platReader["Nationalité_platJ"]?.ToString(),
                    Regime = platReader["Régime_alimentaire_platJ"]?.ToString()
                };
            }
            platReader.Close();



            // 5. Liste des plats disponibles (mocké avec nom et cuisinier seulement)
            if (!string.IsNullOrEmpty(plats))
            {
                var noms = plats.Split(',');
                PlatsDisponibles = noms.Select(n => new PlatDispoDto
                {
                    Nom = n.Trim(),
                    Cuisinier = "Moi"
                }).ToList();
            }

            return Page();
        }

        public IActionResult OnPostSettingsCuisinier()
        {
            return RedirectToPage("/Cuisinier/SettingsCuisinier");
        }


        public IActionResult OnPostChangeTodaysPlat()
        {
            return RedirectToPage("/Cuisinier/ChangeTodaysPlat");
        }
        public IActionResult OnPostChangeDetailsPlat()
        {
            return RedirectToPage("/Cuisinier/DetailsPlat");
        }
        public IActionResult OnPostDeletePlat(string nomPlat)
        {
            return RedirectToPage("/Cuisinier/DeletePlat", new { nomPlat = nomPlat });
        }

        public IActionResult OnPostAddPlat()
        {
            return RedirectToPage("/Cuisinier/AddPlat");
        }

        public IActionResult OnPostSeeCurrentCommand()
        {
            return RedirectToPage("/Cuisinier/SeeCurrentCommand");
        }

        public class PlatDuJourDto
        {
            public string Nom { get; set; }
            public string Prix { get; set; }
            public int NbPersonnes { get; set; }
            public string Nationalite { get; set; }
            public string Regime { get; set; }
        }


        public class PlatDispoDto
        {
            public string? Nom { get; set; }
            public string? Cuisinier { get; set; }
        }
    }
}