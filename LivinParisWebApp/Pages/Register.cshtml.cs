using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LivinParisWebApp.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _config;

        public string Message { get; set; }

        [Required]
        [BindProperty]
        public string Email { get; set; }

        [Required]
        [BindProperty]
        public string Password { get; set; }

        public RegisterModel(IConfiguration config)
        {
            _config = config;
        }

        public void OnGet() { }

        public IActionResult OnPostRegister()
        {
            if (!ModelState.IsValid)
                return Page();

            string connStr = _config.GetConnectionString("MyDb");

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            try
            {
                // V�rifier si l'utilisateur existe d�j�
                var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM Utilisateur WHERE Mail_Utilisateur = @Email", conn);
                checkCmd.Parameters.AddWithValue("@Email", Email);
                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (exists > 0)
                {
                    Message = "Cet email est d�j� utilis�.";
                    return Page();
                }

                // Ins�rer utilisateur + r�cup�rer son ID
                var insertCmd = new MySqlCommand(@"
                    INSERT INTO Utilisateur (Mail_Utilisateur, Mdp)
                    VALUES (@Email, @Pwd);
                    SELECT LAST_INSERT_ID();", conn);

                insertCmd.Parameters.AddWithValue("@Email", Email);
                insertCmd.Parameters.AddWithValue("@Pwd", Password);

                int userId = Convert.ToInt32(insertCmd.ExecuteScalar());

                if (userId == 0)
                {
                    ModelState.AddModelError("", "�chec de r�cup�ration de l'ID utilisateur.");
                    return Page();
                }

                // Stockage session & TempData
                HttpContext.Session.SetInt32("UserId", userId);
                TempData["Email"] = Email;

                return RedirectToPage("/ChoixCC");
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                {
                    Message = "Cet email est d�j� utilis�.";
                }
                else
                {
                    Message = "Erreur inattendue : " + ex.Message;
                }
                return Page();
            }
        }
    }
}