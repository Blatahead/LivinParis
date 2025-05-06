using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class SupprimerCuisinierModel : PageModel
    {
        #region Attribut
        private readonly IConfiguration _config;
        #endregion

        #region Constructeur
        public SupprimerCuisinierModel(IConfiguration config)
        {
            _config = config;
        }
        #endregion

        #region Methodes
        public void OnGet()
        {
        }

        /// <summary>
        /// clic sur l'option non
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostNo()
        {
            return RedirectToPage("/Cuisinier/SettingsCuisinier");
        }

        /// <summary>
        /// clic sur l'option oui
        /// </summary>
        /// <returns></returns>
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
                MySqlCommand deleteCmd = new MySqlCommand("DELETE FROM Cuisinier WHERE Id_Utilisateur = @UserId", conn);
                deleteCmd.Parameters.AddWithValue("@UserId", userId.Value);
                deleteCmd.ExecuteNonQuery();

                HttpContext.Session.Clear();

                return RedirectToPage("/Register");
            }
            catch (Exception ex)
            {
                return Page();
            }
        }
        #endregion
    }
}