using ClassLibraryRendu1;
using MySql.Data.MySqlClient;
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

        public void AjouterArc(int idSource, int idDestination, double distance, string ligne)
        {
            var source = Stations.FirstOrDefault(s => s.Id == idSource);
            var destination = Stations.FirstOrDefault(s => s.Id == idDestination);

            if (source == null || destination == null)
                throw new Exception("Source ou destination introuvable");

            var arc = new Arc(source, destination, distance, ligne);

            source.ArcsSortants.Add(arc);
            Arcs.Add(arc);
        }
        public void ChargerDepuisBDD(string connectionString)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            // stations
            var cmdStations = new MySqlCommand("SELECT ID_station, Libelle_station, Latitude, Longitude FROM Station", conn);
            using var readerStations = cmdStations.ExecuteReader();
            while (readerStations.Read())
            {
                int id = readerStations.GetInt32(0);
                string nom = readerStations.GetString(1);
                double lat = readerStations.GetDouble(2);
                double lon = readerStations.GetDouble(3);

                var station = new StationNoeud(id, nom, lat, lon);
                AjouterStation(station);
            }
            readerStations.Close();

            // arcs
            var cmdArcs = new MySqlCommand("SELECT Depart, Arrivee, Distance, Libelle_ligne FROM Station WHERE Depart != 0 AND Arrivee != 0", conn);
            using var readerArcs = cmdArcs.ExecuteReader();
            while (readerArcs.Read())
            {
                int depart = readerArcs.GetInt32(0);
                int arrivee = readerArcs.GetInt32(1);
                double distance = readerArcs.GetDouble(2);
                string ligne = readerArcs.GetString(3);

                try
                {
                    AjouterArc(depart, arrivee, distance, ligne);
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}