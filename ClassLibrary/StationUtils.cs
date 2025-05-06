using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public static class StationUtils
    {
        #region Methodes
        /// <summary>
        /// Calcule la distance entre deux points GPS avec la formule de Haversine
        /// </summary>
        public static double CalculerDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // rayon de la Terre en km
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        /// <summary>
        /// Renvoie la station la plus proche d'une latitude/longitude donnée
        /// </summary>
        public static StationNoeud GetStationProche(double latitude, double longitude, List<StationNoeud> stations)
        {
            StationNoeud plusProche = null;
            double distanceMin = double.MaxValue;

            foreach (var station in stations)
            {
                double dist = CalculerDistance(latitude, longitude, station.Latitude, station.Longitude);
                if (dist < distanceMin)
                {
                    distanceMin = dist;
                    plusProche = station;
                }
            }
            return plusProche;
        }

        /// <summary>
        /// Renvoie la station la plus proche d'une adresse en appelant l'API de geocodage
        /// </summary>
        public static async Task<StationNoeud> GetStationProche(string adresse, List<StationNoeud> stations)
        {
            var (latitude, longitude) = await Convertisseur_coordonnees.GetCoordinatesAsync(adresse);
            return GetStationProche(latitude, longitude, stations);
        }
        #endregion
    }
}