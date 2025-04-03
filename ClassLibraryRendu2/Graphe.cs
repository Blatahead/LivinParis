using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Graphe
    {
        public List<StationNoeud> Stations { get; set; } = new();
        public List<Arc> Arcs { get; set; } = new();

        public void AjouterStation(StationNoeud station)
        {
            if (!Stations.Any(s => s.Id == station.Id))
                Stations.Add(station);
        }

        public void AjouterArc(int idSource, int idDestination, double distance)
        {
            var source = Stations.FirstOrDefault(s => s.Id == idSource);
            var destination = Stations.FirstOrDefault(s => s.Id == idDestination);

            if (source == null || destination == null)
                throw new Exception("Source ou destination introuvable");

            var arc = new Arc(source, destination, distance);
            source.ArcsSortants.Add(arc);
            Arcs.Add(arc);
        }
    }

}
