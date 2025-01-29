using ClassLibraryRendu1;
using System;

namespace ClassLibraryRendu1
{
    [TestClass]
    public class GrapheTests
    {
        Graphe graphe;

        [TestInitialize]
        public void Setup()
        {
            this.graphe = new Graphe();
        }

        [TestMethod]
        public void TestLectureFichierMTX()
        {
            // Arrange : créer un fichier temporaire avec des données fictives
            string cheminFichier = "test.mtx";
            File.WriteAllLines(cheminFichier, new string[]
            {
                "%%MatrixMarket matrix coordinate pattern symmetric",
                "% Example MTX file",
                "4 4 3",
                "1 2",
                "2 3",
                "3 4"
            });

            // Act : Lecture du fichier
            Graphe grapheLu = Graphe.LireFichierMTX(cheminFichier);

            // Assert : Vérification du nombre de nœuds et de liens
            Assert.AreEqual(4, grapheLu.Noeuds.Count);
            Assert.AreEqual(3, grapheLu.Liens.Count);

            // Nettoyage
            File.Delete(cheminFichier);
        }

        [TestMethod]
        public void TestAjouterLien()
        {
            // Act
            graphe.AjouterLien(1, 2);
            graphe.AjouterLien(2, 3);
            graphe.AjouterLien(3, 4);

            // Assert
            Assert.AreEqual(3, graphe.Liens.Count);
            Assert.IsTrue(graphe.Noeuds.Any(n => n.Id == 1));
            Assert.IsTrue(graphe.Noeuds.Any(n => n.Id == 2));
            Assert.IsTrue(graphe.Noeuds.Any(n => n.Id == 3));
            Assert.IsTrue(graphe.Noeuds.Any(n => n.Id == 4));
        }

        [TestMethod]
        public void TestMatriceAdjacence()
        {
            // Arrange
            graphe.AjouterLien(1, 2);
            graphe.AjouterLien(2, 3);
            graphe.AjouterLien(3, 4);

            // Act
            int[,] matrice = graphe.GenererMatriceAdjacence();

            // Assert
            Assert.AreEqual(1, matrice[0, 1]); // (1,2)
            Assert.AreEqual(1, matrice[1, 0]); // Symétrique (2,1)
            Assert.AreEqual(1, matrice[1, 2]); // (2,3)
            Assert.AreEqual(1, matrice[2, 1]); // (3,2)
            Assert.AreEqual(1, matrice[2, 3]); // (3,4)
            Assert.AreEqual(1, matrice[3, 2]); // (4,3)
            Assert.AreEqual(0, matrice[0, 2]); // Pas de lien entre 1 et 3
        }

        [TestMethod]
        public void TestListeAdjacence()
        {
            // Arrange
            graphe.AjouterLien(1, 2);
            graphe.AjouterLien(2, 3);
            graphe.AjouterLien(3, 4);

            // Act
            var listeAdjacence = graphe.GenererListeAdjacence();

            // Assert
            Assert.AreEqual(1, listeAdjacence[1].Count);
            Assert.AreEqual(2, listeAdjacence[2].Count);
            Assert.AreEqual(2, listeAdjacence[3].Count);
            Assert.AreEqual(1, listeAdjacence[4].Count);
            Assert.IsTrue(listeAdjacence[1].Contains(2));
            Assert.IsTrue(listeAdjacence[2].Contains(1));
            Assert.IsTrue(listeAdjacence[2].Contains(3));
            Assert.IsTrue(listeAdjacence[3].Contains(2));
            Assert.IsTrue(listeAdjacence[3].Contains(4));
            Assert.IsTrue(listeAdjacence[4].Contains(3));
        }
    }

}
