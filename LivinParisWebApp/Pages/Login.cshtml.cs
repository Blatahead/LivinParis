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

        /// <summary>
        /// Connexion avec email et mot de passe
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostLoginAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); 
            }

            string connStr = _config.GetConnectionString("MyDb");

            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            var cmd = new MySqlCommand("SELECT Id_Utilisateur FROM Utilisateur WHERE Mail_Utilisateur = @Email AND Mdp = @Pwd", conn);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@Pwd", Password);

            var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return RedirectToPage("/AnyAccount");
            }

            int userId = reader.GetInt32(0);
            reader.Close();

            HttpContext.Session.SetInt32("UserId", userId);

            cmd = new MySqlCommand("SELECT COUNT(*) FROM Client_ WHERE Id_Utilisateur = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", userId);
            bool isClient = Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;

            cmd = new MySqlCommand("SELECT COUNT(*) FROM Cuisinier WHERE Id_Utilisateur = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", userId);
            bool isCuisinier = Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;

            if (isClient && isCuisinier)
                return RedirectToPage("/CuisinierPanel"); 
            else if (isClient)
                return RedirectToPage("/ClientPanel");
            else if (isCuisinier)
                return RedirectToPage("/CuisinierPanel");
            else if (!isClient && !isCuisinier)
                return RedirectToPage("/ChoixCC");
            return Page();
        }
    }
}