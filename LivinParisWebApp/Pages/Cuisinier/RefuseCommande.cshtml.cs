using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class RefuseCommandeModel : PageModel
    {
        private readonly IConfiguration _config;

        public RefuseCommandeModel(IConfiguration config)
        {
            _config = config;
        }
        public void OnGet()
        {
        }
        public IActionResult OnPostSeeCurrentCommandNo()
        {
            return RedirectToPage("/Cuisinier/SeeCurrentCommand");
        }

        public IActionResult OnPostSeeCurrentCommandYes(int numCommande)
        {
            string connStr = _config.GetConnectionString("MyDb");

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            //Supprimer la commande de la table Commande
            var deleteCmd = new MySqlCommand("DELETE FROM Commande WHERE Num_commande = @id", conn);
            deleteCmd.Parameters.AddWithValue("@id", numCommande);
            deleteCmd.ExecuteNonQuery();

            //Retirer la commande de la Liste_commandes du cuisinier
            int idUtilisateur = int.Parse(HttpContext.Session.GetString("Id_Utilisateur") ?? "0");
            var getCmd = new MySqlCommand("SELECT Liste_commandes FROM Cuisinier WHERE Id_Utilisateur = @idU", conn);
            getCmd.Parameters.AddWithValue("@idU", idUtilisateur);
            string? listeCommandes = getCmd.ExecuteScalar()?.ToString();

            if (!string.IsNullOrEmpty(listeCommandes))
            {
                var commandes = listeCommandes.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList();
                commandes.Remove(numCommande.ToString());

                string nouvelleListe = string.Join(",", commandes);

                var updateCmd = new MySqlCommand("UPDATE Cuisinier SET Liste_commandes = @liste WHERE Id_Utilisateur = @idU", conn);
                updateCmd.Parameters.AddWithValue("@liste", nouvelleListe);
                updateCmd.Parameters.AddWithValue("@idU", idUtilisateur);
                updateCmd.ExecuteNonQuery();
            }

            TempData["Message"] = $"Commande {numCommande} refusée avec succès.";

            return RedirectToPage("/Cuisinier/SeeCurrentCommand");
        }
    }
}
