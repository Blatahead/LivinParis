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
            var cmdStations = new MySqlCommand("SELECT ID_station, Libelle_station, Latitude, Longitude, Libelle_ligne FROM Station", conn);
            using var readerStations = cmdStations.ExecuteReader();
            while (readerStations.Read())
            {
                int id = readerStations.GetInt32(0);
                string nom = readerStations.GetString(1);
                double lat = readerStations.GetDouble(2);
                double lon = readerStations.GetDouble(3);
                string ligne = readerStations.GetString(4);

                var station = new StationNoeud(id, nom, lat, lon, ligne);
                AjouterStation(station);
            }
            readerStations.Close();

            // arcs en lien direct (adjacence)
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
                catch (Exception)
                {
                }
            }
            readerArcs.Close();

            // arcs de correspondances (stations aux mêmes coordonnées)
            var groupesParCoord = Stations
                .GroupBy(s => $"{Math.Round(s.Latitude, 5)}|{Math.Round(s.Longitude, 5)}")
                .Where(g => g.Count() > 1);

            foreach (var groupe in groupesParCoord)
            {
                var stations = groupe.ToList();
                for (int i = 0; i < stations.Count; i++)
                {
                    for (int j = i + 1; j < stations.Count; j++)
                    {
                        AjouterArc(stations[i].Id, stations[j].Id, 0.1, stations[i].Ligne);
                        AjouterArc(stations[j].Id, stations[i].Id, 0.1, stations[j].Ligne);
                    }
                }
            }
        }

        public List<StationNoeud> Dijkstra(int idDepart, int idArrivee)
        {
            var distances = new Dictionary<int, double>();
            var precedent = new Dictionary<int, int?>();
            var visite = new HashSet<int>();
            var file = new SortedSet<(double distance, int idStation)>(new DistanceComparer());
            var stationsParId = Stations.ToDictionary(s => s.Id);

            foreach (var station in Stations)
            {
                distances[station.Id] = double.PositiveInfinity;
                precedent[station.Id] = null;
            }

            distances[idDepart] = 0;
            file.Add((0, idDepart));

            while (file.Count > 0)
            {
                var (distanceActuelle, idActuel) = file.Min;
                file.Remove(file.Min);

                if (visite.Contains(idActuel))
                    continue;

                visite.Add(idActuel);

                if (idActuel == idArrivee)
                    break;

                var stationActuelle = stationsParId[idActuel];

                foreach (var arc in stationActuelle.ArcsSortants)
                {
                    var voisin = arc.Destination;
                    double nouvelleDistance = distanceActuelle + arc.Distance;

                    if (nouvelleDistance < distances[voisin.Id])
                    {
                        file.Remove((distances[voisin.Id], voisin.Id)); // supprimer ancienne entrée
                        distances[voisin.Id] = nouvelleDistance;
                        precedent[voisin.Id] = idActuel;
                        file.Add((nouvelleDistance, voisin.Id));
                    }
                }
            }

            // Reconstruire le chemin
            var chemin = new List<StationNoeud>();
            int? courant = idArrivee;

            while (courant != null)
            {
                chemin.Insert(0, stationsParId[courant.Value]);
                courant = precedent[courant.Value];
            }

            // Si le premier noeud n’est pas le départ, aucun chemin trouvé
            if (chemin.Count == 0 || chemin[0].Id != idDepart)
                return new List<StationNoeud>(); // chemin vide donc pas de chemin

            return chemin;
        }
    }
}