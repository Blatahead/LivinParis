using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class SeeCurrentCommandModel : PageModel
    {
        private readonly IConfiguration _config;

        public List<string> ListeCommandes { get; set; } = new();
        public List<string> ListeCommandesPretes { get; set; } = new();

        public SeeCurrentCommandModel(IConfiguration config)
        {
            _config = config;
        }

        public void OnGet()
        {
            int userId = (int)(HttpContext.Session.GetInt32("UserId") ?? 0);
            if (userId == 0)
                return;

            string connStr = _config.GetConnectionString("MyDb");

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            var cmd = new MySqlCommand("SELECT Liste_commandes, Liste_commandes_pretes FROM Cuisinier WHERE Id_Utilisateur = @UserId", conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string commandes = reader.IsDBNull(0) ? "" : reader.GetString(0);
                string pretes = reader.IsDBNull(1) ? "" : reader.GetString(1);

                ListeCommandes = string.IsNullOrWhiteSpace(commandes) ? new List<string>() : commandes.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

                ListeCommandesPretes = string.IsNullOrWhiteSpace(pretes) ? new List<string>() : pretes.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

            }
        }

        public IActionResult OnPostCuisinierPanel()
        {
            return RedirectToPage("/CuisinierPanel");
        }

        public IActionResult OnPostRefuseCommande()
        {
            return RedirectToPage("/Cuisinier/RefuseCommande");
        }

        public IActionResult OnPostDetailsCommande()
        {
            return RedirectToPage("/Cuisinier/DetailsCommande");
        }

        public IActionResult OnPostCancelCommande()
        {
            return RedirectToPage();
        }

        public IActionResult OnPostLivrerCommande()
        {
            return RedirectToPage("/Cuisinier/LivraisonCuisinier");
        }
    }
}
