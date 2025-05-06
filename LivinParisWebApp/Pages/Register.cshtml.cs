using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LivinParisWebApp.Pages
{
    public class RegisterModel : PageModel
    {
        #region Propriétés
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
        #endregion

        #region Méthodes

        public void OnGet() { }

        /// <summary>
        /// Inscription utilisateur
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostRegister()
        {
            if (!ModelState.IsValid)
                return Page();
            string connStr = _config.GetConnectionString("MyDb");

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM Utilisateur WHERE Mail_Utilisateur = @Email", conn);
            checkCmd.Parameters.AddWithValue("@Email", Email);
            int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (exists > 0)
            {
                return RedirectToPage("/Login");
            }

            TempData["Email"] = Email;
            TempData["Password"] = Password;

            return RedirectToPage("/ChoixCC");
        }
        #endregion
    }
}