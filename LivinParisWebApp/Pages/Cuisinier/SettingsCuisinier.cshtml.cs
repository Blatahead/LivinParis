using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class SettingsCuisinierModel : PageModel
    {
        #region Attribut
        private readonly IConfiguration _config;
        #endregion

        #region Constrcuteur
        public SettingsCuisinierModel(IConfiguration config)
        {
            _config = config;
        }
        #endregion

        #region Proprietes
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Password { get; set; }
        [BindProperty] public string Prenom { get; set; }
        [BindProperty] public string Nom { get; set; }
        [BindProperty] public string Arrondissement { get; set; }
        [BindProperty] public string Voirie { get; set; }
        [BindProperty] public string Numero { get; set; }

        public int NbPlatsVendus { get; set; }
        public decimal RevenusTotaux { get; set; }
        public List<string> ClientsServis { get; set; } = new();
        public string TempsLivraison { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Tri { get; set; }
        #endregion

        #region Methodes
        /// <summary>
        /// au lancement de la page
        /// </summary>
        /// <returns></returns>
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

            var cuisCmd = new MySqlCommand(@"
                SELECT Prenom_cuisinier, Nom_particulier, Adresse_cuisinier, 
                       Liste_commandes_livrees, Revenus_totaux, Clients_servis
                FROM Cuisinier 
                WHERE Id_Utilisateur = @Uid", conn);

            cuisCmd.Parameters.AddWithValue("@Uid", userId);
            string? livrees = null;

            using var cuisReader = await cuisCmd.ExecuteReaderAsync();
            if (await cuisReader.ReadAsync())
            {
                Prenom = cuisReader["Prenom_cuisinier"]?.ToString();
                Nom = cuisReader["Nom_particulier"]?.ToString();

                RevenusTotaux = cuisReader["Revenus_totaux"] != DBNull.Value ? Convert.ToDecimal(cuisReader["Revenus_totaux"]): 0;

                var clientsServisRaw = cuisReader["Clients_servis"]?.ToString();
                ClientsServis = !string.IsNullOrEmpty(clientsServisRaw)
                    ? clientsServisRaw.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList()
                    : new List<string>();

                var adresse = cuisReader["Adresse_cuisinier"]?.ToString()?.Split(',');
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

                livrees = cuisReader["Liste_commandes_livrees"]?.ToString();
            }
            cuisReader.Close();

            if (!string.IsNullOrEmpty(livrees))
            {
                var commandes = livrees.Split(',').Select(commande => commande.Trim()).ToList();
                NbPlatsVendus = commandes.Count;

                TimeSpan totalLivraison = TimeSpan.Zero;

                foreach (var id in commandes)
                {
                    var cmdDetails = new MySqlCommand(@"
                    SELECT 
                        c.Prix_commande,
                        p.Prenom_particulier,
                        p.Nom_particulier,
                        e.Nom_référent,
                        e.Nom_entreprise
                    FROM Commande c
                    LEFT JOIN Client_ cl ON c.id_Utilisateur = cl.Id_Utilisateur
                    LEFT JOIN Particulier p ON cl.Id_Client = p.Id_Client
                    LEFT JOIN Entreprise e ON cl.Id_Client = e.Id_Client
                    WHERE c.Num_commande = @id", conn);

                    cmdDetails.Parameters.AddWithValue("@id", id);

                    using var reader = await cmdDetails.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        if (decimal.TryParse(reader["Prix_commande"].ToString(), out decimal prix))
                        {
                            RevenusTotaux += prix;
                        }

                        string nomPrenom = null;

                        if (reader["Prenom_particulier"] != DBNull.Value && reader["Nom_particulier"] != DBNull.Value)
                        {
                            nomPrenom = $"{reader["Prenom_particulier"]} {reader["Nom_particulier"]}";
                        }
                        else if (reader["Nom_référent"] != DBNull.Value && reader["Nom_entreprise"] != DBNull.Value)
                        {
                            nomPrenom = $"{reader["Nom_référent"]} ({reader["Nom_entreprise"]})";
                        }

                        if (!string.IsNullOrEmpty(nomPrenom) && !ClientsServis.Contains(nomPrenom))
                            ClientsServis.Add(nomPrenom);
                    }
                    reader.Close();

                    totalLivraison += TimeSpan.FromMinutes(15);
                }
                TempsLivraison = $"{(int)totalLivraison.TotalHours} h {totalLivraison.Minutes} min";
            }

            return Page();
        }

        /// <summary>
        /// valider les changements dans les champs des donnees
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostValidate()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return RedirectToPage("/Login");

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            if (!string.IsNullOrEmpty(Email) || !string.IsNullOrEmpty(Password))
            {
                var updateUser = new MySqlCommand("UPDATE Utilisateur SET Mail_Utilisateur = COALESCE(NULLIF(@mail, ''), Mail_Utilisateur), Mdp = COALESCE(NULLIF(@mdp, ''), Mdp) WHERE Id_Utilisateur = @Uid", conn);
                updateUser.Parameters.AddWithValue("@mail", Email ?? "");
                updateUser.Parameters.AddWithValue("@mdp", Password ?? "");
                updateUser.Parameters.AddWithValue("@Uid", userId);
                await updateUser.ExecuteNonQueryAsync();
            }

            if (!string.IsNullOrEmpty(Prenom) || !string.IsNullOrEmpty(Nom) || !string.IsNullOrEmpty(Arrondissement) || !string.IsNullOrEmpty(Voirie) || !string.IsNullOrEmpty(Numero))
            {
                string adresseComplete = $"{Numero} {Voirie}, 750{Arrondissement} Paris";

                var updateCuis = new MySqlCommand(@"
                    UPDATE Cuisinier 
                    SET Prenom_cuisinier = COALESCE(NULLIF(@prenom, ''), Prenom_cuisinier),
                        Nom_particulier = COALESCE(NULLIF(@nom, ''), Nom_particulier),
                        Adresse_cuisinier = COALESCE(NULLIF(@adresse, ''), Adresse_cuisinier)
                    WHERE Id_Utilisateur = @Uid", conn);

                updateCuis.Parameters.AddWithValue("@prenom", Prenom ?? "");
                updateCuis.Parameters.AddWithValue("@nom", Nom ?? "");
                updateCuis.Parameters.AddWithValue("@adresse", adresseComplete);
                updateCuis.Parameters.AddWithValue("@Uid", userId);

                await updateCuis.ExecuteNonQueryAsync();
            }

            TempData["Message"] = "Modifications enregistrées !";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostActionPage()
        {
            await OnGetAsync();

            switch (Tri)
            {
                case "nc":
                    ClientsServis = ClientsServis.OrderBy(n => n).ToList();
                    break;
                case "nd":
                    ClientsServis = ClientsServis.OrderByDescending(n => n).ToList();
                    break;
            }
            return Page();
        }

        /// <summary>
        /// bouton de retour
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostCuisinierPanel()
        {
            return RedirectToPage("/CuisinierPanel");
        }

        /// <summary>
        /// deconnexion du cuisinier
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostDeconnexion()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        /// <summary>
        /// suppression du cuisinier
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostSupprimer()
        {
            return RedirectToPage("/Cuisinier/SupprimerCuisinier");
        }
        #endregion
    }
}