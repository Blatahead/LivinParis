using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace LivinParis.Pages
{
    public class CreateParticulierModel : PageModel
    {
        #region Propri�t�s
        private readonly IConfiguration _config;
        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Arrondissement { get; set; }

        [BindProperty]
        public string Voirie { get; set; }

        [BindProperty]
        public string Num�ro { get; set; }

        public string Message { get; set; }

        public CreateParticulierModel(IConfiguration config)
        {
            _config = config;
        }
        #endregion
        #region M�thodes

        public async Task<IActionResult> OnPostCestParti()
        {
            string email = TempData["Email"]?.ToString();
            string password = TempData["Password"]?.ToString();

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                if (userId == 0 && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    var insertUserCmd = new MySqlCommand(
                        "INSERT INTO Utilisateur (Mail_Utilisateur, Mdp) VALUES (@Email, @Pwd); SELECT LAST_INSERT_ID();",
                        conn, transaction);
                    insertUserCmd.Parameters.AddWithValue("@Email", email);
                    insertUserCmd.Parameters.AddWithValue("@Pwd", password);

                    userId = Convert.ToInt32(await insertUserCmd.ExecuteScalarAsync());

                    HttpContext.Session.SetInt32("UserId", userId);
                }
                else if (userId == 0)
                {
                    ModelState.AddModelError("", "Utilisateur non identifi�.");
                    return Page();
                }
                var insertClientCmd = new MySqlCommand(
                    "INSERT INTO Client_ (Id_Utilisateur) VALUES (@UserId); SELECT LAST_INSERT_ID();", conn, transaction);
                insertClientCmd.Parameters.AddWithValue("@UserId", userId);
                int clientId = Convert.ToInt32(await insertClientCmd.ExecuteScalarAsync());
                string adresse = $"{Voirie} {Num�ro}, 750{Arrondissement} Paris";
                var insertPartCmd = new MySqlCommand(
                    "INSERT INTO Particulier (Prenom_particulier, Nom_particulier, Adresse_particulier, Id_Client) " +
                    "VALUES (@Prenom, @Nom, @Adresse, @ClientId)", conn, transaction);
                insertPartCmd.Parameters.AddWithValue("@Prenom", FirstName);
                insertPartCmd.Parameters.AddWithValue("@Nom", Name);
                insertPartCmd.Parameters.AddWithValue("@Adresse", adresse);
                insertPartCmd.Parameters.AddWithValue("@ClientId", clientId);
                await insertPartCmd.ExecuteNonQueryAsync();

                await transaction.CommitAsync();

                TempData["Email"] = email;
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
        #endregion
    }
}