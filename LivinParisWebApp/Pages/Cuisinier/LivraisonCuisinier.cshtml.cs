using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class LivraisonCuisinierModel : PageModel
    {
        //chargement du graphe
        private readonly IConfiguration _config;


        public LivraisonCuisinierModel(IConfiguration config)
        {
            _config = config;
        }
        public string CheminJson { get; set; }

        public void OnGet()
        {
            var graphe = new Graphe();
            string connStr = _config.GetConnectionString("MyDb");
            graphe.ChargerDepuisBDD(connStr);

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

        public IActionResult OnPostComandeDetails()
        {
            return RedirectToPage("/Cuisinier/DetailsCommande");
        }
        public IActionResult OnPostConfirm()
        {
            return RedirectToPage("/CuisinierPanel");
        }
    }
}