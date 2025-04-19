using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MySql.Data.MySqlClient;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class ChangeTodaysPlatModel : PageModel
    {
        private readonly IConfiguration _config;

        public ChangeTodaysPlatModel(IConfiguration config)
        {
            _config = config;
        }

        [Required]
        [BindProperty] public string NomDuPlat { get; set; }
        [Required]
        [BindProperty] public string Prix { get; set; }
        [Required]
        [BindProperty] public string NbDePersonnes { get; set; }
        [Required]
        [BindProperty] public string Type { get; set; }

        [Required]
        [BindProperty] public string JourCreation { get; set; }
        [Required]
        [BindProperty] public string MoisCreation { get; set; }
        [Required]
        [BindProperty] public string AnneeCreation { get; set; }

        [Required]
        [BindProperty] public string JourPerem { get; set; }
        [Required]
        [BindProperty] public string MoisPerem { get; set; }
        [Required]
        [BindProperty] public string AnneePerem { get; set; }
        [Required]
        [BindProperty] public string Nationalite { get; set; }
        [Required]
        [BindProperty] public string Regime { get; set; }
        [Required]
        [BindProperty] public string Ingredients { get; set; }

        public void OnGet() { }

        public IActionResult OnPostCuisinierPanelRetour()
        {
            return RedirectToPage("/CuisinierPanel");
        }

        public async Task<IActionResult> OnPostCuisinierPanelConfirm()
        {
            int userId = (int)(HttpContext.Session.GetInt32("UserId") ?? 0);
            if (userId == 0)
            {
                ModelState.AddModelError("", "Utilisateur non connecté.");
                return Page();
            }

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            //Id_Cuisinier
            var getCuisinierCmd = new MySqlCommand("SELECT Id_Cuisinier FROM Cuisinier WHERE Id_Utilisateur = @UserId", conn);
            getCuisinierCmd.Parameters.AddWithValue("@UserId", userId);
            object result = await getCuisinierCmd.ExecuteScalarAsync();
            if (result == null)
            {
                ModelState.AddModelError("", "Cuisinier introuvable.");
                return Page();
            }

            int cuisinierId = Convert.ToInt32(result);

            string fabrication = $"{AnneeCreation}-{MoisCreation}-{JourCreation}";
            string peremption = $"{AnneePerem}-{MoisPerem}-{JourPerem}";
            string photo = ""; // pas encore supporté

            //Num_platJ unique
            int idPlat = 1; // valeur au cas où y'a pas de plat
            var dernierNumPlat = new MySqlCommand("SELECT MAX(Num_platJ) FROM Plat_du_jour", conn);
            object dernierNumPlatResult = dernierNumPlat.ExecuteScalar();
            if (dernierNumPlatResult != DBNull.Value && result != null)
            {
                idPlat = Convert.ToInt32(result) + 1;
            }

            var insertCmd = new MySqlCommand(
                @"INSERT INTO Plat_du_jour (Num_platJ, Nom_platJ, Nombre_de_personneJ, Type_platJ, Nationalité_platJ, Date_péremption_platJ, prix_platJ, 
                Ingrédients_platJ, Régime_alimentaire_platJ, Photo_platJ, Date_fabrication_platJ, id_Cuisinier)
                VALUES (@Num, @Nom, @NbPers, @Type, @Natio, @Peremption, @Prix, @Ingredients, @Regime, @Photo, @Fabrication, @CuisinierId)", conn);

            insertCmd.Parameters.AddWithValue("@Num", idPlat);
            insertCmd.Parameters.AddWithValue("@Nom", NomDuPlat);
            insertCmd.Parameters.AddWithValue("@NbPers", NbDePersonnes);
            insertCmd.Parameters.AddWithValue("@Type", Type);
            insertCmd.Parameters.AddWithValue("@Natio", Nationalite);
            insertCmd.Parameters.AddWithValue("@Peremption", peremption);
            insertCmd.Parameters.AddWithValue("@Prix", Prix);
            insertCmd.Parameters.AddWithValue("@Ingredients", Ingredients);
            insertCmd.Parameters.AddWithValue("@Regime", Regime);
            insertCmd.Parameters.AddWithValue("@Photo", photo); // vide pour l’instant
            insertCmd.Parameters.AddWithValue("@Fabrication", fabrication);
            insertCmd.Parameters.AddWithValue("@CuisinierId", cuisinierId);

            try
            {
                await insertCmd.ExecuteNonQueryAsync();
                return RedirectToPage("/CuisinierPanel");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erreur lors de l'ajout du plat : " + ex.Message);
                return Page();
            }
        }

    }
}