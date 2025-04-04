using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace LivinParisWebApp.Pages
{
    public class CreateCuisinierModel : PageModel
    {
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
        public string Numéro { get; set; }

        public string Message { get; set; }
        public CreateCuisinierModel(IConfiguration configuration)
        {
            _config = configuration;
        }

        public IActionResult OnPostCestParti()
        {
            string connStr = _config.GetConnectionString("MyDb");

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            // stocker user id
            int userId = (int)(HttpContext.Session.GetInt32("UserId") ?? 0);
            if (userId == 0)
            {
                Message = "Erreur : utilisateur non connecté.";
                return Page();
            }

            string adresse = $"{Numéro} {Voirie}, {Arrondissement}e";

            var cmd = new MySqlCommand("INSERT INTO Cuisinier (Prenom_cuisinier, Nom_particulier, Adresse_cuisinier, Id_Utilisateur) VALUES (@Prenom, @Nom, @Adresse, @IdUser)", conn);
            cmd.Parameters.AddWithValue("@Prenom", FirstName);
            cmd.Parameters.AddWithValue("@Nom", Name);
            cmd.Parameters.AddWithValue("@Adresse", adresse);
            cmd.Parameters.AddWithValue("@IdUser", userId);

            try
            {
                cmd.ExecuteNonQuery();
                return RedirectToPage("/CuisinierPanel");
            }
            catch (Exception ex)
            {
                Message = $"Erreur lors de la création : {ex.Message}";
                Console.WriteLine("Erreur SQL : " + ex.ToString());
                return Page();
            }
        }

        public IActionResult OnPostChoixCC()
        {
            return RedirectToPage("/ChoixCC");
        }

        public void OnGet()
        {
        }
    }
}