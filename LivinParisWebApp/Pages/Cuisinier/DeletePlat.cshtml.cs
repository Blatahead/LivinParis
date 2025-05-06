using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class DeletePlatModel : PageModel
    {
        #region Attribut
        private readonly IConfiguration _config;
        #endregion

        #region Constrcuteur
        public DeletePlatModel(IConfiguration config)
        {
            _config = config;
        }
        #endregion

        #region Propriete
        [BindProperty(SupportsGet = true)]
        public string NomPlat { get; set; }
        #endregion

        #region Methodes
        public void OnGet()
        {
        }

        /// <summary>
        /// non pour supprimer un plat
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostCuisinierPanelNo()
        {
            return RedirectToPage("/CuisinierPanel");
        }

        /// <summary>
        /// oui pour supprimer un plat
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostCuisinierPanelYes()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0 || string.IsNullOrEmpty(NomPlat))
            {
                ModelState.AddModelError("", "Utilisateur ou plat non défini.");
                return Page();
            }

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            int cuisinierId = 0;
            string? liste = null;

            var getCmd = new MySqlCommand("SELECT Id_Cuisinier, Liste_de_plats FROM Cuisinier WHERE Id_Utilisateur = @Uid", conn);
            getCmd.Parameters.AddWithValue("@Uid", userId);
            using (var reader = await getCmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    cuisinierId = Convert.ToInt32(reader["Id_Cuisinier"]);
                    liste = reader["Liste_de_plats"]?.ToString();
                }
            }

            if (cuisinierId == 0)
            {
                ModelState.AddModelError("", "Cuisinier non trouvé.");
                return Page();
            }

            var deleteCmd = new MySqlCommand("DELETE FROM Plat WHERE Nom_plat = @Nom AND id_Cuisinier = @Cid", conn);
            deleteCmd.Parameters.AddWithValue("@Nom", NomPlat);
            deleteCmd.Parameters.AddWithValue("@Cid", cuisinierId);
            await deleteCmd.ExecuteNonQueryAsync();

            if (!string.IsNullOrEmpty(liste))
            {
                var plats = liste.Split(',').Select(p => p.Trim()).ToList();
                plats.RemoveAll(p => p == NomPlat);
                string nouvelleListe = string.Join(",", plats);

                var updateCmd = new MySqlCommand("UPDATE Cuisinier SET Liste_de_plats = @List WHERE Id_Cuisinier = @Cid", conn);
                updateCmd.Parameters.AddWithValue("@List", nouvelleListe);
                updateCmd.Parameters.AddWithValue("@Cid", cuisinierId);
                await updateCmd.ExecuteNonQueryAsync();
            }

            return RedirectToPage("/CuisinierPanel");
        }
        #endregion
    }
}