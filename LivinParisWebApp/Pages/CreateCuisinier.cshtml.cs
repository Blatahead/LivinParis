using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace LivinParisWebApp.Pages
{
    public class CreateCuisinierModel : PageModel
    {
        private readonly IConfiguration _config;

        [BindProperty] public string FirstName { get; set; }
        [BindProperty] public string Name { get; set; }
        [BindProperty] public string Arrondissement { get; set; }
        [BindProperty] public string Voirie { get; set; }
        [BindProperty] public string Numéro { get; set; }

        public string Message { get; set; }

        public CreateCuisinierModel(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void OnGet()
        {
            TempData.Keep("Email");
            TempData.Keep("Password");
        }

        public IActionResult OnPostCestParti()
        {
            string email = TempData["Email"] as string;
            string password = TempData["Password"] as string;

            string connStr = _config.GetConnectionString("MyDb");

            using var conn = new MySqlConnection(connStr);
            conn.Open();
            using var transaction = conn.BeginTransaction();

            try
            {
                int userId;

                // Cas 1 : On vient du processus Register
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    var insertUserCmd = new MySqlCommand(
                        "INSERT INTO Utilisateur (Mail_Utilisateur, Mdp) VALUES (@Email, @Pwd); SELECT LAST_INSERT_ID();",
                        conn, transaction);
                    insertUserCmd.Parameters.AddWithValue("@Email", email);
                    insertUserCmd.Parameters.AddWithValue("@Pwd", password);

                    userId = Convert.ToInt32(insertUserCmd.ExecuteScalar());

                    if (userId == 0)
                        throw new Exception("Impossible de récupérer l'identifiant utilisateur.");

                    HttpContext.Session.SetInt32("UserId", userId);
                }
                else
                {
                    // Cas 2 : Utilisateur déjà connecté (via panel client)
                    userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                    if (userId == 0)
                        throw new Exception("Utilisateur non connecté.");
                }

                string adresse = $"{Numéro} {Voirie}, {Arrondissement}e";

                var insertCuisinierCmd = new MySqlCommand(
                    "INSERT INTO Cuisinier (Prenom_cuisinier, Nom_particulier, Adresse_cuisinier, Id_Utilisateur) " +
                    "VALUES (@Prenom, @Nom, @Adresse, @IdUser)", conn, transaction);

                insertCuisinierCmd.Parameters.AddWithValue("@Prenom", FirstName);
                insertCuisinierCmd.Parameters.AddWithValue("@Nom", Name);
                insertCuisinierCmd.Parameters.AddWithValue("@Adresse", adresse);
                insertCuisinierCmd.Parameters.AddWithValue("@IdUser", userId);

                insertCuisinierCmd.ExecuteNonQuery();
                transaction.Commit();

                return RedirectToPage("/CuisinierPanel");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Message = "Erreur : " + ex.Message;
                return Page();
            }
        }

        public IActionResult OnPostChoixCC()
        {
            return RedirectToPage("/ChoixCC");
        }
    }
}