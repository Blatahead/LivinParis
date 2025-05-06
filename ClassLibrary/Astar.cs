using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class AStar
    {
        private double DistanceEuclid(StationNoeud a, StationNoeud b)
        {
            double distX = a.Longitude - b.Longitude;
            double distY = a.Latitude - b.Latitude;
            return Math.Sqrt(distX * distX + distY * distY);
        }

        public List<StationNoeud> TrouverChemin(Graphe graphe, int idDepart, int idArrivee)
        {
            var stations = graphe.Stations.ToDictionary(s => s.Id);
            var ouvert = new SortedSet<(double estimPassantParN, int idStation)>(new DistanceComparer());
            var distAccumuleeReelle = new Dictionary<int, double>();
            var estimPassantParN = new Dictionary<int, double>();
            var precedent = new Dictionary<int, int?>();

            foreach (var s in graphe.Stations)
            {
                distAccumuleeReelle[s.Id] = double.PositiveInfinity;
                estimPassantParN[s.Id] = double.PositiveInfinity;
                precedent[s.Id] = null;
            }

            distAccumuleeReelle[idDepart] = 0;
            estimPassantParN[idDepart] = DistanceEuclid(stations[idDepart], stations[idArrivee]);
            ouvert.Add((estimPassantParN[idDepart], idDepart));

            while (ouvert.Count > 0)
            {
                var (currentF, IdCourrant) = ouvert.Min;
                ouvert.Remove(ouvert.Min);

                if (IdCourrant == idArrivee)
                    return ReconstituerChemin(precedent, stations, idArrivee);

                var currentStation = stations[IdCourrant];

                foreach (var arc in currentStation.ArcsSortants)
                {
                    int voisinId = arc.Destination.Id;
                    double tentativeG = distAccumuleeReelle[IdCourrant] + arc.Distance;

                    if (tentativeG < distAccumuleeReelle[voisinId])
                    {
                        precedent[voisinId] = IdCourrant;
                        distAccumuleeReelle[voisinId] = tentativeG;
                        estimPassantParN[voisinId] = tentativeG + DistanceEuclid(arc.Destination, stations[idArrivee]);
                        ouvert.RemoveWhere(t => t.idStation == voisinId);
                        ouvert.Add((estimPassantParN[voisinId], voisinId));
                    }
                }
            }
            return new List<StationNoeud>(); 
        }

        /// <summary>
        /// Reconstruction du chemin en liste de StationNoeud
        /// </summary>
        /// <param name="precedent"></param>
        /// <param name="stations"></param>
        /// <param name="idArrivee"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Classe qui compare deux distances entre elles
        /// </summary>
        private class DistanceComparer : IComparer<(double fScore, int id)>
        {
            /// <summary>
            /// Méthode qui permet la comparaison
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public int Compare((double fScore, int id) x, (double fScore, int id) y)
            {
                int compareF = x.fScore.CompareTo(y.fScore);
                return compareF != 0 ? compareF : x.id.CompareTo(y.id);
            }
        }
    }
}