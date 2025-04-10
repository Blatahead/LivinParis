using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class WelshPowell
    { 
        public int ColorierGraphe(List<StationNoeud> sommets)
        {
            // Étape 1 : Calculer le degré de chaque sommet
            var sommetsParDegré = sommets
                .OrderByDescending(s => s.ArcsSortants.Select(a => a.Destination.Id).Distinct().Count())
                .ToList();

            // Dictionnaire pour stocker la couleur attribuée à chaque sommet
            var couleurs = new Dictionary<int, int>();

            int couleurActuelle = 1;

            foreach (var sommet in sommetsParDegré)
            {
                // Si ce sommet est déjà colorié, on passe
                if (couleurs.ContainsKey(sommet.Id))
                    continue;

                // On colore le sommet courant avec la couleur actuelle
                couleurs[sommet.Id] = couleurActuelle;

                // Parcours des autres sommets non colorés
                foreach (var autre in sommetsParDegré)
                {
                    if (couleurs.ContainsKey(autre.Id))
                        continue;

                    // Vérifie si ce sommet est adjacent à un sommet déjà colorié avec la couleur actuelle
                    bool adjacentAvecMemeCouleur = autre.ArcsSortants
                        .Select(a => a.Destination.Id)
                        .Any(voisinId => couleurs.ContainsKey(voisinId) && couleurs[voisinId] == couleurActuelle);

                    if (!adjacentAvecMemeCouleur)
                    {
                        couleurs[autre.Id] = couleurActuelle;
                    }
                }

                couleurActuelle++; // Incrémente la couleur pour la prochaine passe
            }

            // Retourne le nombre total de couleurs utilisées
            return couleurs.Values.Distinct().Count();
        }



    }
}
