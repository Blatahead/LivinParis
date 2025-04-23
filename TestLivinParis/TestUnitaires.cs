namespace ClassLibrary
{
    [TestClass]
    public sealed class TestUnitaires
    {
        [TestMethod]
        public void Graphe_Triangle()
        {

            var g = new Graphe2();
            var a = new Noeud("A", "Test");
            var b = new Noeud("B", "Test");
            var c = new Noeud("C", "Test");

            g.AjouterNoeud(a);
            g.AjouterNoeud(b);
            g.AjouterNoeud(c);

            g.AjouterLien(a, b, "x");
            g.AjouterLien(b, c, "x");
            g.AjouterLien(c, a, "x");


            var couleurs = WelshPowell.ColorierGraphe(g);
            var nbCouleurs = couleurs.Values.Distinct().Count();


            Assert.AreEqual(3, nbCouleurs);
        }

        [TestMethod]
        public void Graphe_Ligne()
        {

            var g = new Graphe2();
            var a = new Noeud("A", "Test");
            var b = new Noeud("B", "Test");
            var c = new Noeud("C", "Test");
            var d = new Noeud("D", "Test");

            g.AjouterNoeud(a);
            g.AjouterNoeud(b);
            g.AjouterNoeud(c);
            g.AjouterNoeud(d);

            g.AjouterLien(a, b, "x");
            g.AjouterLien(b, c, "x");
            g.AjouterLien(c, d, "x");


            var couleurs = WelshPowell.ColorierGraphe(g);
            var nbCouleurs = couleurs.Values.Distinct().Count();


            Assert.AreEqual(2, nbCouleurs);
        }

        [TestClass]
        public class BellmanFordTests
        {
            public class GrapheTest
            {
                public Dictionary<int, double> BellmanFord(int idDepart, List<Station<object>> stations)
                {
                    var distances = new Dictionary<int, double>();

                    // Initialisation des distances
                    foreach (var station in stations)
                    {
                        distances[station.Depart] = double.PositiveInfinity;
                        distances[station.Arrivee] = double.PositiveInfinity;
                    }
                    distances[idDepart] = 0;

                    int nombreStations = distances.Count;

                    // Détente des arêtes
                    for (int i = 0; i < nombreStations - 1; i++)
                    {
                        foreach (var station in stations)
                        {
                            double poids = station.Distance;
                            if (distances[station.Depart] + poids < distances[station.Arrivee])
                            {
                                distances[station.Arrivee] = distances[station.Depart] + poids;
                            }
                        }
                    }

                    // Détection des cycles de poids négatif
                    foreach (var station in stations)
                    {
                        double poids = station.Distance;
                        if (distances[station.Depart] + poids < distances[station.Arrivee])
                        {
                            throw new Exception("Le graphe contient un cycle de poids négatif.");
                        }
                    }

                    return distances;
                }
            }

            [TestMethod]
            public void BellmanFord_CalculDistanceCorrecte()
            {
                var algo = new GrapheTest();
                var stations = new List<Station<object>>()
        {
            new Station<object>(1, "A", 0, 0, 1, 75000, 0, 1, 2, 0, 2),
            new Station<object>(2, "B", 0, 0, 1, 75000, 0, 2, 3, 0, 3)
        };

                var result = algo.BellmanFord(1, stations);

                Assert.AreEqual(0, result[1]);
                Assert.AreEqual(2, result[2]);
                Assert.AreEqual(5, result[3]);
            }
        }
    }


    [TestClass]
    public class AStarTests
    {
        [TestMethod]
        public void AStar_Trouve_Chemin_Correct()
        {
            // Création du graphe
            var graphe = new Graphe();

            // Ajout des stations (id, nom, lat, lon, ligne)
            var s1 = new StationNoeud(1, "A", 0, 0, "L1");
            var s2 = new StationNoeud(2, "B", 0, 1, "L1");
            var s3 = new StationNoeud(3, "C", 1, 1, "L1");
            var s4 = new StationNoeud(4, "D", 1, 2, "L1");

            graphe.AjouterStation(s1);
            graphe.AjouterStation(s2);
            graphe.AjouterStation(s3);
            graphe.AjouterStation(s4);

            // Connexions (poids = distance)
            graphe.AjouterArc(1, 2, 1, "L1");
            graphe.AjouterArc(2, 3, 1, "L1");
            graphe.AjouterArc(3, 4, 1, "L1");

            // Lancement de A*
            var aStar = new AStar();
            var chemin = aStar.TrouverChemin(graphe, 1, 4);

            // Validation
            Assert.AreEqual(4, chemin.Count);
            Assert.AreEqual("A", chemin[0].Nom);
            Assert.AreEqual("D", chemin[3].Nom);
        }
    }

}
