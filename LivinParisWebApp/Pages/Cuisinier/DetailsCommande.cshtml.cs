using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class DetailsCommandeModel : PageModel
    {
        #region Attribut
        private readonly IConfiguration _config;
        #endregion

        #region Constructeur
        public DetailsCommandeModel(IConfiguration config)
        {
            _config = config;
        }
        #endregion

        #region Proprietes
        public List<string> NomsPlats { get; set; } = new();
        public decimal PrixTotal { get; set; }
        public string DateLivraison { get; set; } = "";
        public string LieuLivraison { get; set; } = "";
        #endregion

        #region Methodes
        /// <summary>
        /// au lancement de la page
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync()
        {
            if (!TempData.TryGetValue("IdLigneCommande", out var ligneObj) || ligneObj is not int idLigneCommande)
                return RedirectToPage("/Cuisinier/SeeCurrentCommand");

            TempData.Keep("IdLigneCommande");

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(@"SELECT p.Nom_plat, p.prix_plat, lc.DateLivraison, lc.LieuLivraison
                FROM Plat_LigneCommande plc
                JOIN Plat p ON plc.Num_Plat = p.Num_plat
                JOIN LigneCommande lc ON plc.Id_LigneCommande = lc.Id_LigneCommande
                WHERE plc.Id_LigneCommande = @id", conn);

            cmd.Parameters.AddWithValue("@id", idLigneCommande);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                NomsPlats.Add(reader.GetString("Nom_plat"));

                if (decimal.TryParse(reader["prix_plat"]?.ToString(), out var prix))
                    PrixTotal += prix;

                if (string.IsNullOrEmpty(DateLivraison))
                    DateLivraison = Convert.ToDateTime(reader["DateLivraison"]).ToString("dd/MM/yy");

                if (string.IsNullOrEmpty(LieuLivraison))
                    LieuLivraison = reader["LieuLivraison"].ToString() ?? "";
            }

            return Page();
        }

        /// <summary>
        /// bouton retour
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostRetour() => RedirectToPage("/Cuisinier/SeeCurrentCommand");

        /// <summary>
        /// redirection vers la livraison cuisinier
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostLivrerCommande()
        {
            if (!TempData.TryGetValue("IdLigneCommande", out var ligneObj) || ligneObj is not int idLigneCommande)
                return RedirectToPage("/Cuisinier/SeeCurrentCommand");

            TempData.Keep("IdLigneCommande");

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return RedirectToPage("/Login");

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            var selectCmd = new MySqlCommand("SELECT Id_Cuisinier, Liste_commandes, Liste_commandes_pretes FROM Cuisinier WHERE Id_Utilisateur = @Uid", conn);
            selectCmd.Parameters.AddWithValue("@Uid", userId);

            int cuisinierId = 0;
            string? commandes = null, pretes = null;

            using (var reader = await selectCmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    cuisinierId = reader.GetInt32("Id_Cuisinier");
                    commandes = reader["Liste_commandes"]?.ToString();
                    pretes = reader["Liste_commandes_pretes"]?.ToString();
                }
            }

            if (cuisinierId == 0)
                return RedirectToPage("/Cuisinier/SeeCurrentCommand");

            var listeCommandes = (commandes ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                .Where(id => id != -1)
                .ToList();

            var listePretes = (pretes ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                .Where(id => id != -1)
                .ToList();

            listeCommandes.Remove(idLigneCommande);
            if (!listePretes.Contains(idLigneCommande))
                listePretes.Add(idLigneCommande);

            var updateCmd = new MySqlCommand("UPDATE Cuisinier SET Liste_commandes = @Cmds, Liste_commandes_pretes = @Pretes WHERE Id_Cuisinier = @Cid", conn);
            updateCmd.Parameters.AddWithValue("@Cmds", string.Join(",", listeCommandes));
            updateCmd.Parameters.AddWithValue("@Pretes", string.Join(",", listePretes));
            updateCmd.Parameters.AddWithValue("@Cid", cuisinierId);
            await updateCmd.ExecuteNonQueryAsync();

            return RedirectToPage("/Cuisinier/SeeCurrentCommand");
        }
        #endregion
    }
}