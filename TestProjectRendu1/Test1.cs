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
        private Graphe grapheConnexe;
        private Graphe grapheNonConnexe;
        private Graphe grapheAvecCycle;
        private Graphe grapheSansCycle;

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

            // Graphe AVEC cycle
            grapheAvecCycle = new Graphe();
            grapheAvecCycle.NouveauLien(1, 2);
            grapheAvecCycle.NouveauLien(2, 3);
            grapheAvecCycle.NouveauLien(3, 1); // Cycle !

            // Graphe SANS cycle
            grapheSansCycle = new Graphe();
            grapheSansCycle.NouveauLien(1, 2);
            grapheSansCycle.NouveauLien(2, 3);
            grapheSansCycle.NouveauLien(3, 4);
        }

        [TestMethod]
        public void TestLectureFichierMTX()
        {
            Graphe graphe = Graphe.LectureMTX("./../../../../ClassLibraryRendu1/soc-karate.mtx");

            Assert.AreEqual(34, graphe.Noeuds.Count);
            Assert.AreEqual(78, graphe.Liens.Count);
        }

        [TestMethod]
        public void TestAjouterLien()
        {
            Graphe graphe = Graphe.LectureMTX("./../../../../ClassLibraryRendu1/soc-karate.mtx");

            Assert.IsTrue(graphe.Noeuds.Any(n => n.Id == 1));
            Assert.IsTrue(graphe.Noeuds.Any(n => n.Id == 2));
            Assert.IsFalse(graphe.Noeuds.Any(n => n.Id == 278));
        }

        [TestMethod]
        public void TestMatriceAdjacence()
        {
            Graphe graphe = Graphe.LectureMTX("./../../../../ClassLibraryRendu1/soc-karate.mtx");

            int[,] matrice = graphe.MatriceAdjacence();

            Assert.AreEqual(1, matrice[0, 1]);
            Assert.AreEqual(1, matrice[1, 0]);
            Assert.AreEqual(0, matrice[2, 4]); // 3 et 5
        }

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

        [TestMethod]
        public void TestEstConnexe_True()
        {
            bool resultat = grapheConnexe.EstConnexe();

            Assert.IsTrue(resultat, "Le graphe connexe devrait retourner true.");
        }

        [TestMethod]
        public void TestEstConnexe_False()
        {
            bool resultat = grapheNonConnexe.EstConnexe();

            Assert.IsFalse(resultat, "Le graphe non connexe devrait retourner false.");
        }

        [TestMethod]
        public void TestContientCycle_True()
        {
            Assert.IsTrue(grapheAvecCycle.ContientCycle(), "Le graphe avec cycle devrait retourner true.");
        }

        [TestMethod]
        public void TestContientCycle_False()
        {
            Assert.IsFalse(grapheSansCycle.ContientCycle(), "Le graphe sans cycle devrait retourner false.");
        }
    }
}