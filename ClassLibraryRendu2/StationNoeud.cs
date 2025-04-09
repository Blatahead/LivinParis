using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class StationNoeud
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Ligne { get; set; }
        public List<Arc> ArcsSortants { get; set; } = new();

        public StationNoeud(int id, string nom, double latitude, double longitude, string ligne)
        {
            Id = id;
            Nom = nom;
            Latitude = latitude;
            Longitude = longitude;
            Ligne = ligne;
        }
    }
}