using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using ClassLibrary;

namespace ClassLibrary
{
    public static class StationConvertisseurs
    {
        public static StationDTO FromMtxLine(string[] parts)
        {
            return new StationDTO
            {
                Depart = Convert.ToInt32(parts[0]),
                Arrivee = Convert.ToInt32(parts[1]),
                Id = Convert.ToInt32(parts[2]),
                Ligne = parts[3],
                Nom = parts[4].Trim('"'),
                Longitude = Convert.ToDouble(parts[5].Replace(',', '.'), CultureInfo.InvariantCulture),
                Latitude = Convert.ToDouble(parts[6].Replace(',', '.'), CultureInfo.InvariantCulture),
                CodePostal = Convert.ToInt32(parts[7]),
                Sens = Convert.ToInt32(parts[8]),
                Distance = Convert.ToDouble(parts[9].Replace(',', '.'), CultureInfo.InvariantCulture),
                TempsVersStation = Convert.ToInt32(parts[10]),
            };
        }

        public static StationNoeud ToNoeud(StationDTO dto)
        {
            return new StationNoeud(dto.Id, dto.Nom, dto.Latitude, dto.Longitude, dto.Ligne);
        }

        public static Station<object> NoeudToStation(StationNoeud s)
        {
            return new Station<object>(s.Id, s.Nom, s.Longitude, s.Latitude, s.Ligne, s.Depart, s.Arrivee, s.Distance);
        }
        public static StationNoeud StationToNoeud(Station<object> s)
        {
            return new StationNoeud(s.IdentifiantStation, s.NomStation, s.Latitude, s.Longitude, s.Libelle_ligne.ToString(), s.Depart, s.Arrivee, s.Distance)
            {
                Id = s.IdentifiantStation,
                Nom = s.NomStation,
                Longitude = s.Longitude,
                Latitude = s.Latitude,
                Ligne = s.Libelle_ligne.ToString(),
                Depart = s.Depart,
                Arrivee = s.Arrivee,
                Distance = s.Distance
            };
        }
        public static StationDTO ToDTO(StationNoeud station)
        {
            return new StationDTO
            {
                Id = station.Id,
                Nom = station.Nom,
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                Ligne = station.Ligne,
            };
        }
        public static ArcDTO ArcToDTO(Arc arc)
        {
            return new ArcDTO
            {
                SourceId = arc.Source.Id,
                DestinationId = arc.Destination.Id,
                Distance = arc.Distance,
                Ligne = arc.Ligne
            };
        }
    }
}