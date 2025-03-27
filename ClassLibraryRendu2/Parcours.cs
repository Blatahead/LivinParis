using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibraryRendu1; // Pour Graphe, Noeud, Lien

namespace ClassLibraryRendu2
{
    public class SommetPrioritaire : IComparable<SommetPrioritaire>
    {
        public int Id { get; set; }
        public double Distance { get; set; }

        public SommetPrioritaire(int id, double distance)
        {
            Id = id;
            Distance = distance;
        }

        public int CompareTo(SommetPrioritaire other)
        {
            int result = Distance.CompareTo(other.Distance);
            return result != 0 ? result : Id.CompareTo(other.Id);
        }
    }

    public class Parcours
    {
        private Graphe graphe;

        public Parcours(Graphe graphe)
        {
            this.graphe = graphe;
        }

        /// <summary>
        /// Algorithme de Dijkstra pour calculer le plus court chemin
        /// </summary>
        public Dictionary<int, double> Dijkstra(int idDepart)
        {
            var distances = new Dictionary<int, double>();
            var precedent = new Dictionary<int, int?>();
            var file = new SortedSet<SommetPrioritaire>();

            foreach (var noeud in graphe.Noeuds)
            {
                distances[noeud.Id] = double.PositiveInfinity;
                precedent[noeud.Id] = null;
            }

            distances[idDepart] = 0;
            file.Add(new SommetPrioritaire(idDepart, 0));

            while (file.Count > 0)
            {
                var actuel = file.Min;
                file.Remove(actuel);

                var noeudActuel = graphe.Noeuds.First(n => n.Id == actuel.Id);

                foreach (var voisin in noeudActuel.Voisins)
                {
                    double poids = 1; // À remplacer par le poids réel entre actuel.Id et voisin.Id
                    double tentativeDistance = distances[actuel.Id] + poids;

                    if (tentativeDistance < distances[voisin.Id])
                    {
                        file.RemoveWhere(x => x.Id == voisin.Id);
                        distances[voisin.Id] = tentativeDistance;
                        precedent[voisin.Id] = actuel.Id;
                        file.Add(new SommetPrioritaire(voisin.Id, tentativeDistance));
                    }
                }
            }

            return distances;
        }

        /// <summary>
        /// Squelette de l'algorithme de Bellman-Ford pour gérer des graphes avec poids négatifs
        /// </summary>
        public Dictionary<int, double> BellmanFord(int idDepart)
        {
            var distances = new Dictionary<int, double>();

            foreach (var noeud in graphe.Noeuds)
            {
                distances[noeud.Id] = double.PositiveInfinity;
            }
            distances[idDepart] = 0;

            int nombreNoeuds = graphe.Noeuds.Count;

            for (int i = 0; i < nombreNoeuds - 1; i++)
            {
                foreach (var lien in graphe.Liens)
                {
                    double poids = 1; // À remplacer par le poids réel entre Source et Destination
                    if (distances[lien.Source.Id] + poids < distances[lien.Destination.Id])
                    {
                        distances[lien.Destination.Id] = distances[lien.Source.Id] + poids;
                    }
                }
            }

            // Vérification des cycles de poids négatif
            foreach (var lien in graphe.Liens)
            {
                double poids = 1; // Même remarque ici
                if (distances[lien.Source.Id] + poids < distances[lien.Destination.Id])
                {
                    throw new Exception("Le graphe contient un cycle de poids négatif.");
                }
            }

            return distances;
        }
    }
}
