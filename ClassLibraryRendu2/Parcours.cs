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
        public Dictionary<int, double> Dijkstra(int idDepart, List<Station<object>> stations)
        {
            var distances = new Dictionary<int, double>();
            var precedent = new Dictionary<int, int?>();
            var file = new SortedSet<SommetPrioritaire>();

            foreach (var station in stations)
            {
                distances[station.IdentifiantStation] = double.PositiveInfinity;
                precedent[station.IdentifiantStation] = null;
            }

            distances[idDepart] = 0;
            file.Add(new SommetPrioritaire(idDepart, 0));

            while (file.Count > 0)
            {
                var actuel = file.Min;
                file.Remove(actuel);

                var stationActuelle = stations.First(s => s.IdentifiantStation == actuel.Id);

                foreach (var voisin in stations.Where(s => s.Depart == stationActuelle.IdentifiantStation))
                {
                    if (voisin.Sens == 1 && voisin.Arrivee < stationActuelle.IdentifiantStation)
                        continue; // Respecter le sens de circulation

                    double tentativeDistance = distances[actuel.Id] + voisin.Distance;

                    if (tentativeDistance < distances[voisin.Arrivee])
                    {
                        file.RemoveWhere(x => x.Id == voisin.Arrivee);
                        distances[voisin.Arrivee] = tentativeDistance;
                        precedent[voisin.Arrivee] = actuel.Id;
                        file.Add(new SommetPrioritaire(voisin.Arrivee, tentativeDistance));
                    }
                }
            }

            return distances;
        }



        /// <summary>
        /// Squelette de l'algorithme de Bellman-Ford pour gérer des graphes avec poids négatifs
        /// </summary>
        public Dictionary<int, double> BellmanFord(int idDepart, List<Station<object>> stations)
        {
            var distances = new Dictionary<int, double>();

            foreach (var station in stations)
            {
                distances[station.IdentifiantStation] = double.PositiveInfinity;
            }
            distances[idDepart] = 0;

            int nombreStations = stations.Count;

            for (int i = 0; i < nombreStations - 1; i++)
            {
                foreach (var station in stations)
                {
                    foreach (var voisin in stations.Where(s => s.Depart == station.IdentifiantStation))
                    {
                        double poids = voisin.Distance; // Utilisation de la distance réelle
                        if (distances[station.IdentifiantStation] + poids < distances[voisin.Arrivee])
                        {
                            distances[voisin.Arrivee] = distances[station.IdentifiantStation] + poids;
                        }
                    }
                }
            }

            // Vérification des cycles de poids négatif
            foreach (var station in stations)
            {
                foreach (var voisin in stations.Where(s => s.Depart == station.IdentifiantStation))
                {
                    double poids = voisin.Distance;
                    if (distances[station.IdentifiantStation] + poids < distances[voisin.Arrivee])
                    {
                        throw new Exception("Le graphe contient un cycle de poids négatif.");
                    }
                }
            }

            return distances;
        }

    }
}
