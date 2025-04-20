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
        public List<PlatDisponibleDTO> PanierPlats { get; set; } = new();
        public decimal TotalPanier => PanierPlats.Sum(p =>
        {
            decimal.TryParse(p.Prix, out var prix);
            return prix;
        });
        [BindProperty]
        public PlatAjouteModel ModelAjout { get; set; }
        public class PlatAjouteModel
        {
            public int PlatId { get; set; }
        }
        [BindProperty]
        public PlatSuppressionModel ModelSuppression { get; set; }

        public class PlatSuppressionModel
        {
            public int PlatIdSupp { get; set; }
        }

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
            var platsCmd = new MySqlCommand(@"SELECT p.Num_plat, p.Nom_plat, p.prix_plat, c.Prenom_cuisinier, 
                p.Nombre_de_personne_plat, p.Type_plat, p.Nationalité_plat, p.Date_fabrication_plat, 
                p.Date_péremption_plat, p.Ingrédients_plat, p.Régime_alimentaire_plat, c.Adresse_cuisinier
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
                    catch { }
                }

                PlatsDisponibles.Add(new PlatDisponibleDTO
                {
                    Id = Convert.ToInt32(platsReader["Num_plat"]),
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
            platsReader.Close();
            ViewData["PlatsCoords"] = JsonConvert.SerializeObject(PlatsDisponibles);

            //récupération du panier
            var panierCmd = new MySqlCommand(@"SELECT p.Num_plat, p.Nom_plat, p.prix_plat, c.Prenom_cuisinier
                FROM Panier pa
                JOIN Plat p ON pa.Num_plat = p.Num_plat
                JOIN Cuisinier c ON p.id_Cuisinier = c.Id_Cuisinier
                JOIN Client_ cl ON pa.Id_Client = cl.Id_Client
                WHERE cl.Id_Utilisateur = @id", conn);

            panierCmd.Parameters.AddWithValue("@id", userId);

            using var panierReader = await panierCmd.ExecuteReaderAsync();
            while (await panierReader.ReadAsync())
            {
                PanierPlats.Add(new PlatDisponibleDTO
                {
                    NumPlat = Convert.ToInt32(panierReader["Num_plat"]),
                    Nom = panierReader["Nom_plat"]?.ToString() ?? "",
                    Prix = panierReader["prix_plat"]?.ToString() ?? "",
                    Cuisinier = panierReader["Prenom_cuisinier"]?.ToString() ?? ""
                });
            }
            panierReader.Close();

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

        public async Task<IActionResult> OnPostAddToCartAsync()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0 || ModelAjout?.PlatId == 0)
                return RedirectToPage();

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            var cmdClient = new MySqlCommand("SELECT Id_Client FROM Client_ WHERE Id_Utilisateur = @userId", conn);
            cmdClient.Parameters.AddWithValue("@userId", userId);
            var idClient = Convert.ToInt32(await cmdClient.ExecuteScalarAsync());

            var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM Panier WHERE Id_Client = @idClient AND Num_plat = @platId", conn);
            checkCmd.Parameters.AddWithValue("@idClient", idClient);
            checkCmd.Parameters.AddWithValue("@platId", ModelAjout.PlatId);
            var exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync()) > 0;

            if (!exists)
            {
                var insertCmd = new MySqlCommand("INSERT INTO Panier (Id_Client, Num_plat) VALUES (@idClient, @platId)", conn);
                insertCmd.Parameters.AddWithValue("@idClient", idClient);
                insertCmd.Parameters.AddWithValue("@platId", ModelAjout.PlatId);
                await insertCmd.ExecuteNonQueryAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveFromCartAsync()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            int idPlat = ModelSuppression.PlatIdSupp;
            if (userId == 0 || ModelSuppression?.PlatIdSupp == 0)
                return RedirectToPage();

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            var cmdClient = new MySqlCommand("SELECT Id_Client FROM Client_ WHERE Id_Utilisateur = @userId", conn);
            cmdClient.Parameters.AddWithValue("@userId", userId);
            var idClient = Convert.ToInt32(await cmdClient.ExecuteScalarAsync());

            var deleteCmd = new MySqlCommand("DELETE FROM Panier WHERE Id_Client = @idClient AND Num_plat = @platId", conn);
            deleteCmd.Parameters.AddWithValue("@idClient", idClient);
            deleteCmd.Parameters.AddWithValue("@platId", ModelSuppression.PlatIdSupp);
            await deleteCmd.ExecuteNonQueryAsync();

            return RedirectToPage();
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
        public int Id { get; set; }
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
        public int NumPlat { get; set; }
    }
}