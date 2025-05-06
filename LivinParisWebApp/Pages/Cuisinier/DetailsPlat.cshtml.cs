using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class DetailsPlatModel : PageModel
    {
        #region Attribut
        private readonly IConfiguration _config;
        #endregion

        #region Contructeur
        public DetailsPlatModel(IConfiguration config)
        {
            _config = config;
        }
        #endregion

        #region Proprietes
        [BindProperty(SupportsGet = true)]
        public string NomPlat { get; set; }

        public string Prix { get; set; }
        public string NbPersonnes { get; set; }
        public string Nationalite { get; set; }
        public string Regime { get; set; }
        public string Fabrication { get; set; }
        public string Peremption { get; set; }
        public string Ingredients { get; set; }
        public string? PhotoPath { get; set; }
        #endregion

        #region Methodes
        /// <summary>
        /// au lancement de la page
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0 || string.IsNullOrEmpty(NomPlat))
                return RedirectToPage("/CuisinierPanel");

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(@"SELECT * FROM Plat 
                WHERE Nom_plat = @Nom AND id_Cuisinier = (SELECT Id_Cuisinier FROM Cuisinier WHERE Id_Utilisateur = @Uid)", conn);

            cmd.Parameters.AddWithValue("@Nom", NomPlat);
            cmd.Parameters.AddWithValue("@Uid", userId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                Prix = reader["prix_plat"]?.ToString();
                NbPersonnes = reader["Nombre_de_personne_plat"]?.ToString();
                Nationalite = reader["Nationalité_plat"]?.ToString();
                Regime = reader["Régime_alimentaire_plat"]?.ToString();
                Fabrication = Convert.ToDateTime(reader["Date_fabrication_plat"]).ToString("dd/MM/yy");
                Peremption = Convert.ToDateTime(reader["Date_péremption_plat"]).ToString("dd/MM/yy");
                Ingredients = reader["Ingrédients_plat"]?.ToString();
                PhotoPath = reader["Photo_plat"]?.ToString();
            }

            return Page();
        }

        /// <summary>
        /// retour vers le panel cuisinier
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostCuisinierPanel()
        {
            return RedirectToPage("/CuisinierPanel");
        }
        #endregion
    }
}