using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryRendu1;


namespace TestProjectRendu1
{

    [TestClass]
    public class GrapheTests
    {
        #region Attribus
        Graphe grapheConnexe;
        Graphe grapheNonConnexe;
        Graphe grapheAvecCycle;
        Graphe grapheSansCycle;
        #endregion

        /// <summary>
        /// Initialise tous les graphes a tester
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            // graphe connexe
            grapheConnexe = new Graphe();
            grapheConnexe.NouveauLien(1, 2);
            grapheConnexe.NouveauLien(2, 3);
            grapheConnexe.NouveauLien(3, 4);
            grapheConnexe.NouveauLien(4, 5);

            // graphe non connexe
            grapheNonConnexe = new Graphe();
            grapheNonConnexe.NouveauLien(1, 2);
            grapheNonConnexe.NouveauLien(2, 3);
            grapheNonConnexe.NouveauLien(4, 5);

            // Graphe avec cycle
            grapheAvecCycle = new Graphe();
            grapheAvecCycle.NouveauLien(1, 2);
            grapheAvecCycle.NouveauLien(2, 3);
            grapheAvecCycle.NouveauLien(3, 1);

            // Graphe sans cycle
            grapheSansCycle = new Graphe();
            grapheSansCycle.NouveauLien(1, 2);
            grapheSansCycle.NouveauLien(2, 3);
            grapheSansCycle.NouveauLien(3, 4);
        }

        /// <summary>
        /// Importation du graphe de l'association de karate
        /// Check des bons nombres de sommets et liens
        /// </summary>
        [TestMethod]
        public void TestLectureFichierMTX()
        {
            Graphe graphe = Graphe.LectureMTX("./../../../../ClassLibraryRendu1/soc-karate.mtx");

            Assert.AreEqual(34, graphe.Noeuds.Count);
            Assert.AreEqual(78, graphe.Liens.Count);
        }

        /// <summary>
        /// Teste la presence ou non d'un sommet dans le graphe importé
        /// </summary>
        [TestMethod]
        public void TestPresenceNoeudDansGraphe()
        {
            Graphe graphe = Graphe.LectureMTX("./../../../../ClassLibraryRendu1/soc-karate.mtx");

            Assert.IsTrue(graphe.Noeuds.Any(n => n.Id == 1));
            Assert.IsTrue(graphe.Noeuds.Any(n => n.Id == 2));
            Assert.IsFalse(graphe.Noeuds.Any(n => n.Id == 278));
        }

        /// <summary>
        /// Permet de tester si la matrice d'adjacence est correctement construite
        /// </summary>
        [TestMethod]
        public void TestMatriceAdjacence()
        {
            Graphe graphe = Graphe.LectureMTX("./../../../../ClassLibraryRendu1/soc-karate.mtx");

            int[,] matrice = graphe.MatriceAdjacence();

            Assert.AreEqual(1, matrice[0, 1]); // 1 et 2
            Assert.AreEqual(1, matrice[1, 0]); // 2 et 1
            Assert.AreEqual(0, matrice[2, 4]); // 3 et 5
        }

        /// <summary>
        /// Permet de tester si la liste d'adjacence est correctement construite
        /// </summary>
        [TestMethod]
        public void TestListeAdjacence()
        {
            Graphe graphe = Graphe.LectureMTX("./../../../../ClassLibraryRendu1/soc-karate.mtx");

            var listeAdjacence = graphe.ListeAdjacence();

            Assert.AreEqual(16, listeAdjacence[1].Count);
            Assert.AreNotEqual(2, listeAdjacence[2].Count);
            Assert.IsTrue(listeAdjacence[1].Contains(2));
            Assert.IsFalse(listeAdjacence[4].Contains(34));
        }

        /// <summary>
        /// Check si le graphe est bien connexe
        /// </summary>
        [TestMethod]
        public void TestEstConnexe_True()
        {
            bool resultat = grapheConnexe.EstConnexe();

            Assert.IsTrue(resultat, "Le graphe connexe devrait retourner true.");
        }


        /// <summary>
        /// Check si le graphe est bien non connexe
        /// </summary>
        [TestMethod]
        public void TestEstConnexe_False()
        {
            bool resultat = grapheNonConnexe.EstConnexe();

            Assert.IsFalse(resultat, "Le graphe non connexe devrait retourner false.");
        }

        /// <summary>
        /// Check si le graphe contient bien un cycle
        /// </summary>
        [TestMethod]
        public void TestContientCycle_True()
        {
            Assert.IsTrue(grapheAvecCycle.ContientCycle(), "Le graphe avec cycle devrait retourner true.");
        }

        /// <summary>
        /// Check si le graphe ne contient bien pas de cycle
        /// </summary>
        [TestMethod]
        public void TestContientCycle_False()
        {
            Assert.IsFalse(grapheSansCycle.ContientCycle(), "Le graphe sans cycle devrait retourner false.");
        }

        /// <summary>
        /// Permet de s'assurer du bon calcul de la taille d'un graphe
        /// </summary>
        [TestMethod]
        public void TailleGraphe_True()
        {
            Graphe graphe = Graphe.LectureMTX("./../../../../ClassLibraryRendu1/soc-karate.mtx");
            Assert.AreEqual(78,graphe.Liens.Count);
        }

        /// <summary>
        /// Permet de s'assurer du bon calcul de l'ordre d'un graphe
        /// </summary>
        [TestMethod]
        public void OrdreGraphe_True()
        {
            Graphe graphe = Graphe.LectureMTX("./../../../../ClassLibraryRendu1/soc-karate.mtx");
            Assert.AreEqual(34, graphe.Noeuds.Count);
        }
    }

    [TestClass]
    public class GeocodingHelperTests
    {
        [TestMethod]
        public async Task TestGetCoordinatesAsync()
        {
            
            string address = "15 rue de la Paix, Paris, 75002";

            
            var (latitude, longitude) = await Convertisseur_coordonnees.GetCoordinatesAsync(address);

            // Assert
            Assert.IsTrue(latitude > 48 && latitude < 49, "Latitude inattendue");//48.86935043334961
            Assert.IsTrue(longitude > 2 && longitude < 3, "Longitude inattendue");//2.3313136100769043
        }
    }
}