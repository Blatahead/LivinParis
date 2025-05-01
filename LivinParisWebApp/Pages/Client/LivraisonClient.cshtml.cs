using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibrary;
using LivinParisWebApp.Utils;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using ClassLibraryRendu1;

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
        public List<List<string>> StationsTraversees { get; set; } = new();
        public List<decimal> PrixParLigne { get; set; } = new();
        public decimal PrixTotalCommande { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var lignes = HttpContext.Session.GetObjectFromJson<List<LigneCommandeTemp>>("LignesCommandeTemp");
            var platsPanier = HttpContext.Session.GetObjectFromJson<List<int>>("PanierClient");
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (lignes == null || platsPanier == null || userId == 0)
                return RedirectToPage("/Client/DetailsCommande");

            string connStr = _config.GetConnectionString("MyDb");
            var graphe = new ClassLibrary.Graphe();
            graphe.ChargerDepuisBDD(connStr);

            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            // Récupération des prix des plats
            var cmdPrix = new MySqlCommand($"SELECT Num_plat, Prix_plat FROM Plat WHERE Num_plat IN ({string.Join(",", platsPanier)})", conn);
            var prixMap = new Dictionary<int, decimal>();
            using var readerPrix = await cmdPrix.ExecuteReaderAsync();
            while (await readerPrix.ReadAsync())
                prixMap[readerPrix.GetInt32(0)] = readerPrix.GetDecimal(1);
            await readerPrix.CloseAsync();

            foreach (var ligne in lignes)
            {
                int idPlat = ligne.Plats.FirstOrDefault();
                string adresseCuisinier = await GetAdresseCuisinierAsync(conn, idPlat);
                var (latCuis, lonCuis) = await ClassLibrary.Convertisseur_coordonnees.GetCoordinatesAsync(adresseCuisinier);
                var stationCuis = StationUtils.GetStationProche(latCuis, lonCuis, graphe.Stations);

                var (latClient, lonClient) = await ClassLibrary.Convertisseur_coordonnees.GetCoordinatesAsync(ligne.LieuLivraison);
                var stationClient = StationUtils.GetStationProche(latClient, lonClient, graphe.Stations);

                var chemin = graphe.Dijkstra(stationCuis.Id, stationClient.Id);
                Chemins.Add(chemin.Select(StationConvertisseurs.ToDTO).ToList());
                StationsTraversees.Add(chemin.Select(s => s.Nom).ToList());

                double distance = 0;
                for (int i = 0; i < chemin.Count - 1; i++)
                    distance += Station<StationNoeud>.CalculDistance2stations(chemin[i], chemin[i + 1]);
                DistancesKm.Add(distance);

                decimal prixLigne = ligne.Plats.Where(id => prixMap.ContainsKey(id)).Sum(id => prixMap[id]);
                PrixParLigne.Add(prixLigne);
            }

            PrixTotalCommande = PrixParLigne.Sum();
            HttpContext.Session.SetObject("DistancesKm", DistancesKm);
            HttpContext.Session.SetObject("PrixParLigne", PrixParLigne);
            HttpContext.Session.SetObject("StationsTraversees", StationsTraversees);

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

        public async Task<IActionResult> OnPostConfirm()
        {
            var lignes = HttpContext.Session.GetObjectFromJson<List<LigneCommandeTemp>>("LignesCommandeTemp");
            var platsPanier = HttpContext.Session.GetObjectFromJson<List<int>>("PanierClient");
            var distances = HttpContext.Session.GetObjectFromJson<List<double>>("DistancesKm");
            var prixLignes = HttpContext.Session.GetObjectFromJson<List<decimal>>("PrixParLigne");
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (lignes == null || platsPanier == null || distances == null || prixLignes == null || userId == 0)
                return RedirectToPage("/ClientPanel");

            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            // 1. Exécuter l'insertion
            var insertCommande = new MySqlCommand("INSERT INTO Commande (Prix_commande, Id_Utilisateur) VALUES (@prix, @uid)", conn);
            insertCommande.Parameters.AddWithValue("@prix", prixLignes.Sum());
            insertCommande.Parameters.AddWithValue("@uid", userId);
            await insertCommande.ExecuteNonQueryAsync();

            // 2. Récupérer le dernier ID généré
            var getLastIdCmd = new MySqlCommand("SELECT LAST_INSERT_ID();", conn);
            int idCommande = Convert.ToInt32(await getLastIdCmd.ExecuteScalarAsync());


            for (int i = 0; i < lignes.Count; i++)
            {
                var ligne = lignes[i];
                var insertLigne = new MySqlCommand("INSERT INTO LigneCommande (Id_Commande, DateLivraison, LieuLivraison) VALUES (@idc, @date, @lieu); SELECT LAST_INSERT_ID();", conn);
                insertLigne.Parameters.AddWithValue("@idc", idCommande);
                insertLigne.Parameters.AddWithValue("@date", ligne.DateLivraison);
                insertLigne.Parameters.AddWithValue("@lieu", ligne.LieuLivraison);
                int idLigne = Convert.ToInt32(await insertLigne.ExecuteScalarAsync());

                foreach (var idPlat in ligne.Plats)
                {
                    var insertPlat = new MySqlCommand("INSERT INTO Plat_LigneCommande (Id_LigneCommande, Num_Plat) VALUES (@idL, @idP)", conn);
                    insertPlat.Parameters.AddWithValue("@idL", idLigne);
                    insertPlat.Parameters.AddWithValue("@idP", idPlat);
                    await insertPlat.ExecuteNonQueryAsync();

                    // add le plat à la liste de commande du cuisinier
                    var updateCmd = new MySqlCommand("UPDATE Cuisinier SET Liste_commandes = CONCAT_WS(',', Liste_commandes, @idC) WHERE Id_Cuisinier = (SELECT Id_Cuisinier FROM Plat WHERE Num_plat = @idP)", conn);
                    updateCmd.Parameters.AddWithValue("@idC", idCommande);
                    updateCmd.Parameters.AddWithValue("@idP", idPlat);
                    await updateCmd.ExecuteNonQueryAsync();

                    var disablePlatCmd = new MySqlCommand("UPDATE Plat SET Disponible = FALSE WHERE Num_plat = @idPlat", conn);
                    disablePlatCmd.Parameters.AddWithValue("@idPlat", idPlat);
                    await disablePlatCmd.ExecuteNonQueryAsync();
                }
            }

            var idClientCmd = new MySqlCommand("SELECT Id_Client FROM Client_ WHERE Id_Utilisateur = @uid", conn);
            idClientCmd.Parameters.AddWithValue("@uid", userId);
            int idClient = Convert.ToInt32(await idClientCmd.ExecuteScalarAsync());

            var deletePanier = new MySqlCommand("DELETE FROM Panier WHERE Id_Client = @idClient", conn);
            deletePanier.Parameters.AddWithValue("@idClient", idClient);
            await deletePanier.ExecuteNonQueryAsync();

            var updateStats = new MySqlCommand(@"UPDATE Statistiques SET
                Nb_Paniers_Validés = Nb_Paniers_Validés + 1,
                Nb_Commandes = Nb_Commandes + 1,
                Argent_Total = Argent_Total + @total,
                Temps_Livraison_Total = Temps_Livraison_Total + @temps,
                Distance_Totale = Distance_Totale + @distance,
                Nb_Plats_Livrés = Nb_Plats_Livrés + @nbplats
                WHERE Id_Statistiques = 1;", conn);

            updateStats.Parameters.AddWithValue("@total", prixLignes.Sum());
            updateStats.Parameters.AddWithValue("@temps", distances.Sum(d => d / 20 * 60));
            updateStats.Parameters.AddWithValue("@distance", distances.Sum());
            updateStats.Parameters.AddWithValue("@nbplats", platsPanier.Count);
            await updateStats.ExecuteNonQueryAsync();

            HttpContext.Session.Remove("PanierClient");
            HttpContext.Session.Remove("LignesCommandeTemp");
            HttpContext.Session.Remove("DistancesKm");
            HttpContext.Session.Remove("PrixParLigne");
            HttpContext.Session.Remove("StationsTraversees");

            return RedirectToPage("/ClientPanel");
        }

        public IActionResult OnPostRetour() => RedirectToPage("/Client/DetailsCommande");

        private async Task<string> GetAdresseCuisinierAsync(MySqlConnection conn, int idPlat)
        {
            var cmd = new MySqlCommand(@"SELECT Adresse_cuisinier FROM Cuisinier 
                                         WHERE Id_Cuisinier = (SELECT Id_Cuisinier FROM Plat WHERE Num_plat = @id)", conn);
            cmd.Parameters.AddWithValue("@id", idPlat);
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() ?? "Paris, France";
        }
    }
}