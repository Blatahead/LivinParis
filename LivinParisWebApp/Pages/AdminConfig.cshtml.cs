using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibraryRendu2;
using static Org.BouncyCastle.Math.EC.ECCurve;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace LivinParisWebApp.Pages
{
    public class AdminConfigModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public AdminConfigModel(IConfiguration config, IWebHostEnvironment env)
        {
            _env = env;
            _config = config;
        }
        public void OnGet()
        {
            var graphe = new Graphe();
            string connStr = _config.GetConnectionString("MyDb");
            graphe.ChargerDepuisBDD(connStr);

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            ViewData["Stations"] = JsonConvert.SerializeObject(graphe.Stations, settings);
            ViewData["Arcs"] = JsonConvert.SerializeObject(graphe.Arcs, settings);
        }

        public IActionResult OnPostDeleteContenuStations()
        {
            string connStr = _config.GetConnectionString("MyDb");

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            var cmd = new MySqlCommand("DELETE FROM Station", conn);
            cmd.ExecuteNonQuery();

            TempData["Message"] = "La table Station a été vidée avec succès.";
            return RedirectToPage();
        }

        public IActionResult OnPostLoadStationInBDD()
        {
            var import = new ImportStations(_config);

            string cheminFichier = Path.Combine(_env.WebRootPath, "data", "stations.mtx");

            if (!System.IO.File.Exists(cheminFichier))
            {
                TempData["Message"] = $"Fichier introuvable : {cheminFichier}";
                return Page();
            }

            try
            {
                import.ImporterDepuisMTX(cheminFichier);
                TempData["Message"] = "Importation réussie";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Erreur d'importation : {ex.Message}";
            }

            return Page();
        }

        public IActionResult OnPostGenererGraphe()
        {
            var graphe = new Graphe();
            string connStr = _config.GetConnectionString("MyDb");
            graphe.ChargerDepuisBDD(connStr);

            TempData["Message"] = "Graphe généré avec succès.";
            return Page();
        }

    }
}
