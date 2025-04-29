using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibrary;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text.Json;




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
        public async Task OnGet()
        {
            //Graphe 1
            var graphe = new ClassLibrary.Graphe();
            string connStr = _config.GetConnectionString("MyDb");
            graphe.ChargerDepuisBDD(connStr);


            //affichage graphe avec Bellman Ford
            var cheminNoeuds = Parcours.BellmanFord(1, 332, graphe.Arcs);

            //affichage graphe avec Dijkstra
            //var cheminNoeuds = graphe.Dijkstra(1, 170);
            //var cheminNoeuds = graphe.Dijkstra(96, 300);
            //var cheminNoeuds = graphe.Dijkstra(210, 66);
            //var cheminNoeuds = graphe.Dijkstra(258, 332);
            //var cheminNoeuds = graphe.Dijkstra(1, 332);


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

            //Graphe 2
            var graphe2 = new ClassLibrary.Graphe2();
            string connStr2 = _config.GetConnectionString("MyDb");
            graphe2.ChargerDepuisBDD2(connStr2);


            var noeuds = graphe2.Noeuds.Select(n => new { id = n.Id, group = n.Type }).ToList();
            var liens = graphe2.Liens.Select(l => new { source = l.Noeud1.Id, target = l.Noeud2.Id, label = l.Libelle }).ToList();

            ViewData["NoeudsJson"] = JsonConvert.SerializeObject(noeuds);
            ViewData["LiensJson"] = JsonConvert.SerializeObject(liens);


            var clientsDTOs = new List<object>();
            var cuisiniersDTOs = new List<object>();

            foreach (var noeud in graphe2.Noeuds)
            {
                if (noeud.Type == "Cuisinier")
                {
                    string adresseCuisinier = "";

                    using (var conn = new MySqlConnection(_config.GetConnectionString("MyDb")))
                    {
                        await conn.OpenAsync();

                        var cmd = new MySqlCommand("SELECT Adresse_cuisinier FROM Cuisinier WHERE Id_Cuisinier = @id", conn);
                        cmd.Parameters.AddWithValue("@id", noeud.Id);
                        var result = await cmd.ExecuteScalarAsync();

                        if (result != null)
                            adresseCuisinier = result.ToString();
                    }
                    if (!string.IsNullOrEmpty(adresseCuisinier))
                    {
                        var coords = await GetCoordinatesFromAdresseAsync(adresseCuisinier);

                        cuisiniersDTOs.Add(new
                        {
                            id = noeud.Id,
                            latitude = coords.latitude,
                            longitude = coords.longitude
                        });
                    }
                }
                else if (noeud.Type == "Client")
                {
                    string adresseClient = "";

                    using (var conn = new MySqlConnection(_config.GetConnectionString("MyDb")))
                    {
                        await conn.OpenAsync();

                        var cmd = new MySqlCommand("SELECT Adresse_particulier FROM Particulier WHERE Id_Client = @id", conn);
                        cmd.Parameters.AddWithValue("@id", noeud.Id);
                        var result = await cmd.ExecuteScalarAsync();

                        if (result != null)
                        {
                            adresseClient = result.ToString();
                        }
                        else
                        {
                            cmd = new MySqlCommand("SELECT Adresse_entreprise FROM Entreprise WHERE Id_Client = @id", conn);
                            cmd.Parameters.AddWithValue("@id", noeud.Id);
                            result = await cmd.ExecuteScalarAsync();

                            if (result != null)
                                adresseClient = result.ToString();
                        }
                    }

                    if (!string.IsNullOrEmpty(adresseClient))
                    {
                        var coords = await GetCoordinatesFromAdresseAsync(adresseClient);

                        clientsDTOs.Add(new
                        {
                            id = noeud.Id,
                            latitude = coords.latitude,
                            longitude = coords.longitude
                        });
                    }
                }
            }

            ViewData["Clients"] = JsonConvert.SerializeObject(clientsDTOs);
            ViewData["Cuisiniers"] = JsonConvert.SerializeObject(cuisiniersDTOs);
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

        public async Task<(double latitude, double longitude)> GetCoordinatesFromAdresseAsync(string adresse)
        {
            if (string.IsNullOrWhiteSpace(adresse))
                return (0, 0);

            var (latitude, longitude) = await Convertisseur_coordonnees.GetCoordinatesAsync(adresse);
            return (latitude, longitude);
        }












        public IActionResult OnPostLoadStationInBDD()
        {
            var import = new ImportStations(_config);

            string cheminFichier = Path.Combine(_env.WebRootPath, "data", "stations.mtx");

            if (!System.IO.File.Exists(cheminFichier))
            {
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

        public IActionResult OnPostVoidBDD()
        {

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