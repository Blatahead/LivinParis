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
            var graphe = new Graphe();

            var cheminNoeuds = graphe.Dijkstra(1, 332);

            var graphe2 = new Graphe2();
            string connStr = _config.GetConnectionString("MyDb");
            graphe2.ChargerDepuisBDD2(connStr);

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