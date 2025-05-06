using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class SeeCurrentCommandModel : PageModel
    {
        #region Attribut
        private readonly IConfiguration _config;
        #endregion

        #region Proprietes
        public List<LigneCommandeInfo> CommandesEnCours { get; set; } = new();
        public List<LigneCommandeInfo> CommandesPretes { get; set; } = new();
        #endregion

        #region Constrcuteur
        public SeeCurrentCommandModel(IConfiguration config)
        {
            _config = config;
        }
        #endregion

        #region Methodes
        /// <summary>
        /// au lancement de la page
        /// </summary>
        public void OnGet()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return;

            string connStr = _config.GetConnectionString("MyDb");

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            var getCmds = new MySqlCommand("SELECT Liste_commandes, Liste_commandes_pretes FROM Cuisinier WHERE Id_Utilisateur = @uid", conn);
            getCmds.Parameters.AddWithValue("@uid", userId);

            string commandesRaw = "", pretesRaw = "";
            using (var reader = getCmds.ExecuteReader())
            {
                if (reader.Read())
                {
                    commandesRaw = reader.IsDBNull(0) ? "" : reader.GetString(0);
                    pretesRaw = reader.IsDBNull(1) ? "" : reader.GetString(1);
                }
            }

            var commandes = commandesRaw
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                .Where(id => id != -1)
                .ToArray();

            var pretes = pretesRaw
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                .Where(id => id != -1)
                .ToArray();

            if (commandes.Length > 0)
                CommandesEnCours = RecupererDetailsLignes(conn, commandes);

            if (pretes.Length > 0)
                CommandesPretes = RecupererDetailsLignes(conn, pretes);
        }

        /// <summary>
        /// recuperer les details des lignes de commande
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="commandeIds"></param>
        /// <returns></returns>
        private List<LigneCommandeInfo> RecupererDetailsLignes(MySqlConnection conn, int[] commandeIds)
        {
            var result = new List<LigneCommandeInfo>();
            if (commandeIds.Length == 0) return result;

            var idList = string.Join(",", commandeIds.Select(id => $"'{id}'"));

            var query = $@"SELECT lc.Id_LigneCommande, lc.Id_Commande, COALESCE(p.Prenom_particulier, e.Nom_r�f�rent, 'Inconnu') AS ClientNom, COUNT(pl.Num_Plat) AS NbPlats
                FROM LigneCommande lc
                JOIN Commande c ON lc.Id_Commande = c.Num_commande
                JOIN Client_ cl ON cl.Id_Utilisateur = c.Id_Utilisateur
                LEFT JOIN Particulier p ON cl.Id_Client = p.Id_Client
                LEFT JOIN Entreprise e ON cl.Id_Client = e.Id_Client
                LEFT JOIN Plat_LigneCommande pl ON lc.Id_LigneCommande = pl.Id_LigneCommande
                WHERE lc.Id_LigneCommande IN ({idList})
                GROUP BY lc.Id_LigneCommande, lc.Id_Commande, ClientNom";

            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new LigneCommandeInfo
                {
                    Id_LigneCommande = reader.GetInt32("Id_LigneCommande"),
                    Num_commande = reader.GetInt32("Id_Commande"),
                    ClientNom = reader.GetString("ClientNom"),
                    NbPlats = reader.GetInt32("NbPlats")
                });
            }

            return result;
        }

        /// <summary>
        /// retour au panel cuisinier
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostCuisinierPanel() => RedirectToPage("/CuisinierPanel");

        /// <summary>
        /// refuser une commande
        /// </summary>
        /// <param name="idLigneCommande"></param>
        /// <returns></returns>
        public IActionResult OnPostRefuseCommande(int idLigneCommande)
        {
            return RedirectToPage("/Cuisinier/RefuseCommande", new { idLigneCommande });
        }

        /// <summary>
        /// voir les details d'une commande
        /// </summary>
        /// <param name="idLigneCommande"></param>
        /// <returns></returns>
        public IActionResult OnPostDetailsCommande(int idLigneCommande)
        {
            TempData["IdLigneCommande"] = idLigneCommande;
            return RedirectToPage("/Cuisinier/DetailsCommande");
        }

        /// <summary>
        /// annuler une commande prete � livrer
        /// </summary>
        /// <param name="commandeId"></param>
        /// <returns></returns>
        public IActionResult OnPostCancelCommande(string commandeId)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0 || string.IsNullOrEmpty(commandeId) || !int.TryParse(commandeId, out int idLigneCommande))
                return RedirectToPage();

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            var selectCmd = new MySqlCommand("SELECT Id_Cuisinier, Liste_commandes, Liste_commandes_pretes FROM Cuisinier WHERE Id_Utilisateur = @Uid", conn);
            selectCmd.Parameters.AddWithValue("@Uid", userId);

            int cuisinierId = 0;
            string? listeCommandesStr = null, listePretesStr = null;

            using (var reader = selectCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    cuisinierId = reader.GetInt32("Id_Cuisinier");
                    listeCommandesStr = reader["Liste_commandes"]?.ToString();
                    listePretesStr = reader["Liste_commandes_pretes"]?.ToString();
                }
            }

            if (cuisinierId == 0) return RedirectToPage();

            var commandes = (listeCommandesStr ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                .Where(id => id != -1)
                .ToList();

            var pretes = (listePretesStr ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                .Where(id => id != -1)
                .ToList();

            if (!pretes.Contains(idLigneCommande)) return RedirectToPage();

            pretes.Remove(idLigneCommande);

            if (!commandes.Contains(idLigneCommande))
                commandes.Add(idLigneCommande);

            var updateCmd = new MySqlCommand("UPDATE Cuisinier SET Liste_commandes = @Lc, Liste_commandes_pretes = @Lp WHERE Id_Cuisinier = @Cid", conn);
            updateCmd.Parameters.AddWithValue("@Lc", string.Join(",", commandes));
            updateCmd.Parameters.AddWithValue("@Lp", string.Join(",", pretes));
            updateCmd.Parameters.AddWithValue("@Cid", cuisinierId);
            updateCmd.ExecuteNonQuery();

            return RedirectToPage();
        }

        /// <summary>
        /// clic pour aller sur la page de livraison
        /// </summary>
        /// <param name="idLigneCommande"></param>
        /// <returns></returns>
        public IActionResult OnPostLivrerCommande(int idLigneCommande)
        {
            TempData["IdLigneCommande"] = idLigneCommande;
            return RedirectToPage("/Cuisinier/LivraisonCuisinier");
        }
        #endregion
    }

    public class LigneCommandeInfo
    {
        public int Id_LigneCommande { get; set; }
        public int Num_commande { get; set; }
        public string ClientNom { get; set; } = "Inconnu";
        public int NbPlats { get; set; }
    }
}