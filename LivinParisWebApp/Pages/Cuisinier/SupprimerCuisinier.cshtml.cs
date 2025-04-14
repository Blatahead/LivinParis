using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class SupprimerCuisinierModel : PageModel
    {
        private readonly IConfiguration _config;

        public SupprimerCuisinierModel(IConfiguration config)
        {
            _config = config;
        }
        public void OnGet()
        {
        }
        public IActionResult OnPostNo()
        {
            return RedirectToPage("/Cuisinier/SettingsCuisinier");
        }

        public IActionResult OnPostYes()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            string connStr = _config.GetConnectionString("MyDb");

            using MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            try
            {
                //supprimer le cuisinier lié à l'utilisateur
                MySqlCommand deleteCmd = new MySqlCommand("DELETE FROM Cuisinier WHERE Id_Utilisateur = @UserId", conn);
                deleteCmd.Parameters.AddWithValue("@UserId", userId.Value);
                deleteCmd.ExecuteNonQuery();

                //supprimer la session
                HttpContext.Session.Clear();

                return RedirectToPage("/Register");
            }
            catch (Exception ex)
            {
                return Page();
            }
        }
    }
}
