using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibraryRendu2;
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
        public string CheminJson { get; set; }

        //public List<StationNoeud> Chemin { get; set; }
        public void OnGet()
        {
            var graphe = new Graphe();
            string connStr = _config.GetConnectionString("MyDb");
            graphe.ChargerDepuisBDD(connStr);


            //var cheminNoeuds = graphe.Dijkstra(1, 170);
            //var cheminNoeuds = graphe.Dijkstra(96, 300);
            //var cheminNoeuds = graphe.Dijkstra(210, 66);
            //var cheminNoeuds = graphe.Dijkstra(258, 332);
            var cheminNoeuds = graphe.Dijkstra(1, 332);


            // Conversion en DTO pour casser les cycles
            var cheminDTOs = cheminNoeuds.Select(StationConvertisseurs.ToDTO).ToList();
            CheminJson = JsonConvert.SerializeObject(cheminDTOs);

            var stationDTOs = graphe.Stations.Select(st => new StationDTO
                {
                    Id = st.Id,
                    Nom = st.Nom,
                    Latitude = st.Latitude,
                    Longitude = st.Longitude
                })
                .ToList();

            var arcDTOs = graphe.Arcs.Select(arc => new ArcDTO
                {
                    SourceId = arc.Source.Id,
                    SourceLat = arc.Source.Latitude,
                    SourceLong = arc.Source.Longitude,
                    DestLat = arc.Destination.Latitude,
                    DestLong = arc.Destination.Longitude,
                    DestinationId = arc.Destination.Id,
                    Distance = arc.Distance,
                    Ligne = arc.Ligne
                }).ToList();

            ViewData["Stations"] = JsonConvert.SerializeObject(stationDTOs);
            ViewData["Arcs"] = JsonConvert.SerializeObject(arcDTOs);
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