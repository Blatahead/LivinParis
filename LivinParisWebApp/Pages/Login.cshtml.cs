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

            var reader = await cmd.ExecuteReaderAsync();

            if (!reader.Read())
            {
                Message = "Identifiants incorrects.";
                return Page();
            }

            int userId = reader.GetInt32(0);
            reader.Close();

            // check client
            bool isClient = false;
            cmd = new MySqlCommand("SELECT COUNT(*) FROM Client_ WHERE Id_Utilisateur = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", userId);
            isClient = Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;

            // check cuisinier
            bool isCuisinier = false;
            cmd = new MySqlCommand("SELECT COUNT(*) FROM Cuisinier WHERE Id_Utilisateur = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", userId);
            isCuisinier = Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;

            // Redirection
            if (isClient && isCuisinier)
                return RedirectToPage("/ClientPanel");
            else if (isClient)
                return RedirectToPage("/ClientPanel");
            else if (isCuisinier)
                return RedirectToPage("/CuisinierPanel");

            Message = "Aucun rôle associé à ce compte.";
            return Page();
        }
    }
}

