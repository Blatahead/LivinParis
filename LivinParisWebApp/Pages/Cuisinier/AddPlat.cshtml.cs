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

        [BindProperty]
        public bool UsePlatJour { get; set; }


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

            // Récupération Id_Cuisinier
            int cuisinierId;
            string? listeExistante = null;
            var getCmd = new MySqlCommand("SELECT Id_Cuisinier, Liste_de_plats FROM Cuisinier WHERE Id_Utilisateur = @Uid", conn);
            getCmd.Parameters.AddWithValue("@Uid", userId);

            using var reader = await getCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                cuisinierId = Convert.ToInt32(reader["Id_Cuisinier"]);
                listeExistante = reader["Liste_de_plats"] as string;
            }
            else
            {
                ModelState.AddModelError("", "Cuisinier introuvable.");
                return Page();
            }
            reader.Close();

            int numPlatJour;

            if (UsePlatJour)
            {
                //Récupération du plat du jour existant
                var getPlatJourCmd = new MySqlCommand("SELECT Num_platJ FROM Plat_du_jour WHERE id_Cuisinier = @Cid AND Est_plat_du_jour = TRUE", conn);
                getPlatJourCmd.Parameters.AddWithValue("@Cid", cuisinierId);
                object result = await getPlatJourCmd.ExecuteScalarAsync();

                if (result == null)
                {
                    ModelState.AddModelError("", "Aucun plat du jour trouvé.");
                    return Page();
                }

                numPlatJour = Convert.ToInt32(result);
            }
            else
            {
                //Création d’un nouveau plat du jour (faux)
                var maxNumPlatJCmd = new MySqlCommand("SELECT MAX(Num_platJ) FROM Plat_du_jour", conn);
                object maxResult = await maxNumPlatJCmd.ExecuteScalarAsync();
                numPlatJour = maxResult != DBNull.Value ? Convert.ToInt32(maxResult) + 1 : 1;

                var insertPlatJourCmd = new MySqlCommand(@"INSERT INTO Plat_du_jour (Num_platJ, Nom_platJ, Nombre_de_personneJ, Type_platJ, 
                    Nationalité_platJ, Date_péremption_platJ, prix_platJ, Ingrédients_platJ, Régime_alimentaire_platJ, Photo_platJ, Date_fabrication_platJ, id_Cuisinier, Est_plat_du_jour)
                    VALUES (@Num, @Nom, @Nb, @Type, @Natio, @Peremp, @Prix, @Ingredients, @Regime, @Photo, @Fab, @Cid, FALSE)", conn);

                insertPlatJourCmd.Parameters.AddWithValue("@Num", numPlatJour);
                insertPlatJourCmd.Parameters.AddWithValue("@Nom", NomDuPlat);
                insertPlatJourCmd.Parameters.AddWithValue("@Nb", NbDePersonnes);
                insertPlatJourCmd.Parameters.AddWithValue("@Type", Type);
                insertPlatJourCmd.Parameters.AddWithValue("@Natio", Nationalite);
                insertPlatJourCmd.Parameters.AddWithValue("@Peremp", $"{AnneePerem}-{MoisPerem}-{JourPerem}");
                insertPlatJourCmd.Parameters.AddWithValue("@Prix", Convert.ToDecimal(Prix.Replace(",", ".")));
                insertPlatJourCmd.Parameters.AddWithValue("@Ingredients", Ingredients);
                insertPlatJourCmd.Parameters.AddWithValue("@Regime", Regime);
                insertPlatJourCmd.Parameters.AddWithValue("@Photo", "");
                insertPlatJourCmd.Parameters.AddWithValue("@Fab", $"{AnneeCreation}-{MoisCreation}-{JourCreation}");
                insertPlatJourCmd.Parameters.AddWithValue("@Cid", cuisinierId);

                await insertPlatJourCmd.ExecuteNonQueryAsync();
            }

            //Insertion dans Plat
            int numPlat = 1;
            var dernierNumPlat = new MySqlCommand("SELECT MAX(Num_plat) FROM Plat", conn);
            object dernierNumPlatResult = await dernierNumPlat.ExecuteScalarAsync();
            if (dernierNumPlatResult != DBNull.Value)
                numPlat = Convert.ToInt32(dernierNumPlatResult) + 1;

            var insertPlatCmd = new MySqlCommand(@"INSERT INTO Plat (Num_plat, Nom_plat, Nombre_de_personne_plat, Type_plat, Nationalité_plat, 
                Date_péremption_plat, prix_plat, Ingrédients_plat, Régime_alimentaire_plat, Photo_plat, Date_fabrication_plat, Num_platJ, id_Cuisinier)
                VALUES (@Num, @Nom, @Nb, @Type, @Natio, @Peremp, @Prix, @Ingredients, @Regime, @Photo, @Fab, @NumPlatJ, @Cid)", conn);

            insertPlatCmd.Parameters.AddWithValue("@Num", numPlat);
            insertPlatCmd.Parameters.AddWithValue("@Nom", NomDuPlat);
            insertPlatCmd.Parameters.AddWithValue("@Nb", NbDePersonnes);
            insertPlatCmd.Parameters.AddWithValue("@Type", Type);
            insertPlatCmd.Parameters.AddWithValue("@Natio", Nationalite);
            insertPlatCmd.Parameters.AddWithValue("@Peremp", $"{AnneePerem}-{MoisPerem}-{JourPerem}");
            insertPlatCmd.Parameters.AddWithValue("@Prix", Convert.ToDecimal(Prix.Replace(",", ".")));
            insertPlatCmd.Parameters.AddWithValue("@Ingredients", Ingredients);
            insertPlatCmd.Parameters.AddWithValue("@Regime", Regime);
            insertPlatCmd.Parameters.AddWithValue("@Photo", "");
            insertPlatCmd.Parameters.AddWithValue("@Fab", $"{AnneeCreation}-{MoisCreation}-{JourCreation}");
            insertPlatCmd.Parameters.AddWithValue("@NumPlatJ", numPlatJour);
            insertPlatCmd.Parameters.AddWithValue("@Cid", cuisinierId);

            var updateListeCmd = new MySqlCommand("UPDATE Cuisinier SET Liste_de_plats = @Liste WHERE Id_Cuisinier = @Cid", conn);
            string nouvelleListe = string.IsNullOrEmpty(listeExistante) ? $"{NomDuPlat}" : $"{listeExistante},{NomDuPlat}";
            updateListeCmd.Parameters.AddWithValue("@Liste", nouvelleListe);
            updateListeCmd.Parameters.AddWithValue("@Cid", cuisinierId);

            try
            {
                await insertPlatCmd.ExecuteNonQueryAsync();
                await updateListeCmd.ExecuteNonQueryAsync();
                return RedirectToPage("/CuisinierPanel");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erreur : " + ex.Message);
                return Page();
            }
        }


        public async Task<IActionResult> OnPostCochePlatDuJour()
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

            // Récupération Id_Cuisinier
            var getCidCmd = new MySqlCommand("SELECT Id_Cuisinier FROM Cuisinier WHERE Id_Utilisateur = @Uid", conn);
            getCidCmd.Parameters.AddWithValue("@Uid", userId);
            object result = await getCidCmd.ExecuteScalarAsync();

            if (result == null)
            {
                ModelState.AddModelError("", "Cuisinier introuvable.");
                return Page();
            }

            int cuisinierId = Convert.ToInt32(result);

            // Récupération du plat du jour
            var getPlatCmd = new MySqlCommand("SELECT * FROM Plat_du_jour WHERE id_Cuisinier = @Cid", conn);
            getPlatCmd.Parameters.AddWithValue("@Cid", cuisinierId);

            using var reader = await getPlatCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                NomDuPlat = reader["Nom_platJ"].ToString();
                Prix = reader["prix_platJ"].ToString();
                NbDePersonnes = reader["Nombre_de_personneJ"].ToString();
                Nationalite = reader["Nationalité_platJ"].ToString();
                Regime = reader["Régime_alimentaire_platJ"].ToString();
                Type = reader["Type_platJ"].ToString();
                Ingredients = reader["Ingrédients_platJ"].ToString();

                // Dates
                if (DateTime.TryParse(reader["Date_fabrication_platJ"].ToString(), out DateTime fabrication))
                {
                    JourCreation = fabrication.Day.ToString("D2");
                    MoisCreation = fabrication.Month.ToString("D2");
                    AnneeCreation = fabrication.Year.ToString();
                }

                if (DateTime.TryParse(reader["Date_péremption_platJ"].ToString(), out DateTime peremp))
                {
                    JourPerem = peremp.Day.ToString("D2");
                    MoisPerem = peremp.Month.ToString("D2");
                    AnneePerem = peremp.Year.ToString();
                }
            }
            else
            {
                ModelState.AddModelError("", "Aucun plat du jour trouvé.");
            }
            ModelState.Clear();
            return Page();
        }



        public IActionResult OnPostCuisinierPanelRetour()
        {
            return RedirectToPage("/CuisinierPanel");
        }
    }
}