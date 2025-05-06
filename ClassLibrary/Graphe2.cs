using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClassLibrary
{
    public class Graphe2
    {
        #region Attributs
        public List<Noeud> Noeuds = new();
        public List<Lien> Liens = new();
        #endregion

        #region Methodes
        /// <summary>
        /// Ajoute un noeud à la liste de noeuds
        /// </summary>
        /// <param name="n"></param>
        public void AjouterNoeud(Noeud n) => Noeuds.Add(n);

        /// <summary>
        /// Ajoute un lien à la liste de liens
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="libelle"></param>
        public void AjouterLien(Noeud n1, Noeud n2, string libelle)
        {
            Liens.Add(new Lien(n1, n2, libelle));
        }

        /// <summary>
        /// Cette méthode permet de charger les informations dans la base de données à partir de requêtes SQL
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public async Task ChargerDepuisBDD2(string connectionString)
        {
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();

            var clients = new List<Noeud>();
            var commandes = new List<(Noeud, int)>();
            var plats = new List<(Noeud noeud, int numPlatId)>();

            var cuisiniers = new List<(Noeud, string, string)>();

            var adressesUtilisateurs = new Dictionary<string, (double lat, double lon)>();
            var userToClient = new Dictionary<string, string>();
            var cmdPart = new MySqlCommand("SELECT Id_Client, Adresse_particulier FROM Particulier", conn);
            using var readerPart = await cmdPart.ExecuteReaderAsync();
            while (await readerPart.ReadAsync())
            {
                string idClient = readerPart.GetInt32(0).ToString();
                string adresse = readerPart.GetString(1);
                var (lat, lon) = await Convertisseur_coordonnees.GetCoordinatesAsync(adresse);
                adressesUtilisateurs[idClient] = (lat, lon);
            }
            await readerPart.CloseAsync();
            var cmdEnt = new MySqlCommand("SELECT id_Client, Adresse_entreprise FROM Entreprise", conn);
            using var readerEnt = await cmdEnt.ExecuteReaderAsync();
            while (await readerEnt.ReadAsync())
            {
                string idClient = readerEnt.GetInt32(0).ToString();
                string adresse = readerEnt.GetString(1);
                var (lat, lon) = await Convertisseur_coordonnees.GetCoordinatesAsync(adresse);
                adressesUtilisateurs[idClient] = (lat, lon);
            }
            await readerEnt.CloseAsync();
            var cmdLien = new MySqlCommand("SELECT Id_Client, Id_Utilisateur FROM Client_", conn);
            using var readerLien = await cmdLien.ExecuteReaderAsync();
            while (await readerLien.ReadAsync())
            {
                string idClient = readerLien.GetInt32(0).ToString();
                string idUser = readerLien.GetInt32(1).ToString();
                userToClient[idUser] = idClient;

                var (lat, lon) = adressesUtilisateurs.GetValueOrDefault(idClient, (48.8566, 2.3522));
                var noeudClient = new Noeud($"Client:{idClient}", "Client", lat, lon);
                AjouterNoeud(noeudClient);
                clients.Add(noeudClient);
            }
            await readerLien.CloseAsync();
            var cmdCommandes = new MySqlCommand("SELECT Num_commande, Id_Utilisateur FROM Commande", conn);
            using var readerCommandes = await cmdCommandes.ExecuteReaderAsync();
            while (await readerCommandes.ReadAsync())
            {
                int numCommande = readerCommandes.GetInt32(0);
                string idUser = readerCommandes.GetInt32(1).ToString();

                string idClient = userToClient.GetValueOrDefault(idUser, "0");
                var (lat, lon) = adressesUtilisateurs.GetValueOrDefault(idClient, (48.8566, 2.3522));

                var noeudCommande = new Noeud($"Commande:{numCommande}", "Commande", lat, lon);
                AjouterNoeud(noeudCommande);
                commandes.Add((noeudCommande, numCommande));
            }
            await readerCommandes.CloseAsync();
            var cmdPlats = new MySqlCommand(@"SELECT p.Num_plat, c.Adresse_cuisinier
                FROM Plat p
                JOIN Cuisinier c ON p.id_Cuisinier = c.Id_Cuisinier", conn);

            using var readerPlats = await cmdPlats.ExecuteReaderAsync();
            while (await readerPlats.ReadAsync())
            {
                int numPlat = readerPlats.GetInt32(0);
                string numPlatStr = numPlat.ToString();
                string adresseCuisinier = readerPlats.GetString(1);
                var (lat, lon) = await Convertisseur_coordonnees.GetCoordinatesAsync(adresseCuisinier);
                var noeudPlat = new Noeud($"Plat:{numPlatStr}", "Plat", lat, lon);
                AjouterNoeud(noeudPlat);
                plats.Add((noeudPlat, numPlat));
            }
            await readerPlats.CloseAsync();
            var cmdCuisiniers = new MySqlCommand("SELECT Id_Cuisinier, Id_Utilisateur, Adresse_cuisinier FROM Cuisinier", conn);
            using var readerCuisiniers = await cmdCuisiniers.ExecuteReaderAsync();
            while (await readerCuisiniers.ReadAsync())
            {
                string idCuisinier = readerCuisiniers.GetInt32(0).ToString();
                string idUser = readerCuisiniers.GetInt32(1).ToString();
                string adresse = readerCuisiniers.GetString(2);
                var (lat, lon) = await Convertisseur_coordonnees.GetCoordinatesAsync(adresse);
                var noeudCuisinier = new Noeud($"Cuisinier:{idCuisinier}", "Cuisinier", lat, lon);
                AjouterNoeud(noeudCuisinier);
                cuisiniers.Add((noeudCuisinier, idUser, adresse));
            }
            await readerCuisiniers.CloseAsync();
            var lignesCommande = new List<(int Id_LigneCommande, int Id_Commande)>();

            var cmdLignes = new MySqlCommand("SELECT Id_LigneCommande, Id_Commande FROM LigneCommande", conn);
            using var readerLignes = await cmdLignes.ExecuteReaderAsync();
            while (await readerLignes.ReadAsync())
            {
                lignesCommande.Add((
                    readerLignes.GetInt32(0),
                    readerLignes.GetInt32(1)
                ));
            }
            await readerLignes.CloseAsync();
            var platLigneCommande = new List<(int Id_LigneCommande, int Num_Plat)>();

            var cmdPLC = new MySqlCommand("SELECT Id_LigneCommande, Num_Plat FROM Plat_LigneCommande", conn);
            using var readerPLC = await cmdPLC.ExecuteReaderAsync();
            while (await readerPLC.ReadAsync())
            {
                platLigneCommande.Add((
                    readerPLC.GetInt32(0),
                    readerPLC.GetInt32(1)
                ));
            }
            await readerPLC.CloseAsync();
            foreach (var (noeudPlat, numPlatId) in plats)
            {
                var cmd = new MySqlCommand($"SELECT id_Cuisinier FROM Plat WHERE Num_plat = {numPlatId}", conn);
                var idCuisinier = (await cmd.ExecuteScalarAsync())?.ToString();

                var noeudCuisinier = cuisiniers
                    .FirstOrDefault(c => c.Item1.Id == $"Cuisinier:{idCuisinier}").Item1;

                if (noeudCuisinier != null)
                {
                    AjouterLien(noeudCuisinier, noeudPlat, "prépare");
                }
            }
            foreach (var (noeudCommande, numCommande) in commandes)
            {
                var lignes = lignesCommande.Where(l => l.Id_Commande == numCommande);

                foreach (var ligne in lignes)
                {
                    var platIds = platLigneCommande
                        .Where(plc => plc.Item1 == ligne.Id_LigneCommande)
                        .Select(plc => plc.Item2)
                        .ToList();

                    var platsAssocies = plats
                        .Where(p => platIds.Contains(p.numPlatId))
                        .Select(p => p.noeud);

                    foreach (var plat in platsAssocies)
                    {
                        AjouterLien(noeudCommande, plat, "contient");
                    }
                }
            }
            foreach (var (noeudCommande, numCommande) in commandes)
            {
                var cmd = new MySqlCommand($"SELECT Id_Utilisateur FROM Commande WHERE Num_commande = {numCommande}", conn);
                var idUserCommande = (await cmd.ExecuteScalarAsync())?.ToString();

                if (idUserCommande != null && userToClient.TryGetValue(idUserCommande, out string idClient))
                {
                    var noeudClient = clients.FirstOrDefault(c => c.Id == $"Client:{idClient}");
                    if (noeudClient != null)
                    {
                        AjouterLien(noeudClient, noeudCommande, "a commandé");
                    }
                }
            }
            foreach (var client in clients)
            {
                foreach (var (cuisinier, idUserCuisinier, _) in cuisiniers)
                {
                    if (userToClient.TryGetValue(idUserCuisinier, out string idClient)
                        && client.Id == $"Client:{idClient}")
                    {
                        AjouterLien(client, cuisinier, "est aussi cuisinier");
                    }
                }
            }
        }

        /// <summary>
        /// Construit le graphe 2 avec noeuds, arcs
        /// </summary>
        /// <param name="cuisiniers"></param>
        /// <param name="particuliers"></param>
        /// <param name="entreprises"></param>
        /// <param name="commandes"></param>
        /// <returns></returns>
        public async Task<(List<Noeud> noeuds, List<Lien> liens)> BuildGraphAsync(
            List<Cuisinier<object>> cuisiniers,
            List<Particulier<object>> particuliers,
            List<Entreprise<object>> entreprises,
            List<Commande<object>> commandes)
        {
            var noeuds = new List<Noeud>();
            var liens = new List<Lien>();

            var noeudDict = new Dictionary<string, Noeud>();
            foreach (var cuisiner in cuisiniers)
            {
                var noeud = new Noeud($"Cuisinier:{cuisiner.Id_Cuisinier}", "Cuisinier");
                noeuds.Add(noeud);
                noeudDict[noeud.Id] = noeud;
            }
            foreach (var particulier in particuliers)
            {
                var noeud = new Noeud($"Particulier:{particulier.IdParticulier}", "Particulier");
                noeuds.Add(noeud);
                noeudDict[noeud.Id] = noeud;
            }
            foreach (var entreprise in entreprises)
            {
                var noeud = new Noeud($"Entreprise:{entreprise.NumeroSiret}", "Entreprise");
                noeuds.Add(noeud);
                noeudDict[noeud.Id] = noeud;
            }
            foreach (var commande in commandes)
            {
                string clientId;

                if (particuliers.Any(p => p.IdParticulier == commande.IdUser))
                {
                    clientId = $"Particulier:{commande.IdUser}";
                }
                else if (entreprises.Any(e => e.NumeroSiret == commande.IdUser))
                {
                    clientId = $"Entreprise:{commande.IdUser}";
                }
                else
                {
                    continue;
                }

                if (noeudDict.TryGetValue($"Cuisinier:{commande.Id_Cuisinier}", out var cuisinierNoeud) &&
                    noeudDict.TryGetValue(clientId, out var clientNoeud))
                {
                    liens.Add(new Lien(cuisinierNoeud, clientNoeud, "Commande"));
                }
            }

            return (noeuds, liens);
        }
        #endregion
    }
}