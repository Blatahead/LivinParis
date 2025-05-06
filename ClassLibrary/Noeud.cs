using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Noeud
    {
        public string Id { get; set; }
        public string Type { get; set; }  
        public double Latitude { get; set; }
        public double Longitude { get; set; }


        public Noeud(string id, string type, double latitude, double longitude)
        {
            Id = id;
            Type = type;
            Latitude = latitude;
            Longitude = longitude;
        }
        public Noeud(string id, string type)
        {
            Id = id;
            Type = type;
        }
        public override string ToString() => $"[{Type}] {Id}";
    }
}
