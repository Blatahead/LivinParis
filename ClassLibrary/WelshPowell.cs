using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class WelshPowell
{
    #region Methodes
    /// <summary>
    /// Cette méthode permet de colorier chaque sommet en fonction de la couleur des sommets adjacents
    /// </summary>
    /// <param name="graphe"></param>
    /// <returns></returns>
    public static Dictionary<Noeud, int> ColorierGraphe(Graphe2 graphe)
    {
        var couleurs = new Dictionary<Noeud, int>();
        var degres = graphe.Noeuds.ToDictionary(
            n => n,
            n => graphe.Liens.Count(l => l.Noeud1 == n || l.Noeud2 == n)
        );
        var noeudsTries = degres.OrderByDescending(paire => paire.Value).Select(paire => paire.Key).ToList();
        foreach (var noeud in noeudsTries)
        {
            var voisins = GetVoisins(noeud, graphe);
            var couleursVoisines = voisins
                .Where(voisin => couleurs.ContainsKey(voisin))
                .Select(voisin => couleurs[voisin])
                .ToHashSet();
            int couleur = 1;
            while (couleursVoisines.Contains(couleur))
            {
                couleur++;
            }
            couleurs[noeud] = couleur;
        }

        return couleurs;
    }

    /// <summary>
    /// Cette méthode retourne les noeuds voisins
    /// </summary>
    /// <param name="n"></param>
    /// <param name="g"></param>
    /// <returns></returns>
    private static List<Noeud> GetVoisins(Noeud n, Graphe2 g)
    {
        return g.Liens
            .Where(l => l.Noeud1 == n || l.Noeud2 == n)
            .Select(l => l.Noeud1 == n ? l.Noeud2 : l.Noeud1)
            .Distinct()
            .ToList();
    }
    #endregion
}