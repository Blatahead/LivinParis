using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class AddPlatModel : PageModel
    {
        private readonly IConfiguration _config;

        public AddPlatModel(IConfiguration config)
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
        [BindProperty] public string Nationalite { get; set; }
        [Required]
        [BindProperty] public string Regime { get; set; }
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
        [BindProperty] public string Ingredients { get; set; }

        public async Task<IActionResult> OnPostCuisinierPanelConfirm()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0)
            {
                ModelState.AddModelError("", "Utilisateur non connecté.");
                return Page();
            }

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            // 1. Récupérer Id_Cuisinier + ancienne liste
            int cuisinierId = 0;
            string? listeExistante = null;

            var getCmd = new MySqlCommand("SELECT Id_Cuisinier, Liste_de_plats FROM Cuisinier WHERE Id_Utilisateur = @Uid", conn);
            getCmd.Parameters.AddWithValue("@Uid", userId);

            using var reader = await getCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                cuisinierId = Convert.ToInt32(reader["Id_Cuisinier"]);
                listeExistante = reader["Liste_de_plats"] as string;
            }
            reader.Close();

            if (cuisinierId == 0)
            {
                ModelState.AddModelError("", "Cuisinier introuvable.");
                return Page();
            }

            // 2. Construire les dates en DateTime
            string fabricationStr = $"{AnneeCreation}-{MoisCreation}-{JourCreation}";
            string peremptionStr = $"{AnneePerem}-{MoisPerem}-{JourPerem}";

            if (!DateTime.TryParse(fabricationStr, out DateTime fabrication) ||
                !DateTime.TryParse(peremptionStr, out DateTime peremption))
            {
                ModelState.AddModelError("", "Format de date invalide.");
                return Page();
            }

            // 3. Insertion dans Plat
            var insertCmd = new MySqlCommand(@"
        INSERT INTO Plat 
        (Num_plat, Nom_plat, Nombre_de_personne_plat, Type_plat, Nationalité_plat, 
         Date_péremption_plat, prix_plat, Ingrédients_plat, Régime_alimentaire_plat, 
         Photo_plat, Date_fabrication_plat, Num_platJ, id_Cuisinier)
        VALUES 
        (@Num, @Nom, @Nb, @Type, @Natio, @Peremption, @Prix, @Ingredients, 
         @Regime, @Photo, @Fabrication, @NumPlatJ, @Cid)", conn);

            // Générer un numéro de plat unique
            var rand = new Random();
            int numPlat = rand.Next(100000, 999999);
            string numPlatJ = Guid.NewGuid().ToString("N").Substring(0, 10);

            insertCmd.Parameters.AddWithValue("@Num", numPlat);
            insertCmd.Parameters.AddWithValue("@Nom", NomDuPlat);
            insertCmd.Parameters.AddWithValue("@Nb", NbDePersonnes);
            insertCmd.Parameters.AddWithValue("@Type", Type);
            insertCmd.Parameters.AddWithValue("@Natio", Nationalite);
            insertCmd.Parameters.AddWithValue("@Peremption", peremption);
            insertCmd.Parameters.AddWithValue("@Prix", Convert.ToDecimal(Prix.Replace(",", ".")));
            insertCmd.Parameters.AddWithValue("@Ingredients", Ingredients);
            insertCmd.Parameters.AddWithValue("@Regime", Regime);
            insertCmd.Parameters.AddWithValue("@Photo", ""); // à gérer plus tard
            insertCmd.Parameters.AddWithValue("@Fabrication", fabrication);
            insertCmd.Parameters.AddWithValue("@NumPlatJ", numPlatJ);
            insertCmd.Parameters.AddWithValue("@Cid", cuisinierId);

            // 4. Update la liste des plats
            string updatedListe = string.IsNullOrEmpty(listeExistante)
                ? NomDuPlat
                : $"{listeExistante},{NomDuPlat}";

            var updateCmd = new MySqlCommand("UPDATE Cuisinier SET Liste_de_plats = @Liste WHERE Id_Cuisinier = @Cid", conn);
            updateCmd.Parameters.AddWithValue("@Liste", updatedListe);
            updateCmd.Parameters.AddWithValue("@Cid", cuisinierId);

            try
            {
                await insertCmd.ExecuteNonQueryAsync(); // insertion dans Plat
                await updateCmd.ExecuteNonQueryAsync(); // update de la liste
                return RedirectToPage("/CuisinierPanel");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erreur : " + ex.Message);
                return Page();
            }
        }


        public IActionResult OnPostCuisinierPanelRetour()
        {
            return RedirectToPage("/CuisinierPanel");
        }
    }
}
