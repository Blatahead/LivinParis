using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;

namespace ClassLibrary
{
    public class Arc
    {
        public StationNoeud Source { get; set; }
        public StationNoeud Destination { get; set; }
        public double Distance { get; set; }
        public string Ligne { get; set; }

        public Arc(StationNoeud source, StationNoeud destination, double distance, string ligne)
        {
            Source = source;
            Destination = destination;
            Distance = distance;
            Ligne = ligne;
        }
    }


}
