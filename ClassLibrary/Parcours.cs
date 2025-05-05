using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary;
using ClassLibrary;// Pour Graphe, Noeud, Lien

namespace ClassLibrary
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

        //mis dans la classe Graphe

        /// <summary>
        /// Squelette de l'algorithme de Bellman-Ford pour gérer des graphes avec poids négatifs
        /// </summary>
        public static List<StationNoeud> BellmanFord(int idDepart, int idArrivee, List<Arc> arcs)
        {
            var distances = new Dictionary<int, double>();
            var predecessors = new Dictionary<int, int>();
            var idToStation = new Dictionary<int, StationNoeud>();
            foreach (var arc in arcs)
            {
                if (!idToStation.ContainsKey(arc.Source.Id))
                    idToStation[arc.Source.Id] = arc.Source;

                if (!idToStation.ContainsKey(arc.Destination.Id))
                    idToStation[arc.Destination.Id] = arc.Destination;

                distances[arc.Source.Id] = double.PositiveInfinity;
                distances[arc.Destination.Id] = double.PositiveInfinity;
            }
            distances[idDepart] = 0;

            int nombreStations = idToStation.Count;

            for (int i = 0; i < nombreStations - 1; i++)
            {
                foreach (var arc in arcs)
                {
                    double poids = arc.Distance;
                    if (distances[arc.Source.Id] + poids < distances[arc.Destination.Id])
                    {
                        distances[arc.Destination.Id] = distances[arc.Source.Id] + poids;
                        predecessors[arc.Destination.Id] = arc.Source.Id;
                    }
                }
            }
            foreach (var arc in arcs)
            {
                double poids = arc.Distance;
                if (distances[arc.Source.Id] + poids < distances[arc.Destination.Id])
                {
                    throw new Exception("Le graphe contient un cycle de poids négatif.");
                }
            }
            var chemin = new List<StationNoeud>();
            int courant = idArrivee;

            if (!predecessors.ContainsKey(courant) && courant != idDepart)
                return new List<StationNoeud>(); 

            while (courant != idDepart)
            {
                chemin.Insert(0, idToStation[courant]);
                courant = predecessors[courant];
            }
            chemin.Insert(0, idToStation[idDepart]);

            return chemin;
        }



    }
}