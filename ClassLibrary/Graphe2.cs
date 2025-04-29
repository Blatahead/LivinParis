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


        public void ChargerDepuisBDD2(string connectionString)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            var clients = new List<Noeud>();
            var commandes = new List<(Noeud, string, string)>(); 
            var plats = new List<Noeud>();
            var cuisiniers = new List<(Noeud, string, string)>();
            var particuliers = new List<(string id, string adresse)>();
            var entreprises = new List<(string id, string adresse)>();




            var cmdClients = new MySqlCommand("SELECT Id_Client, Id_Utilisateur FROM Client_", conn);
            using var readerClients = cmdClients.ExecuteReader();
            while (readerClients.Read())
            {
                string idClient = readerClients.GetInt32(0).ToString();
                string idUser = readerClients.GetInt32(1).ToString();
                var noeudClient = new Noeud(idClient, "Client");
                AjouterNoeud(noeudClient);
                clients.Add(noeudClient); 
            }
            readerClients.Close();

            var cmdParticuliers = new MySqlCommand("SELECT Id_Client, Adresse_particulier FROM Particulier", conn);

            var cmdEntreprises = new MySqlCommand("SELECT id_Client, Adresse_entreprise FROM Entreprise", conn);




            var cmdCommandes = new MySqlCommand("SELECT Num_commande, liste_plats, Id_Utilisateur FROM Commande", conn);
            using var readerCommandes = cmdCommandes.ExecuteReader();
            while (readerCommandes.Read())
            {
                string numeroCommande = readerCommandes.GetInt32(0).ToString();
                string listePlats = readerCommandes.IsDBNull(1) ? "" : readerCommandes.GetString(1);
                string idUser = readerCommandes.GetInt32(2).ToString();

                var noeudCommande = new Noeud(numeroCommande, "Commande");
                AjouterNoeud(noeudCommande);
                commandes.Add((noeudCommande, listePlats, idUser));
            }
            readerCommandes.Close();

            
            var cmdPlats = new MySqlCommand("SELECT Num_plat FROM Plat", conn);
            using var readerPlats = cmdPlats.ExecuteReader();
            while (readerPlats.Read())
            {
                string numPlat = readerPlats.GetInt32(0).ToString();
                var noeudPlat = new Noeud(numPlat, "Plat");
                AjouterNoeud(noeudPlat);
                plats.Add(noeudPlat);
            }
            readerPlats.Close();


            var cmdCuisiniers = new MySqlCommand("SELECT Id_Cuisinier, Id_Utilisateur, Adresse_cuisinier FROM Cuisinier", conn);
            using var readerCuisiniers = cmdCuisiniers.ExecuteReader();
            while (readerCuisiniers.Read())
            {
                string idCuisinier = readerCuisiniers.GetInt32(0).ToString();
                string idUser = readerCuisiniers.GetInt32(1).ToString();
                string adresseCuisinier = readerCuisiniers.GetString(2);

                var noeudCuisinier = new Noeud(idCuisinier, "Cuisinier");
                AjouterNoeud(noeudCuisinier);
                cuisiniers.Add((noeudCuisinier, idUser, adresseCuisinier));
            }

            readerCuisiniers.Close();

            
            foreach (var (noeudCommande, _, idUserCommande) in commandes)
            {
                var client = clients.FirstOrDefault(c => c.Id == idUserCommande);
                if (client != null)
                {
                    AjouterLien(client, noeudCommande, "a commandé");
                }
            }

            
            foreach (var (noeudCommande, listePlats, _) in commandes)
            {
                if (!string.IsNullOrEmpty(listePlats))
                {
                    var platsIds = listePlats.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var platId in platsIds)
                    {
                        var plat = plats.FirstOrDefault(p => p.Id == platId.Trim());
                        if (plat != null)
                        {
                            AjouterLien(noeudCommande, plat, "contient");
                        }
                    }
                }
            }


            foreach (var (noeudCuisinier, idUserCuisinier, adresseCuisinier) in cuisiniers)
            {
                foreach (var plat in plats)
                {
                    AjouterLien(noeudCuisinier, plat, "prépare");
                }
            }

         
            foreach (var client in clients)
            {
                foreach (var (cuisinier, idUserCuisinier, adresseCuisinier) in cuisiniers)
                {
                    if (client.Id == idUserCuisinier)
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

