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
    }
}