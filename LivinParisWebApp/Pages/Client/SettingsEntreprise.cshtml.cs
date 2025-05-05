using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;

namespace LivinParisWebApp.Pages.Client
{
    public class SettingsEntrepriseModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string NomEntre { get; set; }

        [BindProperty]
        public string Siret { get; set; }
        [BindProperty]
        public string NomRef { get; set; }
        [BindProperty]
        public string Arrondissement { get; set; }
        [BindProperty]
        public string Voirie { get; set; }

        [BindProperty]
        public string Numero { get; set; }
        private readonly IConfiguration _config;

        public int NbPlatsCommandes { get; set; }
        public decimal DepensesTotales { get; set; }
        public int NbCommandes { get; set; }
        public decimal PrixMoyenCommande { get; set; }
        [BindProperty]
        public string Tri { get; set; }

        public List<PlatDTO> PlatsCommandes { get; set; } = new();
        public decimal Solde { get; set; }

        public SettingsEntrepriseModel(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return RedirectToPage("/Login");

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            // Récupération de l'email et mdp
            var userCmd = new MySqlCommand("SELECT Mail_Utilisateur, Mdp FROM Utilisateur WHERE Id_Utilisateur = @Uid", conn);
            userCmd.Parameters.AddWithValue("@Uid", userId);

            using var userReader = await userCmd.ExecuteReaderAsync();
            if (await userReader.ReadAsync())
            {
                Email = userReader["Mail_Utilisateur"].ToString();
                Password = userReader["Mdp"].ToString();
            }
            userReader.Close();

            // Récupération de l'entreprise via Client_
            var entrepriseCmd = new MySqlCommand(@"
        SELECT e.Nom_entreprise, e.Num_siret, e.Nom_référent, e.Adresse_entreprise
        FROM Client_ c
        JOIN Entreprise e ON c.Id_Client = e.Id_Client
        WHERE c.Id_Utilisateur = @Uid", conn);
            entrepriseCmd.Parameters.AddWithValue("@Uid", userId);

            using var entrepriseReader = await entrepriseCmd.ExecuteReaderAsync();
            if (await entrepriseReader.ReadAsync())
            {
                NomEntre = entrepriseReader["Nom_entreprise"].ToString();
                Siret = entrepriseReader["Num_siret"].ToString();
                NomRef = entrepriseReader["Nom_référent"].ToString();

                var adresse = entrepriseReader["Adresse_entreprise"]?.ToString()?.Split(',');
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
            entrepriseReader.Close();

            var statsCmd = new MySqlCommand("SELECT NbPlatsCommandes, DepensesTotales, NbCommandes FROM Client_ WHERE Id_Utilisateur = @uid", conn);
            statsCmd.Parameters.AddWithValue("@uid", userId);
            using var statsReader = await statsCmd.ExecuteReaderAsync();

            if (await statsReader.ReadAsync())
            {
                NbPlatsCommandes = Convert.ToInt32(statsReader["NbPlatsCommandes"]);
                DepensesTotales = Convert.ToDecimal(statsReader["DepensesTotales"]);
                NbCommandes = Convert.ToInt32(statsReader["NbCommandes"]);
                PrixMoyenCommande = NbCommandes > 0 ? DepensesTotales / NbCommandes : 0;
            }
            statsReader.Close();

            var soldeCmd = new MySqlCommand("SELECT Solde FROM Client_ WHERE Id_Utilisateur = @uid", conn);
            soldeCmd.Parameters.AddWithValue("@uid", userId);
            Solde = Convert.ToDecimal(await soldeCmd.ExecuteScalarAsync());

            var platsCmd = new MySqlCommand("SELECT Liste_plats_commandes FROM Client_ WHERE Id_Utilisateur = @uid", conn);
            platsCmd.Parameters.AddWithValue("@uid", userId);

            string? listePlats = (await platsCmd.ExecuteScalarAsync())?.ToString();

            if (!string.IsNullOrWhiteSpace(listePlats))
            {
                var ids = listePlats.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var id in ids.Distinct())
                {
                    if (int.TryParse(id, out var platId))
                    {
                        var nomCmd = new MySqlCommand("SELECT Nom_plat FROM Plat WHERE Num_plat = @id", conn);
                        nomCmd.Parameters.AddWithValue("@id", platId);
                        var nom = await nomCmd.ExecuteScalarAsync();
                        if (nom != null)
                        {
                            PlatsCommandes.Add(new PlatDTO
                            {
                                Nom = nom.ToString() ?? ""
                            });
                        }
                    }
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostActionPage()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return RedirectToPage("/Login");

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            // Recharge toutes les infos utilisateur/entreprise/statistiques
            await OnGetAsync(); // appelle la méthode GET pour remplir tout le reste du modèle

            // Et trie les plats comme avant
            PlatsCommandes = await ChargerPlatsCommandesAsync(conn, userId);

            switch (Tri)
            {
                case "nc":
                    PlatsCommandes = PlatsCommandes.OrderBy(p => p.Nom).ToList();
                    break;
                case "nd":
                    PlatsCommandes = PlatsCommandes.OrderByDescending(p => p.Nom).ToList();
                    break;
                case "pc":
                    PlatsCommandes = PlatsCommandes.OrderBy(p => p.Prix).ToList();
                    break;
                case "pd":
                    PlatsCommandes = PlatsCommandes.OrderByDescending(p => p.Prix).ToList();
                    break;
            }

            return Page();
        }


        private async Task<List<PlatDTO>> ChargerPlatsCommandesAsync(MySqlConnection conn, int userId)
        {
            var result = new List<PlatDTO>();

            var platsCmd = new MySqlCommand("SELECT Liste_plats_commandes FROM Client_ WHERE Id_Utilisateur = @uid", conn);
            platsCmd.Parameters.AddWithValue("@uid", userId);

            string? listePlats = (await platsCmd.ExecuteScalarAsync())?.ToString();

            if (!string.IsNullOrWhiteSpace(listePlats))
            {
                var ids = listePlats.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var id in ids.Distinct())
                {
                    if (int.TryParse(id, out var platId))
                    {
                        var cmd = new MySqlCommand("SELECT Nom_plat, Prix_plat FROM Plat WHERE Num_plat = @id", conn);
                        cmd.Parameters.AddWithValue("@id", platId);
                        using var reader = await cmd.ExecuteReaderAsync();
                        if (await reader.ReadAsync())
                        {
                            result.Add(new PlatDTO
                            {
                                Nom = reader["Nom_plat"].ToString() ?? "",
                                Prix = reader.GetDecimal("Prix_plat"),
                                DateCommande = DateTime.Now // ou une vraie date si dispo
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

        public IActionResult OnPostClientPanel()
        {
            return RedirectToPage("/ClientPanel");
        }

        public async Task<IActionResult> OnPostValidate()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return RedirectToPage("/Login");

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            // Met à jour Utilisateur
            var updateUser = new MySqlCommand(@"
        UPDATE Utilisateur 
        SET Mail_Utilisateur = COALESCE(NULLIF(@mail, ''), Mail_Utilisateur),
            Mdp = COALESCE(NULLIF(@mdp, ''), Mdp)
        WHERE Id_Utilisateur = @Uid", conn);
            updateUser.Parameters.AddWithValue("@mail", Email ?? "");
            updateUser.Parameters.AddWithValue("@mdp", Password ?? "");
            updateUser.Parameters.AddWithValue("@Uid", userId);
            await updateUser.ExecuteNonQueryAsync();

            // Recompose l'adresse complète
            string adresseComplete = $"{Numero} {Voirie}, {Arrondissement}";

            // Met à jour Entreprise
            var updateEntreprise = new MySqlCommand(@"
        UPDATE Entreprise 
        SET Nom_entreprise = COALESCE(NULLIF(@nomentre, ''), Nom_entreprise),
            Num_siret = COALESCE(NULLIF(@siret, ''), Num_siret),
            Nom_référent = COALESCE(NULLIF(@nomref, ''), Nom_référent),
            Adresse_entreprise = COALESCE(NULLIF(@adresse, ''), Adresse_entreprise)
        WHERE Id_Client = (SELECT Id_Client FROM Client_ WHERE Id_Utilisateur = @Uid)", conn);
            updateEntreprise.Parameters.AddWithValue("@nomentre", NomEntre ?? "");
            updateEntreprise.Parameters.AddWithValue("@siret", Siret ?? "");
            updateEntreprise.Parameters.AddWithValue("@nomref", NomRef ?? "");
            updateEntreprise.Parameters.AddWithValue("@adresse", adresseComplete);
            updateEntreprise.Parameters.AddWithValue("@Uid", userId);
            await updateEntreprise.ExecuteNonQueryAsync();

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
    public class PlatDTO
    {
        public string Nom { get; set; }
        public decimal Prix { get; set; } // à remplir si nécessaire
        public DateTime DateCommande { get; set; } // idem
    }

}