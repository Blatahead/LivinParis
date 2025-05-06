using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibrary;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text.Json;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;




namespace LivinParisWebApp.Pages
{
    public class AdminConfigModel : PageModel
    {
        #region Constructeur
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public AdminConfigModel(IConfiguration config, IWebHostEnvironment env)
        {
            _env = env;
            _config = config;
        }
        #endregion

        #region Proprietes
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
        #endregion

        /// <summary>
        /// Se déclenche à l'arrivée sur la page AdminConfig
        /// Remplis toutes les valeurs, affiche les graphes
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync()
        {
            #region Graphe Station & PCC
            var graphe = new ClassLibrary.Graphe();
            string connStr = _config.GetConnectionString("MyDb");
            graphe.ChargerDepuisBDD(connStr);

            //var cheminNoeuds = Parcours.BellmanFord(20, 95, graphe.Arcs);
            var cheminNoeuds = Parcours.BellmanFord(246, 223, graphe.Arcs);

            //var cheminNoeuds = graphe.Dijkstra(1, 332);

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
            #endregion

            #region Graphe coloré
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
            #endregion

            #region Clients & Cuisiniers
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
            #endregion

            #region Statistiques
            #region NbCommandes
            using (var conn = new MySqlConnection(_config.GetConnectionString("MyDb")))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT Nb_Commandes FROM Statistiques WHERE Id_Statistiques = 3", conn);
                var result = await cmd.ExecuteScalarAsync();
                ViewData["NbCommandesTotales"] = result != null ? Convert.ToInt32(result) : 0;
            }
            #endregion

            #region Client Actifs
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
            #endregion

            #region Cuisinier dispo
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
            #endregion

            #region CommandeSansPlatAbordables
            using (var conn = new MySqlConnection(_config.GetConnectionString("MyDb")))
            {
                await conn.OpenAsync();

                var cmd = new MySqlCommand(@"
                    SELECT DISTINCT c.Num_commande
                    FROM Commande c
                    JOIN LigneCommande lc ON lc.Id_Commande = c.Num_commande
                    JOIN Plat_LigneCommande plc ON plc.Id_LigneCommande = lc.Id_LigneCommande
                    JOIN Plat p ON p.Num_plat = plc.Num_Plat
                    GROUP BY c.Num_commande
                    HAVING MIN(p.Prix_plat) > 10;", conn);

                using var reader = await cmd.ExecuteReaderAsync();
                CommandesSansPlatAbordable = new List<CommandeSansPlatBonMarcheDTO>();
                while (await reader.ReadAsync())
                {
                    CommandesSansPlatAbordable.Add(new CommandeSansPlatBonMarcheDTO
                    {
                        ID_commande = reader.GetInt32("Num_commande")
                    });
                }
                if (CommandesSansPlatAbordable.Count == 0)
                {
                    CommandesSansPlatAbordable.Add(new CommandeSansPlatBonMarcheDTO
                    {
                        ID_commande = -1
                    });
                }
            }
            #endregion

            #region Plat le plus commandé
            using (var conn = new MySqlConnection(_config.GetConnectionString("MyDb")))
            {
                await conn.OpenAsync();

                PlatLePlusCommande = new List<PlatPlusCommandeDTO>();

                var cmd = new MySqlCommand(@"
                    SELECT p.Nom_plat, COUNT(*) AS fois_commande
                    FROM Plat p
                    JOIN Plat_LigneCommande plc ON p.Num_plat = plc.Num_Plat
                    GROUP BY p.Num_plat, p.Nom_plat
                    ORDER BY fois_commande DESC
                    LIMIT 1;", conn);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    PlatLePlusCommande.Add(new PlatPlusCommandeDTO
                    {
                        Nom = reader.GetString("Nom_plat"),
                        Fois_commande = reader.GetInt32("fois_commande")
                    });
                }
            }
            #endregion

            #region Depuis table Stats
            using (var conn = new MySqlConnection(_config.GetConnectionString("MyDb")))
            {
                await conn.OpenAsync();

                var cmdStats = new MySqlCommand(@"
                SELECT 
                    Nb_Paniers_Validés,
                    Argent_Total,
                    Temps_Livraison_Total,
                    Distance_Totale,
                    Nb_Plats_Livrés
                FROM Statistiques
                WHERE Id_Statistiques = 3;", conn);

                using var reader = await cmdStats.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    ViewData["NbPaniers"] = reader.GetInt32("Nb_Paniers_Validés");
                    ViewData["ArgentTotal"] = reader.GetDecimal("Argent_Total");
                    ViewData["TempsLivraison"] = reader.GetDecimal("Temps_Livraison_Total");
                    ViewData["DistanceTotale"] = reader.GetDecimal("Distance_Totale");
                    ViewData["NbPlats"] = reader.GetInt32("Nb_Plats_Livrés");
                }
            }
            #endregion

            #region NbClients
            using (var conn = new MySqlConnection(_config.GetConnectionString("MyDb")))
            {
                await conn.OpenAsync();

                var cmdNbClients = new MySqlCommand("SELECT COUNT(*) FROM Client_;", conn);
                var nbClients = Convert.ToInt32(await cmdNbClients.ExecuteScalarAsync());
                ViewData["NbClients"] = nbClients;
            }
            #endregion
            #endregion
            return Page();
        }

        /// <summary>
        /// Vide la table Station
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Renvoie un tuple lat lon depuis une adresse en entrée
        /// </summary>
        /// <param name="adresse"></param>
        /// <returns></returns>
        public async Task<(double latitude, double longitude)> GetCoordinatesFromAdresseAsync(string adresse)
        {
            if (string.IsNullOrWhiteSpace(adresse))
                return (0, 0);

            var (latitude, longitude) = await Convertisseur_coordonnees.GetCoordinatesAsync(adresse);
            return (latitude, longitude);
        }

        /// <summary>
        /// Rempli la table des stations depuis le fichier .mtx
        /// </summary>
        /// <returns></returns>
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
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Erreur d'importation : {ex.Message}";
            }

            return Page();
        }

        /// <summary>
        /// Vide toutes les classes nécessaires pour le rendu final
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostVoidBDD()
        {
            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                string[] tablesToClearInOrder = new[]
                {
                    "Plat_LigneCommande",
                    "Plat_du_jour",
                    "Plat",
                    "Panier",
                    "LigneCommande",
                    "Commande",
                    "Particulier",
                    "Entreprise",
                    "Client_",
                    "Cuisinier",
                    "Utilisateur"
                };

                foreach (var table in tablesToClearInOrder)
                {
                    var clearCmd = new MySqlCommand($"DELETE FROM {table};", conn, transaction);
                    clearCmd.ExecuteNonQuery();
                }

                var resetStatsCmd = new MySqlCommand(@"UPDATE Statistiques
                    SET 
                        Nb_Paniers_Validés = 0,
                        Nb_Commandes = 0,
                        Argent_Total = 0,
                        Temps_Livraison_Total = 0,
                        Distance_Totale = 0,
                        Nb_Plats_Livrés = 0,
                        Nb_Clients_Uniques = 0
                    WHERE Id_Statistiques = 3;", conn, transaction);

                resetStatsCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
            }

            return Page();
        }

        /// <summary>
        /// Instancie le graphe des stations
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostGenererGraphe()
        {
            var graphe = new Graphe();
            string connStr = _config.GetConnectionString("MyDb");
            graphe.ChargerDepuisBDD(connStr);

            return Page();
        }

        /// <summary>
        /// Cette méthode permet de récupérer une chaîne XML et de la désérialiser en un objet C#
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public T DeserializeFromXml<T>(string xml)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using var reader = new StringReader(xml);
            return (T)serializer.Deserialize(reader);
        }

        /// <summary>
        /// Cette méthode permet de récupérer une chaîne JSON et de la désérialiser en un objet C#
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public T DeserializeFromJson<T>(string json)
        {
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, options);
        }
        static void jsonSerializer_1()
        {
            string bd11 = "";
            string JsonString = JsonSerializer.Serialize(bd11);
            StreamWriter wr = new StreamWriter("/data"); 
            wr.WriteLine(JsonString);
            wr.Close();
        }

        /// <summary>
        /// Cette méthode permet de récupérer les informations des clients et des cuisiniers dans la base de donnée et de les convertir en un fichier XML
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostXmlSerializer_1()
        {
            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            List<Client<object>> clients = new List<Client<object>>();    
            var cmd = new MySqlCommand("SELECT Id_Client, Id_Utilisateur FROM Client_", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                
                int idClient = reader.GetInt32("Id_Client");
                int idUser = reader.GetInt32("Id_Utilisateur");

                
                var client = new Client<object>(idUser, idClient);
                clients.Add(client);
            }
            reader.Close();

            List<Cuisinier<object>> cuisiniers = new List<Cuisinier<object>>();
            var cmdCuisiniers = new MySqlCommand("SELECT Id_Cuisinier, Id_Utilisateur, Nom_particulier, Prenom_cuisinier FROM Cuisinier", conn);

            using var readerCuisiniers = cmdCuisiniers.ExecuteReader();
            while (readerCuisiniers.Read())
            {
                int idCuisinier = readerCuisiniers.GetInt32("Id_Cuisinier");
                int idUser = readerCuisiniers.GetInt32("Id_Utilisateur");
                string nom = readerCuisiniers.GetString("Nom_particulier");
                string prenom = readerCuisiniers.GetString("Prenom_cuisinier");

                var cuisinier = new Cuisinier<object>(idUser, idCuisinier, nom, prenom);
                cuisiniers.Add(cuisinier);
            }
            readerCuisiniers.Close();

            XmlSerializer serializer = new XmlSerializer(typeof(List<Client<object>>));

            
            using (StreamWriter wr = new StreamWriter("wwwroot/data/client.xml"))
            {
                serializer.Serialize(wr, clients);
            }

            XmlSerializer serializerCuisiniers = new XmlSerializer(typeof(List<Cuisinier<object>>));
            using (StreamWriter wr = new StreamWriter("wwwroot/data/cuisiniers.xml"))
            {
                serializerCuisiniers.Serialize(wr, cuisiniers);
            }

            return RedirectToPage();
        }


        /// <summary>
        /// Cette méthode permet de récupérer les informations des clients et des cuisiniers dans la base de donnée et de les convertir en un fichier Json
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostJsonSerializer_1()
        {
            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            List<Client<object>> clients = new List<Client<object>>();
            var cmdClients = new MySqlCommand("SELECT Id_Client, Id_Utilisateur FROM Client_", conn);

            using var readerClients = cmdClients.ExecuteReader();
            while (readerClients.Read())
            {
                int idClient = readerClients.GetInt32("Id_Client");
                int idUser = readerClients.GetInt32("Id_Utilisateur");

                var client = new Client<object>(idUser, idClient);
                clients.Add(client);
            }
            readerClients.Close();

            List<Cuisinier<object>> cuisiniers = new List<Cuisinier<object>>();
            var cmdCuisiniers = new MySqlCommand("SELECT Id_Cuisinier, Id_Utilisateur, Nom_particulier, Prenom_cuisinier FROM Cuisinier", conn);

            using var readerCuisiniers = cmdCuisiniers.ExecuteReader();
            while (readerCuisiniers.Read())
            {
                int idCuisinier = readerCuisiniers.GetInt32("Id_Cuisinier");
                int idUser = readerCuisiniers.GetInt32("Id_Utilisateur");
                string nom = readerCuisiniers.GetString("Nom_particulier");
                string prenom = readerCuisiniers.GetString("Prenom_cuisinier");

                var cuisinier = new Cuisinier<object>(idUser, idCuisinier, nom, prenom);
                cuisiniers.Add(cuisinier);
            }
            readerCuisiniers.Close();

            
            string clientsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "clients.json");
            string cuisiniersFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "cuisiniers.json");

            Directory.CreateDirectory(Path.GetDirectoryName(clientsFilePath));

            var options = new JsonSerializerOptions { WriteIndented = true };

            using (var writer = new StreamWriter(clientsFilePath))
            {
                string json = JsonSerializer.Serialize(clients, options);
                writer.Write(json);
            }

            using (var writer = new StreamWriter(cuisiniersFilePath))
            {
                string json = JsonSerializer.Serialize(cuisiniers, options);
                writer.Write(json);
            }

            return RedirectToPage();
        }
    }
}