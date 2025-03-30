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
            conn.Open();

            var cmd = new MySqlCommand("SELECT COUNT(*) FROM Utilisateur WHERE Mail_Utilisateur = @Email AND Mdp = @Pwd", conn);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@Pwd", Password);

            var count = Convert.ToInt32(cmd.ExecuteScalar());

            if (count > 0)
            {
                Message = "Connexion réussie !";
                //voir les infos à garder de la connexion (si faut la balaser ou pas)
                // puis changer le /register (c'était un test)
                return RedirectToPage("/Register");
            }
            else
            {
                Message = "Identifiants incorrects.";
                return Page();
            }
        }
    }
}

