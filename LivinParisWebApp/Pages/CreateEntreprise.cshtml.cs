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
            Email = TempData["Email"] as string;
            TempData.Keep("Email");
        }

        public async Task<IActionResult> OnPostCestParti()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0)
            {
                ModelState.AddModelError("", "Utilisateur non connecté.");
                return Page();
            }

            string connStr = _config.GetConnectionString("MyDb");

            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                // Insertion Client_
                var insertClientCmd = new MySqlCommand("INSERT INTO Client_ (Id_Utilisateur) VALUES (@UserId)", conn, transaction);
                insertClientCmd.Parameters.AddWithValue("@UserId", userId);
                await insertClientCmd.ExecuteNonQueryAsync();

                var getLastIdCmd = new MySqlCommand("SELECT LAST_INSERT_ID()", conn, transaction);
                int clientId = Convert.ToInt32(await getLastIdCmd.ExecuteScalarAsync());

                // Insertion Entreprise
                string adresse = $"{Voirie} {Numéro}, {Arrondissement}e";
                var insertEntrepriseCmd = new MySqlCommand(
                    "INSERT INTO Entreprise (Id_Client, Nom_entreprise, Nom_référent, Adresse_entreprise, Num_SIRET) " +
                    "VALUES (@ClientId, @Nom, @Referent, @Adresse, @Siret)", conn, transaction);
                insertEntrepriseCmd.Parameters.AddWithValue("@ClientId", clientId);
                insertEntrepriseCmd.Parameters.AddWithValue("@Nom", NomEntreprise);
                insertEntrepriseCmd.Parameters.AddWithValue("@Referent", NomReferent);
                insertEntrepriseCmd.Parameters.AddWithValue("@Adresse", adresse);
                insertEntrepriseCmd.Parameters.AddWithValue("@Siret", NumeroSiret);

                await insertEntrepriseCmd.ExecuteNonQueryAsync();
                await transaction.CommitAsync();

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
