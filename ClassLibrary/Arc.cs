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
        #region Proprietes
        public StationNoeud Source { get; set; }
        public StationNoeud Destination { get; set; }
        public double Distance { get; set; }
        public string Ligne { get; set; }
        #endregion

        #region Constructeur
        public Arc(StationNoeud source, StationNoeud destination, double distance, string ligne)
        {
            Source = source;
            Destination = destination;
            Distance = distance;
            Ligne = ligne;
        }
        #endregion
    }
}