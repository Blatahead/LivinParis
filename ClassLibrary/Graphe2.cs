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

    }
}
