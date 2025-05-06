using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using MySql.Data.MySqlClient;

namespace LivinParisWebApp.Pages.Cuisinier
{
    public class LivraisonCuisinierModel : PageModel
    {
        #region Attribut
        private readonly IConfiguration _config;
        #endregion

        #region Constructeur
        public LivraisonCuisinierModel(IConfiguration config)
        {
            _config = config;
        }
        #endregion

        #region Proprietes
        public List<StationDTO> Chemin { get; set; } = new();
        public List<string> StationsTraversees { get; set; } = new();
        public double DistanceKm { get; set; }
        public decimal PrixLigne { get; set; }
        public List<string> LignesTraversees { get; set; } = new();
        public int NbStations { get; set; }
        public string CheminJson { get; set; }
        #endregion

        #region Methodes
        /// <summary>
        /// au lancement de la page
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync()
        {
            if (!TempData.TryGetValue("IdLigneCommande", out var obj) || obj is not int idLigneCommande)
                return RedirectToPage("/Cuisinier/SeeCurrentCommand");

            string connStr = _config.GetConnectionString("MyDb");
            var graphe = new Graphe();
            graphe.ChargerDepuisBDD(connStr);

            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            var getAdressesCmd = new MySqlCommand(@"
            SELECT cu.Adresse_cuisinier, lc.LieuLivraison
            FROM LigneCommande lc
            JOIN Plat_LigneCommande plc ON lc.Id_LigneCommande = plc.Id_LigneCommande
            JOIN Plat p ON p.Num_plat = plc.Num_Plat
            JOIN Cuisinier cu ON cu.Id_Cuisinier = p.Id_Cuisinier
            WHERE lc.Id_LigneCommande = @id LIMIT 1", conn);
            getAdressesCmd.Parameters.AddWithValue("@id", idLigneCommande);

            string adresseCuisinier = "Paris";
            string adresseClient = "Paris";

            using var reader = await getAdressesCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                adresseCuisinier = reader.GetString(0);
                adresseClient = reader.GetString(1);
            }
            await reader.CloseAsync();

            var (latCuis, lonCuis) = await Convertisseur_coordonnees.GetCoordinatesAsync(adresseCuisinier);
            var (latClient, lonClient) = await Convertisseur_coordonnees.GetCoordinatesAsync(adresseClient);

            var stationDep = StationUtils.GetStationProche(latCuis, lonCuis, graphe.Stations);
            var stationArr = StationUtils.GetStationProche(latClient, lonClient, graphe.Stations);

            var cheminNoeuds = graphe.Dijkstra(stationDep.Id, stationArr.Id);
            Chemin = cheminNoeuds.Select(StationConvertisseurs.ToDTO).ToList();
            StationsTraversees = cheminNoeuds.Select(s => s.Nom).ToList();
            LignesTraversees = new List<string>();
            for (int i = 0; i < cheminNoeuds.Count - 1; i++)
            {
                var arc = graphe.Arcs.FirstOrDefault(a =>
                    (a.Source.Id == cheminNoeuds[i].Id && a.Destination.Id == cheminNoeuds[i + 1].Id) ||
                    (a.Source.Id == cheminNoeuds[i + 1].Id && a.Destination.Id == cheminNoeuds[i].Id));

                if (arc != null && !string.IsNullOrEmpty(arc.Ligne))
                    LignesTraversees.Add(arc.Ligne);
            }

            for (int i = 0; i < cheminNoeuds.Count - 1; i++)
                DistanceKm += Station<StationNoeud>.CalculDistance2stations(cheminNoeuds[i], cheminNoeuds[i + 1]);

            var getPrixCmd = new MySqlCommand(@"
            SELECT SUM(p.Prix_plat)
            FROM Plat p
            JOIN Plat_LigneCommande plc ON p.Num_plat = plc.Num_Plat
            WHERE plc.Id_LigneCommande = @id", conn);
            getPrixCmd.Parameters.AddWithValue("@id", idLigneCommande);
            var prixObj = await getPrixCmd.ExecuteScalarAsync();
            PrixLigne = prixObj != null ? Convert.ToDecimal(prixObj) : 0;

            var stationDTOs = graphe.Stations.Select(st => new StationDTO
            {
                Id = st.Id,
                Nom = st.Nom,
                Latitude = st.Latitude,
                Longitude = st.Longitude
            }).ToList();

            var arcDTOs = graphe.Arcs.Select(arc => new ArcDTO
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

            ViewData["Stations"] = JsonConvert.SerializeObject(stationDTOs);
            ViewData["Arcs"] = JsonConvert.SerializeObject(arcDTOs);

            return Page();
        }

        /// <summary>
        /// retour
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostBack()
        {
            return RedirectToPage("/Cuisinier/SeeCurrentCommand");
        }

        /// <summary>
        /// confirmation de la livraison
        /// </summary>
        /// <param name="idLigneCommande"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostConfirm(int idLigneCommande)
        {
            string connStr = _config.GetConnectionString("MyDb");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            var cmdInfo = new MySqlCommand(@"
        SELECT 
            SUM(p.Prix_plat), 
            COUNT(p.Num_plat),
            lc.LieuLivraison,
            u.Id_Utilisateur
        FROM Plat p
        JOIN Plat_LigneCommande plc ON p.Num_plat = plc.Num_Plat
        JOIN LigneCommande lc ON lc.Id_LigneCommande = plc.Id_LigneCommande
        JOIN Commande c ON lc.Id_Commande = c.Num_Commande
        JOIN Utilisateur u ON u.Id_Utilisateur = c.Id_Utilisateur
        WHERE lc.Id_LigneCommande = @id", conn);
            cmdInfo.Parameters.AddWithValue("@id", idLigneCommande);

            decimal prixTotal = 0;
            int nbPlats = 0;
            string lieuLivraison = "";
            int idUtilisateur = 0;

            using var reader = await cmdInfo.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                prixTotal = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                nbPlats = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                lieuLivraison = reader.GetString(2);
                idUtilisateur = reader.GetInt32(3);
            }
            await reader.CloseAsync();

            var getAdresseCmd = new MySqlCommand(@"
        SELECT cu.Adresse_cuisinier
        FROM Plat p
        JOIN Plat_LigneCommande plc ON p.Num_plat = plc.Num_Plat
        JOIN Cuisinier cu ON cu.Id_Cuisinier = p.Id_Cuisinier
        WHERE plc.Id_LigneCommande = @id LIMIT 1", conn);
            getAdresseCmd.Parameters.AddWithValue("@id", idLigneCommande);
            string adresseCuisinier = (await getAdresseCmd.ExecuteScalarAsync())?.ToString() ?? "Paris";

            var (latCuis, lonCuis) = await Convertisseur_coordonnees.GetCoordinatesAsync(adresseCuisinier);
            var (latClient, lonClient) = await Convertisseur_coordonnees.GetCoordinatesAsync(lieuLivraison);

            var graphe = new Graphe();
            graphe.ChargerDepuisBDD(connStr);

            var stationDep = StationUtils.GetStationProche(latCuis, lonCuis, graphe.Stations);
            var stationArr = StationUtils.GetStationProche(latClient, lonClient, graphe.Stations);

            var chemin = graphe.Dijkstra(stationDep.Id, stationArr.Id);
            double distanceKm = 0;
            for (int i = 0; i < chemin.Count - 1; i++)
                distanceKm += Station<StationNoeud>.CalculDistance2stations(chemin[i], chemin[i + 1]);

            double tempsMinutes = distanceKm / 20 * 60;

            var updateCmd = new MySqlCommand(@"
        UPDATE Statistiques SET
            Nb_Paniers_Validés = Nb_Paniers_Validés + 1,
            Argent_Total = Argent_Total + @prix,
            Temps_Livraison_Total = Temps_Livraison_Total + @temps,
            Distance_Totale = Distance_Totale + @distance,
            Nb_Plats_Livrés = Nb_Plats_Livrés + @nbplats,
            Nb_Clients_Uniques = Nb_Clients_Uniques + @ajoutClient
        WHERE Id_Statistiques = 3", conn);
            updateCmd.Parameters.AddWithValue("@prix", prixTotal);
            updateCmd.Parameters.AddWithValue("@temps", tempsMinutes);
            updateCmd.Parameters.AddWithValue("@distance", distanceKm);
            updateCmd.Parameters.AddWithValue("@nbplats", nbPlats);
            updateCmd.Parameters.AddWithValue("@ajoutClient", 1);
            await updateCmd.ExecuteNonQueryAsync();

            var getIdCuisinierCmd = new MySqlCommand(@"
        SELECT cu.Id_Cuisinier
        FROM Cuisinier cu
        JOIN Plat p ON cu.Id_Cuisinier = p.Id_Cuisinier
        JOIN Plat_LigneCommande plc ON p.Num_plat = plc.Num_Plat
        WHERE plc.Id_LigneCommande = @idLigne
        LIMIT 1", conn);
            getIdCuisinierCmd.Parameters.AddWithValue("@idLigne", idLigneCommande);
            int idCuisinier = Convert.ToInt32(await getIdCuisinierCmd.ExecuteScalarAsync());

            var cleanCmd = new MySqlCommand(@"
        UPDATE Cuisinier
        SET Liste_commandes_pretes = TRIM(BOTH ',' FROM REPLACE(CONCAT(',', Liste_commandes_pretes, ','), CONCAT(',', @id, ','), ','))
        WHERE Id_Cuisinier = @idCuisinier", conn);
            cleanCmd.Parameters.AddWithValue("@id", idLigneCommande);
            cleanCmd.Parameters.AddWithValue("@idCuisinier", idCuisinier);
            await cleanCmd.ExecuteNonQueryAsync();

            var updateLivreesCmd = new MySqlCommand(@"
            UPDATE Cuisinier 
            SET Liste_commandes_livrees = 
                CASE 
                    WHEN Liste_commandes_livrees = '' OR Liste_commandes_livrees IS NULL THEN @id
                    ELSE CONCAT(Liste_commandes_livrees, ',', @id)
                END
            WHERE Id_Cuisinier = @idCuisinier", conn);

            updateLivreesCmd.Parameters.AddWithValue("@id", idLigneCommande);
            updateLivreesCmd.Parameters.AddWithValue("@idCuisinier", idCuisinier);
            await updateLivreesCmd.ExecuteNonQueryAsync();

            var getRevenusEtClientsCmd = new MySqlCommand(@"
    SELECT Revenus_totaux, Clients_servis
    FROM Cuisinier
    WHERE Id_Cuisinier = @idCuisinier", conn);
            getRevenusEtClientsCmd.Parameters.AddWithValue("@idCuisinier", idCuisinier);

            decimal revenusActuels = 0;
            string clientsActuels = "";

            using var revenusEtClientsReader = await getRevenusEtClientsCmd.ExecuteReaderAsync();
            if (await revenusEtClientsReader.ReadAsync())
            {
                revenusActuels = revenusEtClientsReader.IsDBNull(0) ? 0 : revenusEtClientsReader.GetDecimal(0);
                clientsActuels = revenusEtClientsReader.IsDBNull(1) ? "" : revenusEtClientsReader.GetString(1);
            }
            await revenusEtClientsReader.CloseAsync();

            decimal nouveauxRevenus = revenusActuels + prixTotal;

            string nomClientFinal = "";

            var getNomClientCmd = new MySqlCommand(@"
                SELECT 
                    p.Prenom_particulier, p.Nom_particulier,
                    e.Nom_référent, e.Nom_entreprise
                FROM Client_ c
                LEFT JOIN Particulier p ON c.Id_Client = p.Id_Client
                LEFT JOIN Entreprise e ON c.Id_Client = e.Id_Client
                WHERE c.Id_Utilisateur = @uid", conn);
            getNomClientCmd.Parameters.AddWithValue("@uid", idUtilisateur);

            using var clientReader = await getNomClientCmd.ExecuteReaderAsync();
            if (await clientReader.ReadAsync())
            {
                if (clientReader["Prenom_particulier"] != DBNull.Value && clientReader["Nom_particulier"] != DBNull.Value)
                {
                    nomClientFinal = $"{clientReader["Prenom_particulier"]} {clientReader["Nom_particulier"]}";
                }
                else if (clientReader["Nom_référent"] != DBNull.Value && clientReader["Nom_entreprise"] != DBNull.Value)
                {
                    nomClientFinal = $"{clientReader["Nom_référent"]} ({clientReader["Nom_entreprise"]})";
                }
            }
            await clientReader.CloseAsync();

            List<string> listeClients = clientsActuels.Split(',').Select(c => c.Trim()).ToList();
            if (!listeClients.Contains(nomClientFinal))
            {
                listeClients.Add(nomClientFinal);
            }
            string nouvelleListeClients = string.Join(", ", listeClients);

            var updateRevenusEtClientsCmd = new MySqlCommand(@"
                UPDATE Cuisinier
                SET Revenus_totaux = @revenusTotaux,
                    Clients_servis = @clientsServis
                WHERE Id_Cuisinier = @idCuisinier", conn);
            updateRevenusEtClientsCmd.Parameters.AddWithValue("@revenusTotaux", nouveauxRevenus);
            updateRevenusEtClientsCmd.Parameters.AddWithValue("@clientsServis", nouvelleListeClients);
            updateRevenusEtClientsCmd.Parameters.AddWithValue("@idCuisinier", idCuisinier);
            await updateRevenusEtClientsCmd.ExecuteNonQueryAsync();

            return RedirectToPage("/CuisinierPanel");
        }
        #endregion
    }
}