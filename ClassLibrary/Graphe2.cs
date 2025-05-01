using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Graphe2
    {
        public List<Noeud> Noeuds = new();
        public List<Lien> Liens = new();

        public void AjouterNoeud(Noeud n) => Noeuds.Add(n);

        public void AjouterLien(Noeud n1, Noeud n2, string libelle)
        {
            Liens.Add(new Lien(n1, n2, libelle));
        }

        public void Afficher()
        {
            Console.WriteLine("=== Noeuds ===");
            foreach (var n in Noeuds)
                Console.WriteLine(n);

            Console.WriteLine("\n=== Liens (non orientés) ===");
            foreach (var l in Liens)
                Console.WriteLine(l);
        }

        public void ColorierEtAfficher()
        {
            var couleurs = WelshPowell.ColorierGraphe(this);
            var nbCouleurs = couleurs.Values.Distinct().Count();

            Console.WriteLine("\n=== Coloration des nœuds (Welsh-Powell) ===");
            foreach (var kvp in couleurs.OrderBy(kvp => kvp.Value))
            {
                Console.WriteLine($"{kvp.Key} -> Couleur {kvp.Value}");
            }

            Console.WriteLine($"\nNombre total de couleurs utilisées : {nbCouleurs}");
        }


        public async Task ChargerDepuisBDD2(string connectionString)
        {
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();

            var clients = new List<Noeud>();
            var commandes = new List<(Noeud, string, string)>();
            var plats = new List<Noeud>();
            var cuisiniers = new List<(Noeud, string, string)>();

            var adressesUtilisateurs = new Dictionary<string, (double lat, double lon)>();
            var userToClient = new Dictionary<string, string>();

            // Particuliers
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

            // Entreprises
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

            // Lien Utilisateur -> Client
            var cmdLien = new MySqlCommand("SELECT Id_Client, Id_Utilisateur FROM Client_", conn);
            using var readerLien = await cmdLien.ExecuteReaderAsync();
            while (await readerLien.ReadAsync())
            {
                string idClient = readerLien.GetInt32(0).ToString();
                string idUser = readerLien.GetInt32(1).ToString();
                userToClient[idUser] = idClient;

                var (lat, lon) = adressesUtilisateurs.GetValueOrDefault(idClient, (48.8566, 2.3522));
                var noeudClient = new Noeud(idClient, "Client", lat, lon);
                AjouterNoeud(noeudClient);
                clients.Add(noeudClient);
            }
            await readerLien.CloseAsync();

            // Commandes
            //var cmdCommandes = new MySqlCommand("SELECT Num_commande, liste_plats, Id_Utilisateur FROM Commande", conn);
            //using var readerCommandes = await cmdCommandes.ExecuteReaderAsync();
            //while (await readerCommandes.ReadAsync())
            //{
            //    string numCommande = readerCommandes.GetInt32(0).ToString();
            //    string listePlats = readerCommandes.IsDBNull(1) ? "" : readerCommandes.GetString(1);
            //    string idUser = readerCommandes.GetInt32(2).ToString();

            //    string idClient = userToClient.GetValueOrDefault(idUser, "0");
            //    var (lat, lon) = adressesUtilisateurs.GetValueOrDefault(idClient, (48.8566, 2.3522));

            //    var noeudCommande = new Noeud(numCommande, "Commande", lat, lon);
            //    AjouterNoeud(noeudCommande);
            //    commandes.Add((noeudCommande, listePlats, idUser));
            //}
            //await readerCommandes.CloseAsync();

            // Plats avec coordonnées du cuisinier associé
            var cmdPlats = new MySqlCommand(@"SELECT p.Num_plat, c.Adresse_cuisinier
                FROM Plat p
                JOIN Cuisinier c ON p.id_Cuisinier = c.Id_Cuisinier
                WHERE p.Disponible = 1", conn);

            using var readerPlats = await cmdPlats.ExecuteReaderAsync();
            while (await readerPlats.ReadAsync())
            {
                string numPlat = readerPlats.GetInt32(0).ToString();
                string adresseCuisinier = readerPlats.GetString(1);

                var (lat, lon) = await Convertisseur_coordonnees.GetCoordinatesAsync(adresseCuisinier);

                var noeudPlat = new Noeud(numPlat, "Plat", lat, lon);
                AjouterNoeud(noeudPlat);
                plats.Add(noeudPlat);
            }
            await readerPlats.CloseAsync();


            // Cuisiniers
            var cmdCuisiniers = new MySqlCommand("SELECT Id_Cuisinier, Id_Utilisateur, Adresse_cuisinier FROM Cuisinier", conn);
            using var readerCuisiniers = await cmdCuisiniers.ExecuteReaderAsync();
            while (await readerCuisiniers.ReadAsync())
            {
                string idCuisinier = readerCuisiniers.GetInt32(0).ToString();
                string idUser = readerCuisiniers.GetInt32(1).ToString();
                string adresse = readerCuisiniers.GetString(2);
                var (lat, lon) = await Convertisseur_coordonnees.GetCoordinatesAsync(adresse);
                var noeudCuisinier = new Noeud(idCuisinier, "Cuisinier", lat, lon);
                AjouterNoeud(noeudCuisinier);
                cuisiniers.Add((noeudCuisinier, idUser, adresse));
            }
            await readerCuisiniers.CloseAsync();

            // Liaisons : client -> commande
            foreach (var (noeudCommande, _, idUserCommande) in commandes)
            {
                if (userToClient.TryGetValue(idUserCommande, out string idClient))
                {
                    var client = clients.FirstOrDefault(c => c.Id == idClient);
                    if (client != null)
                    {
                        AjouterLien(client, noeudCommande, "a commandé");
                    }
                }
            }

            // Liaisons : commande -> plats
            //foreach (var (noeudCommande, listePlats, _) in commandes)
            //{
            //    if (!string.IsNullOrEmpty(listePlats))
            //    {
            //        var platsIds = listePlats.Split(',', StringSplitOptions.RemoveEmptyEntries);
            //        foreach (var platId in platsIds)
            //        {
            //            var plat = plats.FirstOrDefault(p => p.Id == platId.Trim());
            //            if (plat != null)
            //            {
            //                AjouterLien(noeudCommande, plat, "contient");
            //            }
            //        }
            //    }
            //}

            // Liaisons : cuisinier -> plats
            foreach (var (noeudCuisinier, _, _) in cuisiniers)
            {
                foreach (var plat in plats)
                {
                    AjouterLien(noeudCuisinier, plat, "prépare");
                }
            }

            // Liaisons : client est aussi cuisinier
            foreach (var client in clients)
            {
                foreach (var (cuisinier, idUserCuisinier, _) in cuisiniers)
                {
                    if (userToClient.TryGetValue(idUserCuisinier, out string idClient) && client.Id == idClient)
                    {
                        AjouterLien(client, cuisinier, "est aussi cuisinier");
                    }
                }
            }
        }

        public async Task<(List<Noeud> noeuds, List<Lien> liens)> BuildGraphAsync(
            List<Cuisinier<object>> cuisiniers,
            List<Particulier<object>> particuliers,
            List<Entreprise<object>> entreprises,
            List<Commande<object>> commandes)
        {
            var noeuds = new List<Noeud>();
            var liens = new List<Lien>();

            var noeudDict = new Dictionary<string, Noeud>();

            // Ajout des cuisiniers
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
    }
}