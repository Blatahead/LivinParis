using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;

namespace LivinParisWebApp.Pages.Client
{
    public class SettingsParticulierModel : PageModel
    {
        private readonly IConfiguration _config;

        public SettingsParticulierModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Password { get; set; }
        [BindProperty] public string Prenom { get; set; }
        [BindProperty] public string Nom { get; set; }
        [BindProperty] public string Arrondissement { get; set; }
        [BindProperty] public string Voirie { get; set; }
        [BindProperty] public string Numero { get; set; }
        [BindProperty] public string Tri { get; set; }

        public int NbPlatsCommandes { get; set; }
        public decimal DepensesTotales { get; set; }
        public int NbCommandes { get; set; }
        public decimal PrixMoyenCommande { get; set; }
        public decimal Solde { get; set; }

        public List<PlatDTO> PlatsCommandes { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return RedirectToPage("/Login");

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            var userCmd = new MySqlCommand("SELECT Mail_Utilisateur, Mdp FROM Utilisateur WHERE Id_Utilisateur = @Uid", conn);
            userCmd.Parameters.AddWithValue("@Uid", userId);
            using var userReader = await userCmd.ExecuteReaderAsync();
            if (await userReader.ReadAsync())
            {
                Email = userReader["Mail_Utilisateur"].ToString();
                Password = userReader["Mdp"].ToString();
            }
            userReader.Close();

            var partCmd = new MySqlCommand("SELECT Prenom_particulier, Nom_particulier, Adresse_particulier FROM Particulier WHERE Id_Client = (SELECT Id_Client FROM Client_ WHERE Id_Utilisateur = @Uid)", conn);
            partCmd.Parameters.AddWithValue("@Uid", userId);
            using var partReader = await partCmd.ExecuteReaderAsync();
            if (await partReader.ReadAsync())
            {
                Prenom = partReader["Prenom_particulier"]?.ToString();
                Nom = partReader["Nom_particulier"]?.ToString();

                var adresse = partReader["Adresse_particulier"]?.ToString()?.Split(',');
                if (adresse != null && adresse.Length == 2)
                {
                    var numeroEtVoirie = adresse[0].Trim().Split(' ', 2);
                    if (numeroEtVoirie.Length == 2)
                    {
                        Numero = numeroEtVoirie[0];
                        Voirie = numeroEtVoirie[1];
                    }
                    else
                    {
                        Numero = "";
                        Voirie = adresse[0].Trim();
                    }
                    Arrondissement = adresse[1].Trim();
                }
            }
            partReader.Close();

            var statsCmd = new MySqlCommand("SELECT NbPlatsCommandes, DepensesTotales, NbCommandes, Solde FROM Client_ WHERE Id_Utilisateur = @uid", conn);
            statsCmd.Parameters.AddWithValue("@uid", userId);
            using var statsReader = await statsCmd.ExecuteReaderAsync();
            if (await statsReader.ReadAsync())
            {
                NbPlatsCommandes = Convert.ToInt32(statsReader["NbPlatsCommandes"]);
                DepensesTotales = Convert.ToDecimal(statsReader["DepensesTotales"]);
                NbCommandes = Convert.ToInt32(statsReader["NbCommandes"]);
                Solde = Convert.ToDecimal(statsReader["Solde"]);
                PrixMoyenCommande = NbCommandes > 0 ? DepensesTotales / NbCommandes : 0;
            }
            statsReader.Close();

            PlatsCommandes = await ChargerPlatsCommandesAsync(conn, userId);
            return Page();
        }

        public async Task<IActionResult> OnPostActionPage()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return RedirectToPage("/Login");

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            await OnGetAsync();
            PlatsCommandes = await ChargerPlatsCommandesAsync(conn, userId);

            switch (Tri)
            {
                case "nc": PlatsCommandes = PlatsCommandes.OrderBy(p => p.Nom).ToList(); break;
                case "nd": PlatsCommandes = PlatsCommandes.OrderByDescending(p => p.Nom).ToList(); break;
                case "pc": PlatsCommandes = PlatsCommandes.OrderBy(p => p.Prix).ToList(); break;
                case "pd": PlatsCommandes = PlatsCommandes.OrderByDescending(p => p.Prix).ToList(); break;
                case "pm": PlatsCommandes = PlatsCommandes.OrderByDescending(p => p.DateCommande).ToList(); break;
                case "mp": PlatsCommandes = PlatsCommandes.OrderBy(p => p.DateCommande).ToList(); break;
            }

            return Page();
        }

        private async Task<List<PlatDTO>> ChargerPlatsCommandesAsync(MySqlConnection conn, int userId)
        {
            var result = new List<PlatDTO>();
            var cmd = new MySqlCommand("SELECT Liste_plats_commandes FROM Client_ WHERE Id_Utilisateur = @uid", conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            var listePlats = (await cmd.ExecuteScalarAsync())?.ToString();
            if (!string.IsNullOrWhiteSpace(listePlats))
            {
                var ids = listePlats.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var id in ids.Distinct())
                {
                    if (int.TryParse(id, out var platId))
                    {
                        var detailCmd = new MySqlCommand("SELECT Nom_plat, Prix_plat FROM Plat WHERE Num_plat = @id", conn);
                        detailCmd.Parameters.AddWithValue("@id", platId);
                        using var reader = await detailCmd.ExecuteReaderAsync();
                        if (await reader.ReadAsync())
                        {
                            result.Add(new PlatDTO
                            {
                                Nom = reader["Nom_plat"].ToString() ?? "",
                                Prix = reader.GetDecimal("Prix_plat"),
                                DateCommande = DateTime.Now
                            });
                        }
                        reader.Close();
                    }
                }
            }
            return result;
        }

        public async Task<IActionResult> OnPostAddArgent()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return RedirectToPage("/Login");

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            var cmd = new MySqlCommand("UPDATE Client_ SET Solde = Solde + 10 WHERE Id_Utilisateur = @uid", conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            await cmd.ExecuteNonQueryAsync();

            return RedirectToPage();
        }

        public IActionResult OnPostClientPanel() => RedirectToPage("/ClientPanel");

        public async Task<IActionResult> OnPostValidate()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return RedirectToPage("/Login");

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            var updateUser = new MySqlCommand(@"
                UPDATE Utilisateur 
                SET Mail_Utilisateur = COALESCE(NULLIF(@mail, ''), Mail_Utilisateur),
                    Mdp = COALESCE(NULLIF(@mdp, ''), Mdp)
                WHERE Id_Utilisateur = @Uid", conn);
            updateUser.Parameters.AddWithValue("@mail", Email ?? "");
            updateUser.Parameters.AddWithValue("@mdp", Password ?? "");
            updateUser.Parameters.AddWithValue("@Uid", userId);
            await updateUser.ExecuteNonQueryAsync();

            string adresseComplete = $"{Numero} {Voirie}, {Arrondissement}";

            var updateParticulier = new MySqlCommand(@"
                UPDATE Particulier 
                SET Prenom_particulier = COALESCE(NULLIF(@prenom, ''), Prenom_particulier),
                    Nom_particulier = COALESCE(NULLIF(@nom, ''), Nom_particulier),
                    Adresse_particulier = COALESCE(NULLIF(@adresse, ''), Adresse_particulier)
                WHERE Id_Client = (SELECT Id_Client FROM Client_ WHERE Id_Utilisateur = @Uid)", conn);
            updateParticulier.Parameters.AddWithValue("@prenom", Prenom ?? "");
            updateParticulier.Parameters.AddWithValue("@nom", Nom ?? "");
            updateParticulier.Parameters.AddWithValue("@adresse", adresseComplete);
            updateParticulier.Parameters.AddWithValue("@Uid", userId);
            await updateParticulier.ExecuteNonQueryAsync();

            TempData["Message"] = "Modifications enregistrées !";
            return RedirectToPage();
        }

        public IActionResult OnPostDeconnexion()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        public async Task<IActionResult> OnPostSupprimer()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return RedirectToPage("/Login");

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            var deleteCmd = new MySqlCommand("DELETE FROM Utilisateur WHERE Id_Utilisateur = @id", conn);
            deleteCmd.Parameters.AddWithValue("@id", userId);
            await deleteCmd.ExecuteNonQueryAsync();

            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }

    //public class PlatDTO
    //{
    //    public string Nom { get; set; }
    //    public decimal Prix { get; set; }
    //    public DateTime DateCommande { get; set; }
    //}
}