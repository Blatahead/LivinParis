using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibrary;
using LivinParisWebApp.Utils;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace LivinParisWebApp.Pages.Client
{
    public class LivraisonClientModel : PageModel
    {
        private readonly IConfiguration _config;
        public LivraisonClientModel(IConfiguration config) => _config = config;

        public List<List<StationDTO>> Chemins { get; set; } = new();
        public List<StationDTO> ToutesStations { get; set; } = new();
        public List<ArcDTO> TousArcs { get; set; } = new();
        public List<double> DistancesKm { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var lignes = HttpContext.Session.GetObjectFromJson<List<LigneCommandeTemp>>("LignesCommandeTemp");
            var platsPanier = HttpContext.Session.GetObjectFromJson<List<int>>("PanierClient");
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (lignes == null || platsPanier == null || userId == 0)
                return RedirectToPage("/Client/DetailsCommande");

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            int idClient = 0;
            string adresseClient = "";
            var idCmd = new MySqlCommand("SELECT Id_Client FROM Client_ WHERE Id_Utilisateur = @uid", conn);
            idCmd.Parameters.AddWithValue("@uid", userId);
            var result = await idCmd.ExecuteScalarAsync();
            if (result != null)
            {
                idClient = Convert.ToInt32(result);
                adresseClient = await GetAdresseUtilisateur(conn, idClient);
            }

            var cmdPrix = new MySqlCommand($"SELECT Num_plat, Prix_plat FROM Plat WHERE Num_plat IN ({string.Join(",", platsPanier)})", conn);
            var prixMap = new Dictionary<int, decimal>();
            using var readerPrix = await cmdPrix.ExecuteReaderAsync();
            while (await readerPrix.ReadAsync())
                prixMap[readerPrix.GetInt32(0)] = readerPrix.GetDecimal(1);
            await readerPrix.CloseAsync();

            decimal prixTotal = lignes
                .SelectMany(l => l.Plats)
                .Where(id => prixMap.ContainsKey(id))
                .Sum(id => prixMap[id]);

            var insertCommande = new MySqlCommand("INSERT INTO Commande (Prix_commande, Id_Utilisateur) VALUES (@prix, @uid); SELECT LAST_INSERT_ID();", conn);
            insertCommande.Parameters.AddWithValue("@prix", prixTotal);
            insertCommande.Parameters.AddWithValue("@uid", userId);
            int idCommande = Convert.ToInt32(await insertCommande.ExecuteScalarAsync());

            var graphe = new Graphe();
            graphe.ChargerDepuisBDD(connStr);

            //var (latClient, lonClient) = await ClassLibrary.Convertisseur_coordonnees.GetCoordinatesAsync(adresseClient);
            //var stationClient = StationUtils.GetStationProche(latClient, lonClient, graphe.Stations);

            foreach (var ligne in lignes)
            {
                // Récupération adresse cuisinier à partir du 1er plat
                int? idPlat = ligne.Plats.FirstOrDefault();
                string adresseCuisinier = "";

                var getAddrCmd = new MySqlCommand(@"SELECT Adresse_cuisinier FROM Cuisinier 
                                        WHERE Id_Cuisinier = (SELECT Id_Cuisinier FROM Plat WHERE Num_plat = @idPlat)", conn);
                getAddrCmd.Parameters.AddWithValue("@idPlat", idPlat);
                var addrResult = await getAddrCmd.ExecuteScalarAsync();
                adresseCuisinier = addrResult?.ToString() ?? "Paris, France";

                // Coordonnées cuisinier
                var (latCuis, lonCuis) = await ClassLibrary.Convertisseur_coordonnees.GetCoordinatesAsync(adresseCuisinier);
                var stationCuis = StationUtils.GetStationProche(latCuis, lonCuis, graphe.Stations);

                // Coordonnées client pour CETTE ligne
                var adresseClientLigne = ligne.LieuLivraison;
                var (latClient, lonClient) = await ClassLibrary.Convertisseur_coordonnees.GetCoordinatesAsync(adresseClientLigne);
                var stationClient = StationUtils.GetStationProche(latClient, lonClient, graphe.Stations);

                // Calcul chemin
                var chemin = graphe.Dijkstra(stationCuis.Id, stationClient.Id);
                Chemins.Add(chemin.Select(StationConvertisseurs.ToDTO).ToList());

                // Calcul distance
                double distance = 0;
                for (int i = 0; i < chemin.Count - 1; i++)
                {
                    var s1 = chemin[i];
                    var s2 = chemin[i + 1];
                    distance += Station<StationNoeud>.CalculDistance2stations(s1,s2);
                }
                DistancesKm.Add(distance);

                // Insertion en BDD
                var insertLigne = new MySqlCommand(@"INSERT INTO LigneCommande (Id_Commande, DateLivraison, LieuLivraison) 
                                         VALUES (@idc, @date, @lieu); SELECT LAST_INSERT_ID();", conn);
                insertLigne.Parameters.AddWithValue("@idc", idCommande);
                insertLigne.Parameters.AddWithValue("@date", ligne.DateLivraison);
                insertLigne.Parameters.AddWithValue("@lieu", ligne.LieuLivraison);
                int idLigne = Convert.ToInt32(await insertLigne.ExecuteScalarAsync());

                foreach (var idPlatInLigne in ligne.Plats)
                {
                    var insertPlat = new MySqlCommand("INSERT INTO Plat_LigneCommande (Id_LigneCommande, Num_Plat) VALUES (@idL, @idP)", conn);
                    insertPlat.Parameters.AddWithValue("@idL", idLigne);
                    insertPlat.Parameters.AddWithValue("@idP", idPlatInLigne);
                    await insertPlat.ExecuteNonQueryAsync();
                }
            }


            var deletePanier = new MySqlCommand("DELETE FROM Panier WHERE Id_Client = @idClient", conn);
            deletePanier.Parameters.AddWithValue("@idClient", idClient);
            await deletePanier.ExecuteNonQueryAsync();

            ToutesStations = graphe.Stations.Select(StationConvertisseurs.ToDTO).ToList();
            TousArcs = graphe.Arcs.Select(arc => new ArcDTO
            {
                SourceId = arc.Source.Id,
                DestinationId = arc.Destination.Id,
                SourceLat = arc.Source.Latitude,
                SourceLong = arc.Source.Longitude,
                DestLat = arc.Destination.Latitude,
                DestLong = arc.Destination.Longitude,
                Ligne = arc.Ligne,
                Distance = arc.Distance
            }).ToList();

            return Page();
        }

        private async Task<string> GetAdresseUtilisateur(MySqlConnection conn, int idClient)
        {
            // Essai pour un particulier
            var cmdPart = new MySqlCommand("SELECT Adresse_particulier FROM Particulier WHERE Id_Client = @id", conn);
            cmdPart.Parameters.AddWithValue("@id", idClient);
            var partAddr = await cmdPart.ExecuteScalarAsync();
            if (partAddr != null)
            {
                string adresse = partAddr.ToString();
                if (!adresse.ToLower().Contains("paris"))
                    adresse += ", Paris, France";
                return adresse;
            }

            // Essai pour une entreprise
            var cmdEnt = new MySqlCommand("SELECT Adresse_entreprise FROM Entreprise WHERE Id_Client = @id", conn);
            cmdEnt.Parameters.AddWithValue("@id", idClient);
            var entAddr = await cmdEnt.ExecuteScalarAsync();
            if (entAddr != null)
            {
                string adresse = entAddr.ToString();
                if (!adresse.ToLower().Contains("paris"))
                    adresse += ", Paris, France";
                return adresse;
            }

            return "Paris, France"; // fallback
        }


        public IActionResult OnPostConfirm() => RedirectToPage("/ClientPanel");
        public IActionResult OnPostRetour() => RedirectToPage("/Client/DetailsCommande");
    }
}
