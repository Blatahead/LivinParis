using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace LivinParis.Pages
{
    public class CreateEntrepriseModel : PageModel
    {
        private readonly IConfiguration _config;

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }

        [BindProperty] public string NomEntreprise { get; set; }
        [BindProperty] public string NumeroSiret { get; set; }
        [BindProperty] public string NomReferent { get; set; }
        [BindProperty] public string Arrondissement { get; set; }
        [BindProperty] public string Voirie { get; set; }
        [BindProperty] public string Numéro { get; set; }

        public CreateEntrepriseModel(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void OnGet()
        {
            TempData.Keep("Email");
            TempData.Keep("Password");
        }

        public async Task<IActionResult> OnPostCestParti()
        {
            string email = TempData["Email"] as string;
            string password = TempData["Password"] as string;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Informations d'inscription manquantes.");
                return Page();
            }

            string connStr = _config.GetConnectionString("MyDb");

            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                //Utilisateur
                var insertUserCmd = new MySqlCommand(@"
                    INSERT INTO Utilisateur (Mail_Utilisateur, Mdp)
                    VALUES (@Email, @Pwd);
                    SELECT LAST_INSERT_ID();", conn, transaction);

                insertUserCmd.Parameters.AddWithValue("@Email", email);
                insertUserCmd.Parameters.AddWithValue("@Pwd", password);

                int userId = Convert.ToInt32(await insertUserCmd.ExecuteScalarAsync());

                //Client_
                var insertClientCmd = new MySqlCommand("INSERT INTO Client_ (Id_Utilisateur) VALUES (@UserId)", conn, transaction);
                insertClientCmd.Parameters.AddWithValue("@UserId", userId);
                await insertClientCmd.ExecuteNonQueryAsync();

                //Récupérer Id_Client
                var getLastIdCmd = new MySqlCommand("SELECT LAST_INSERT_ID()", conn, transaction);
                int clientId = Convert.ToInt32(await getLastIdCmd.ExecuteScalarAsync());

                //Insertion dans Entreprise
                string adresse = $"{Voirie} {Numéro}, 750{Arrondissement} Paris";
                var insertEntrepriseCmd = new MySqlCommand(@"
                    INSERT INTO Entreprise (Id_Client, Nom_entreprise, Nom_référent, Adresse_entreprise, Num_SIRET)
                    VALUES (@ClientId, @Nom, @Referent, @Adresse, @Siret)", conn, transaction);

                insertEntrepriseCmd.Parameters.AddWithValue("@ClientId", clientId);
                insertEntrepriseCmd.Parameters.AddWithValue("@Nom", NomEntreprise);
                insertEntrepriseCmd.Parameters.AddWithValue("@Referent", NomReferent);
                insertEntrepriseCmd.Parameters.AddWithValue("@Adresse", adresse);
                insertEntrepriseCmd.Parameters.AddWithValue("@Siret", NumeroSiret);

                await insertEntrepriseCmd.ExecuteNonQueryAsync();
                await transaction.CommitAsync();

                // Enregistrer l’utilisateur en session
                HttpContext.Session.SetInt32("UserId", userId);
                return RedirectToPage("/ClientPanel");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Erreur : " + ex.Message);
                return Page();
            }
        }

        public IActionResult OnPostChoixPe()
        {
            return RedirectToPage("/ChoixPe");
        }
    }
}