using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class AStar
    {
        private double Heuristique(StationNoeud a, StationNoeud b)
        {
            // Heuristique basée sur la distance euclidienne entre deux points (latitude/longitude)
            double dx = a.Longitude - b.Longitude;
            double dy = a.Latitude - b.Latitude;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public List<StationNoeud> TrouverChemin(Graphe graphe, int idDepart, int idArrivee)
        {
            var stations = graphe.Stations.ToDictionary(s => s.Id);
            var ouvert = new SortedSet<(double fScore, int idStation)>(new DistanceComparer());
            var gScore = new Dictionary<int, double>();
            var fScore = new Dictionary<int, double>();
            var precedent = new Dictionary<int, int?>();

            foreach (var s in graphe.Stations)
            {
                gScore[s.Id] = double.PositiveInfinity;
                fScore[s.Id] = double.PositiveInfinity;
                precedent[s.Id] = null;
            }

            gScore[idDepart] = 0;
            fScore[idDepart] = Heuristique(stations[idDepart], stations[idArrivee]);
            ouvert.Add((fScore[idDepart], idDepart));

            while (ouvert.Count > 0)
            {
                var (currentF, currentId) = ouvert.Min;
                ouvert.Remove(ouvert.Min);

                if (currentId == idArrivee)
                    return ReconstituerChemin(precedent, stations, idArrivee);

                var currentStation = stations[currentId];

                foreach (var arc in currentStation.ArcsSortants)
                {
                    int voisinId = arc.Destination.Id;
                    double tentativeG = gScore[currentId] + arc.Distance;

                    if (tentativeG < gScore[voisinId])
                    {
                        precedent[voisinId] = currentId;
                        gScore[voisinId] = tentativeG;
                        fScore[voisinId] = tentativeG + Heuristique(arc.Destination, stations[idArrivee]);

                        // Retirer l’ancienne entrée (si présente) et ajouter la nouvelle
                        ouvert.RemoveWhere(t => t.idStation == voisinId);
                        ouvert.Add((fScore[voisinId], voisinId));
                    }
                }
            }

            return new List<StationNoeud>(); // Aucun chemin trouvé
        }

        private List<StationNoeud> ReconstituerChemin(Dictionary<int, int?> precedent, Dictionary<int, StationNoeud> stations, int idArrivee)
        {
            var chemin = new List<StationNoeud>();
            int? courant = idArrivee;

            while (courant != null)
            {
                chemin.Insert(0, stations[courant.Value]);
                courant = precedent[courant.Value];
            }

            return chemin;
        }

        // Comparateur pour le tri dans le SortedSet
        private class DistanceComparer : IComparer<(double fScore, int id)>
        {
            public int Compare((double fScore, int id) x, (double fScore, int id) y)
            {
                int compareF = x.fScore.CompareTo(y.fScore);
                return compareF != 0 ? compareF : x.id.CompareTo(y.id);
            }
        }
    }
}
