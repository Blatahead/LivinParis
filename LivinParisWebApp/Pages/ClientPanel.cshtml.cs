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
        public List<PlatDisponibleDTO> PlatsDisponibles { get; set; } = new();

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

            // Récupération des plats disponibles
            var platsCmd = new MySqlCommand(@"SELECT p.Nom_plat, p.prix_plat, c.Prenom_cuisinier, p.Nombre_de_personne_plat, p.Type_plat, 
                p.Nationalité_plat, p.Date_fabrication_plat, p.Date_péremption_plat, p.Ingrédients_plat, p.Régime_alimentaire_plat, c.Adresse_cuisinier
                FROM Plat p
                JOIN Cuisinier c ON p.id_Cuisinier = c.Id_Cuisinier", conn);

            using var platsReader = await platsCmd.ExecuteReaderAsync();
            while (await platsReader.ReadAsync())
            {
                var adresse = platsReader["Adresse_cuisinier"]?.ToString();
                double lat = 0, lon = 0;
                if (!string.IsNullOrEmpty(adresse))
                {
                    try
                    {
                        (lat, lon) = await ClassLibrary.Convertisseur_coordonnees.GetCoordinatesAsync(adresse);
                    }
                    catch
                    {
                        lat = 0;
                        lon = 0;
                    }
                }

                PlatsDisponibles.Add(new PlatDisponibleDTO
                {
                    Nom = platsReader["Nom_plat"]?.ToString() ?? "",
                    Prix = platsReader["prix_plat"]?.ToString() ?? "",
                    Cuisinier = platsReader["Prenom_cuisinier"]?.ToString() ?? "",
                    NbPersonnes = platsReader["Nombre_de_personne_plat"]?.ToString() ?? "",
                    Type = platsReader["Type_plat"]?.ToString() ?? "",
                    Nationalite = platsReader["Nationalité_plat"]?.ToString() ?? "",
                    Fabrication = Convert.ToDateTime(platsReader["Date_fabrication_plat"]).ToString("dd/MM/yy"),
                    Peremption = Convert.ToDateTime(platsReader["Date_péremption_plat"]).ToString("dd/MM/yy"),
                    Ingredients = platsReader["Ingrédients_plat"]?.ToString() ?? "",
                    Regime = platsReader["Régime_alimentaire_plat"]?.ToString() ?? "",
                    Latitude = lat,
                    Longitude = lon
                });
            }
            ViewData["PlatsCoords"] = JsonConvert.SerializeObject(PlatsDisponibles);

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
    public class PlatDisponibleDTO
    {
        public string Nom { get; set; }
        public string Prix { get; set; }
        public string Cuisinier { get; set; }
        public string NbPersonnes { get; set; }
        public string Type { get; set; }
        public string Nationalite { get; set; }
        public string Fabrication { get; set; }
        public string Peremption { get; set; }
        public string Ingredients { get; set; }
        public string Regime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }


}