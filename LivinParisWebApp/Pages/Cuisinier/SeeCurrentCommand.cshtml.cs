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

        public IActionResult OnPostCancelCommande(string commandeId)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0 || string.IsNullOrEmpty(commandeId))
                return RedirectToPage();

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            // Récupérer les deux listes actuelles
            var selectCmd = new MySqlCommand("SELECT Id_Cuisinier, Liste_commandes, Liste_commandes_pretes FROM Cuisinier WHERE Id_Utilisateur = @Uid", conn);
            selectCmd.Parameters.AddWithValue("@Uid", userId);

            int cuisinierId = 0;
            string? listeCommandes = null, listePretes = null;

            using (var reader = selectCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    cuisinierId = reader.GetInt32("Id_Cuisinier");
                    listeCommandes = reader["Liste_commandes"]?.ToString();
                    listePretes = reader["Liste_commandes_pretes"]?.ToString();
                }
            }

            if (cuisinierId == 0)
                return RedirectToPage();

            var commandes = (listeCommandes ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
            var pretes = (listePretes ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();

            if (!pretes.Contains(commandeId))
                return RedirectToPage(); // rien à faire

            pretes.Remove(commandeId);
            commandes.Add(commandeId);

            string newCommandes = string.Join(",", commandes);
            string newPretes = string.Join(",", pretes);

            var updateCmd = new MySqlCommand("UPDATE Cuisinier SET Liste_commandes = @Lc, Liste_commandes_pretes = @Lp WHERE Id_Cuisinier = @Cid", conn);
            updateCmd.Parameters.AddWithValue("@Lc", newCommandes);
            updateCmd.Parameters.AddWithValue("@Lp", newPretes);
            updateCmd.Parameters.AddWithValue("@Cid", cuisinierId);
            updateCmd.ExecuteNonQuery();

            return RedirectToPage();
        }


        public IActionResult OnPostLivrerCommande()
        {
            return RedirectToPage("/Cuisinier/LivraisonCuisinier");
        }
    }
}
