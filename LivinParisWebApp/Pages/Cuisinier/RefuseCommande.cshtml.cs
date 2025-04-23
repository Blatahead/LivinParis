using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class RefuseCommandeModel : PageModel
    {
        private readonly IConfiguration _config;
        public int IdLigneCommande { get; set; }

        public RefuseCommandeModel(IConfiguration config)
        {
            _config = config;
        }

        public void OnGet(int idLigneCommande)
        {
            IdLigneCommande = idLigneCommande;
        }

        public IActionResult OnPostSeeCurrentCommandNo()
        {
            return RedirectToPage("/Cuisinier/SeeCurrentCommand");
        }

        public IActionResult OnPostSeeCurrentCommandYes(int idLigneCommande)
        {
            string connStr = _config.GetConnectionString("MyDb");

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            // Id_Commande lié à la ligne
            int idCommande = 0;
            var getCmd = new MySqlCommand("SELECT Id_Commande FROM LigneCommande WHERE Id_LigneCommande = @id", conn);
            getCmd.Parameters.AddWithValue("@id", idLigneCommande);
            var result = getCmd.ExecuteScalar();
            if (result != null)
                idCommande = Convert.ToInt32(result);

            // supp les plats liés à la ligne
            var deletePlats = new MySqlCommand("DELETE FROM Plat_LigneCommande WHERE Id_LigneCommande = @id", conn);
            deletePlats.Parameters.AddWithValue("@id", idLigneCommande);
            deletePlats.ExecuteNonQuery();

            // supp la ligne de commande
            var deleteLigne = new MySqlCommand("DELETE FROM LigneCommande WHERE Id_LigneCommande = @id", conn);
            deleteLigne.Parameters.AddWithValue("@id", idLigneCommande);
            deleteLigne.ExecuteNonQuery();

            // maj liste dans le cuisinier
            int idUtilisateur = int.Parse(HttpContext.Session.GetString("Id_Utilisateur") ?? "0");
            var getListe = new MySqlCommand("SELECT Id_Cuisinier, Liste_commandes FROM Cuisinier WHERE Id_Utilisateur = @idU", conn);
            getListe.Parameters.AddWithValue("@idU", idUtilisateur);

            int idCuisinier = 0;
            string listeCommandes = "";

            using (var reader = getListe.ExecuteReader())
            {
                if (reader.Read())
                {
                    idCuisinier = reader.GetInt32("Id_Cuisinier");
                    listeCommandes = reader["Liste_commandes"]?.ToString() ?? "";
                }
            }

            var commandes = listeCommandes.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList();
            commandes.Remove(idLigneCommande.ToString());

            var updateListe = new MySqlCommand("UPDATE Cuisinier SET Liste_commandes = @liste WHERE Id_Cuisinier = @cid", conn);
            updateListe.Parameters.AddWithValue("@liste", string.Join(",", commandes));
            updateListe.Parameters.AddWithValue("@cid", idCuisinier);
            updateListe.ExecuteNonQuery();

            // supp la commande si plus aucune ligne liée
            var checkLignes = new MySqlCommand("SELECT COUNT(*) FROM LigneCommande WHERE Id_Commande = @idCmd", conn);
            checkLignes.Parameters.AddWithValue("@idCmd", idCommande);
            int nbLignes = Convert.ToInt32(checkLignes.ExecuteScalar());

            if (nbLignes == 0)
            {
                var deleteCommande = new MySqlCommand("DELETE FROM Commande WHERE Num_commande = @idCmd", conn);
                deleteCommande.Parameters.AddWithValue("@idCmd", idCommande);
                deleteCommande.ExecuteNonQuery();
            }

            return RedirectToPage("/Cuisinier/SeeCurrentCommand");
        }
    }
}
