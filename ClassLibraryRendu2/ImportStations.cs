using System.Globalization;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.IO;

namespace ClassLibraryRendu2
{
    public class ImportStations
    {
        private readonly IConfiguration _config;

        public ImportStations(IConfiguration config)
        {
            _config = config;
        }

        public void ImporterDepuisMTX(string cheminFichier)
        {
            string connStr = _config.GetConnectionString("MyDb");

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            foreach (string ligne in File.ReadLines(cheminFichier))
            {
                if (ligne.StartsWith("%") || string.IsNullOrWhiteSpace(ligne))
                    continue;

                var parties = ligne.Split('\t');

                if (parties.Length < 11 || !int.TryParse(parties[2], out int id))
                    continue;

                var dto = StationConvertisseurs.FromMtxLine(parties);


                var cmd = new MySqlCommand("INSERT INTO Station (ID_station, Libelle_station, Longitude, Latitude, Libelle_ligne, Commune_code, Temps_2_stations, Depart, Arrivee, Sens, Distance) " +
                                            "VALUES (@Id, @Nom, @Longitude, @Latitude, @Ligne, @CodePostal, @Temps, @Depart, @Arrivee, @Sens, @Distance)", conn);

                cmd.Parameters.AddWithValue("@Id", dto.Id);
                cmd.Parameters.AddWithValue("@Nom", dto.Nom);
                cmd.Parameters.AddWithValue("@Longitude", dto.Longitude);
                cmd.Parameters.AddWithValue("@Latitude", dto.Latitude);
                cmd.Parameters.AddWithValue("@Ligne", dto.Ligne);
                cmd.Parameters.AddWithValue("@CodePostal", dto.CodePostal);
                cmd.Parameters.AddWithValue("@Temps", dto.TempsVersStation);
                cmd.Parameters.AddWithValue("@Depart", dto.Depart);
                cmd.Parameters.AddWithValue("@Arrivee", dto.Arrivee);
                cmd.Parameters.AddWithValue("@Sens", dto.Sens);
                cmd.Parameters.AddWithValue("@Distance", dto.Distance);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw;
                }

            }
        }
    }
}