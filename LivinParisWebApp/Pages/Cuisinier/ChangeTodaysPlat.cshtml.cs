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
        [BindProperty]
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }



        public async Task OnGetAsync()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0)
                return;

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            // Récupérer Id_Cuisinier
            var getCidCmd = new MySqlCommand("SELECT Id_Cuisinier FROM Cuisinier WHERE Id_Utilisateur = @uid", conn);
            getCidCmd.Parameters.AddWithValue("@uid", userId);
            object? result = await getCidCmd.ExecuteScalarAsync();
            if (result == null) return;

            int cuisinierId = Convert.ToInt32(result);

            // Récupérer le plat du jour actuel
            var getPlatCmd = new MySqlCommand("SELECT * FROM Plat_du_jour WHERE id_Cuisinier = @Cid AND Est_plat_du_jour = TRUE", conn);
            getPlatCmd.Parameters.AddWithValue("@Cid", cuisinierId);

            using var reader = await getPlatCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                NomDuPlat = reader["Nom_platJ"].ToString();
                Prix = reader["prix_platJ"].ToString();
                NbDePersonnes = reader["Nombre_de_personneJ"].ToString();
                Type = reader["Type_platJ"].ToString();
                Nationalite = reader["Nationalité_platJ"].ToString();
                Regime = reader["Régime_alimentaire_platJ"].ToString();
                Ingredients = reader["Ingrédients_platJ"].ToString();
                ImageUrl = reader["Photo_platJ"]?.ToString();

                if (DateTime.TryParse(reader["Date_fabrication_platJ"].ToString(), out DateTime fabrication))
                {
                    JourCreation = fabrication.Day.ToString("D2");
                    MoisCreation = fabrication.Month.ToString("D2");
                    AnneeCreation = fabrication.Year.ToString();
                }

                if (DateTime.TryParse(reader["Date_péremption_platJ"].ToString(), out DateTime peremption))
                {
                    JourPerem = peremption.Day.ToString("D2");
                    MoisPerem = peremption.Month.ToString("D2");
                    AnneePerem = peremption.Year.ToString();
                }
            }
        }

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

            // Met à FALSE tous les anciens plats du jour du cuisinier
            var resetCmd = new MySqlCommand("UPDATE Plat_du_jour SET Est_plat_du_jour = FALSE WHERE id_Cuisinier = @IdCuisinier", conn);
            resetCmd.Parameters.AddWithValue("@IdCuisinier", cuisinierId);
            await resetCmd.ExecuteNonQueryAsync();

            int idPlat = 1;
            var dernierNumPlat = new MySqlCommand("SELECT MAX(Num_platJ) FROM Plat_du_jour", conn);
            object dernierNumPlatResult = await dernierNumPlat.ExecuteScalarAsync();
            if (dernierNumPlatResult != DBNull.Value && dernierNumPlatResult != null)
            {
                idPlat = Convert.ToInt32(dernierNumPlatResult) + 1;
            }
            string photo = "";
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var ext = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();
                if (!new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(ext))
                {
                    ModelState.AddModelError("", "Format d’image non valide.");
                    return Page();
                }
                if (ImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "Fichier trop volumineux (max 2 Mo).");
                    return Page();
                }

                var imageFileName = Guid.NewGuid().ToString() + ext;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/plats", imageFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                photo = "/images/plats/" + imageFileName;
            }

            var insertCmd = new MySqlCommand(
                @"INSERT INTO Plat_du_jour (Num_platJ, Nom_platJ, Nombre_de_personneJ, Type_platJ, Nationalité_platJ, Date_péremption_platJ, prix_platJ, 
                Ingrédients_platJ, Régime_alimentaire_platJ, Photo_platJ, Date_fabrication_platJ, id_Cuisinier, Est_plat_du_jour)
                VALUES (@Num, @Nom, @NbPers, @Type, @Natio, @Peremption, @Prix, @Ingredients, @Regime, @Photo, @Fabrication, @CuisinierId, TRUE)", conn);

            insertCmd.Parameters.AddWithValue("@Num", idPlat);
            insertCmd.Parameters.AddWithValue("@Nom", NomDuPlat);
            insertCmd.Parameters.AddWithValue("@NbPers", NbDePersonnes);
            insertCmd.Parameters.AddWithValue("@Type", Type);
            insertCmd.Parameters.AddWithValue("@Natio", Nationalite);
            insertCmd.Parameters.AddWithValue("@Peremption", peremption);
            insertCmd.Parameters.AddWithValue("@Prix", Prix);
            insertCmd.Parameters.AddWithValue("@Ingredients", Ingredients);
            insertCmd.Parameters.AddWithValue("@Regime", Regime);
            insertCmd.Parameters.AddWithValue("@Photo", photo);
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
