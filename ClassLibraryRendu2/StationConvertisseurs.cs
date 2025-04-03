using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Globalization;

namespace ClassLibraryRendu2
{
    public static class StationConvertisseurs
    {
        public static StationDTO FromMtxLine(string[] parts)
        {
            // tiliser lors de la lecture du fichier .mtx
            // créer un objet complet pour la bdd
            return new StationDTO
            {
                Depart = Convert.ToInt32(parts[0]),
                Arrivee = Convert.ToInt32(parts[1]),
                Id = Convert.ToInt32(parts[2]),
                Ligne = parts[3],
                Nom = parts[4].Trim('"'),
                Longitude = Convert.ToDouble(parts[5]),
                Latitude = Convert.ToDouble(parts[6]),
                CodePostal = Convert.ToInt32(parts[7]),
                Sens = Convert.ToInt32(parts[8]),
                Distance = Convert.ToDouble(parts[9]),
                TempsVersStation = Convert.ToInt32(parts[10]),
            };
        }

        public static StationNoeud ToNoeud(StationDTO dto)
        {
            // pour créer le graphe
            return new StationNoeud(dto.Id, dto.Nom, dto.Latitude, dto.Longitude);
        }
    }
}
