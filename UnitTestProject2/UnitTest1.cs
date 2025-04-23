using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ClassLibrary
{
    [TestClass]
    public class WelshPowellTests
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
    }
}
