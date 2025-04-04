using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.ComponentModel.DataAnnotations;

namespace LivinParis.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _config;
        [Required]
        [BindProperty]
        public string Email { get; set; }

        [Required]
        [BindProperty]
        public string Password { get; set; }

        public string Message { get; set; } = "";

        public LoginModel(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostLoginAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // recharge la page avec les erreurs
            }

            string connStr = _config.GetConnectionString("MyDb");

            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            // R�cup�rer l'ID utilisateur si les identifiants sont corrects
            var cmd = new MySqlCommand("SELECT Id_Utilisateur FROM Utilisateur WHERE Mail_Utilisateur = @Email AND Mdp = @Pwd", conn);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@Pwd", Password);

            var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                Message = "Identifiants incorrects.";
                return Page();
            }

            int userId = reader.GetInt32(0);
            reader.Close();

            // Stocker l'ID en session
            HttpContext.Session.SetInt32("UserId", userId);

            // Check r�les
            // V�rifier les r�les (Client / Cuisinier)
            // check client
            cmd = new MySqlCommand("SELECT COUNT(*) FROM Client_ WHERE Id_Utilisateur = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", userId);
            bool isClient = Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;

            // check cuisinier
            cmd = new MySqlCommand("SELECT COUNT(*) FROM Cuisinier WHERE Id_Utilisateur = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", userId);
            bool isCuisinier = Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;


            // Redirection selon le r�le
            if (isClient && isCuisinier)
                return RedirectToPage("/CuisinierPanel"); // par d�faut Cuisinier
            else if (isClient)
                return RedirectToPage("/ClientPanel");
            else if (isCuisinier)
                return RedirectToPage("/CuisinierPanel");

            Message = "Aucun r�le associ� � ce compte.";
            return Page();
        }
    }
}