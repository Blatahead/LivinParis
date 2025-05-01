using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibrary;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text.Json;
using System.Data;
using System.Reflection.Metadata.Ecma335;




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
        public List<ClientCommandesDTO> ClientsCommandes { get; set; } 
        public List<ClientAvecPlusieursCommandesDTO> ClientsActifs { get; set; } 
        public List<PlatJamaisCommandeDTO> PlatsNonCommandes { get; set; } 
        public List<CuisinierSansCommandeDTO> CuisiniersDispos { get; set; } 
        public List<CommandeSansPlatBonMarcheDTO> CommandesSansPlatAbordable { get; set; } 
        public List<PlatPlusCommandeDTO> PlatLePlusCommande { get; set; } 
        public List<MontantMoyenClientDTO> MontantsMoyensParClient { get; set; } 
        public List<ClientCommandePlusChereDTO> ClientsAvecCommandeMax { get; set; } 
        public List<CommandeVarieeDTO> CommandesDiversifiees { get; set; } 
        public List<ClientPlatCherDTO> ClientsPlatsChers { get; set; }


        //public List<StationNoeud> Chemin { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            ///// Graphe 1 ////
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
            //////////////////

            //// Graphe 2 ////
            var graphe2 = new ClassLibrary.Graphe2();
            string connStr2 = _config.GetConnectionString("MyDb");
            await graphe2.ChargerDepuisBDD2(connStr2);

            var noeuds = graphe2.Noeuds.Select(n => new
            { 
                id = n.Id, 
                type = n.Type,
                latitude = n.Latitude,
                longitude = n.Longitude
            }).ToList();

            var liens = graphe2.Liens.Select(l => new 
            { 
                source = l.Noeud1.Id, 
                target = l.Noeud2.Id, 
                label = l.Libelle 
            }).ToList();

            ViewData["NoeudsJson"] = JsonConvert.SerializeObject(noeuds);
            ViewData["LiensJson"] = JsonConvert.SerializeObject(liens);
            ///////////////////

            //// Statistiques ////
            //envoi des clients et cuisiniers
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
            //fin d'envoi des clients et cuisiniers

            //requetage des stats de la page + envoi sur la page html
            using (var conn = new MySqlConnection(_config.GetConnectionString("MyDb")))
            {
                await conn.OpenAsync();

                var cmd = new MySqlCommand(@"SELECT c.Id_Client, COUNT(co.Num_commande) AS nb_commandes FROM Client_ c
                    LEFT JOIN Commande co ON c.Id_Client = co.Id_Utilisateur
                    GROUP BY c.Id_Client
                    ORDER BY nb_commandes DESC;", conn);

                using var reader = await cmd.ExecuteReaderAsync();
                ClientsCommandes = new List<ClientCommandesDTO>();
                while (await reader.ReadAsync())
                {
                    ClientsCommandes.Add(new ClientCommandesDTO
                    {
                        ID_client = reader.GetInt32("Id_Client"),
                        Nb_commandes = reader.GetInt32("nb_commandes")
                    });
                }
            }

            using (var conn = new MySqlConnection(_config.GetConnectionString("MyDb")))
            {
                await conn.OpenAsync();

                var cmd = new MySqlCommand(@"SELECT c.Id_Client, COUNT(co.Num_commande) AS nb_commandes FROM Client_ c
                    JOIN Commande co ON c.Id_Client = co.Id_Utilisateur
                    GROUP BY c.Id_Client
                    HAVING COUNT(co.Num_commande) > 2;", conn);

                using var reader = await cmd.ExecuteReaderAsync();
                ClientsActifs = new List<ClientAvecPlusieursCommandesDTO>();
                while (await reader.ReadAsync())
                {
                    ClientsActifs.Add(new ClientAvecPlusieursCommandesDTO
                    {
                        ID_client = reader.GetInt32("Id_Client"),
                        Nb_commandes = reader.GetInt32("nb_commandes")
                    });
                }
            }

            using (var conn = new MySqlConnection(_config.GetConnectionString("MyDb")))
            {
                await conn.OpenAsync();

                var cmd = new MySqlCommand(@"SELECT cu.Id_Cuisinier, cu.Nom_particulier FROM Cuisinier cu
                    LEFT JOIN Commande co ON cu.Id_Cuisinier = co.Id_Utilisateur
                    WHERE co.Num_commande IS NULL;", conn);

                using var reader = await cmd.ExecuteReaderAsync();
                CuisiniersDispos = new List<CuisinierSansCommandeDTO>();
                while (await reader.ReadAsync())
                {
                    CuisiniersDispos.Add(new CuisinierSansCommandeDTO
                    {
                        ID_cuisinier = reader.GetInt32("Id_Cuisinier"),
                        Nom = reader.GetString("Nom_particulier")
                    });
                }
            }

            using (var conn = new MySqlConnection(_config.GetConnectionString("MyDb")))
            {
                await conn.OpenAsync();

                var cmd = new MySqlCommand(@"SELECT co.Num_commande FROM Commande co
                    WHERE NOT EXISTS (
                        SELECT 1
                        FROM Commande dc
                        JOIN Plat p ON dc.liste_plats = p.Nom_plat
                        WHERE dc.Num_commande = co.Num_commande AND p.prix_plat <= 15
                    );", conn);

                using var reader = await cmd.ExecuteReaderAsync();
                CommandesSansPlatAbordable = new List<CommandeSansPlatBonMarcheDTO>();
                while (await reader.ReadAsync())
                {
                    CommandesSansPlatAbordable.Add(new CommandeSansPlatBonMarcheDTO
                    {
                        ID_commande = reader.GetInt32("Num_commande")
                    });
                }
            }

            using (var conn = new MySqlConnection(_config.GetConnectionString("MyDb")))
            {
                await conn.OpenAsync();

                var cmd = new MySqlCommand(@"SELECT p.Nom_plat, COUNT(*) AS fois_commande FROM Commande dc
                    JOIN Plat p ON dc.liste_plats = p.Nom_plat
                    GROUP BY p.Num_plat, p.Nom_plat
                    ORDER BY fois_commande DESC
                    LIMIT 1;", conn);

                using var reader = await cmd.ExecuteReaderAsync();
                PlatLePlusCommande = new List<PlatPlusCommandeDTO>();
                while (await reader.ReadAsync())
                {
                    PlatLePlusCommande.Add(new PlatPlusCommandeDTO
                    {
                        Nom = reader.GetString("Nom_plat"),
                        Fois_commande = reader.GetInt32("fois_commande")
                    });
                }
            }


            using (var conn = new MySqlConnection(_config.GetConnectionString("MyDb")))
            {
                await conn.OpenAsync();

                var cmd = new MySqlCommand(@"SELECT DISTINCT c.Id_Client FROM Client_ c
                    JOIN Commande co ON c.Id_Client = co.Id_Utilisateur
                    JOIN Commande dc ON co.Num_commande = dc.Num_commande
                    JOIN Plat p ON dc.liste_plats = p.Nom_plat
                    WHERE p.prix_plat > 20;", conn);

                using var reader = await cmd.ExecuteReaderAsync();
                ClientsPlatsChers = new List<ClientPlatCherDTO>();
                while (await reader.ReadAsync())
                {
                    ClientsPlatsChers.Add(new ClientPlatCherDTO
                    {
                        ID_client = reader.GetInt32("Id_Client")
                    });
                }
            }
            ///////////////////////
            return Page();
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