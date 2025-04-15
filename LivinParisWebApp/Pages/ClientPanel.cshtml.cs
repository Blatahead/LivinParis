using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibrary;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace LivinParisWebApp.Pages
{
    public class ClientPanelModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public ClientPanelModel(IConfiguration config, IWebHostEnvironment env)
        {
            _env = env;
            _config = config;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            //vérfication qu'un utilisateur est connecté
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0) return RedirectToPage("/Login");

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            //vérification qu'un Client est associé au userID
            MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM Client_ WHERE Id_Utilisateur = @id", conn);
            cmd.Parameters.AddWithValue("@id", userId);

            long count = (long)cmd.ExecuteScalar();

            if (count == 0)
            {
                return RedirectToPage("/NoClientAccount");
            }

            //pas fini
            var graphe = new Graphe();
            graphe.ChargerDepuisBDD(connStr);

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
            return Page();
        }

        public IActionResult OnPostClientPanel()
        {
            return Page();
        }
        public IActionResult OnPostCuisinierPanel()
        {
            //checker l'existance du cuisinier
            return RedirectToPage("/CuisinierPanel");
        }
    }
}