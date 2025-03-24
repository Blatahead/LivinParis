using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Station<T>
    {
        int identifiantStation;
        string nomStation;
        double longitude;
        double latitude;
        int libelle_ligne;

        #region propriété
        public Station(int identifiantStation, string nomStation, double longitude, double latitude, int libelle_ligne)
        {
            this.identifiantStation = identifiantStation;
            this.nomStation = nomStation;
            this.longitude = longitude;
            this.latitude = latitude;
            this.libelle_ligne = libelle_ligne;
        }
        #endregion

        #region propriétés
        public int IdentifiantStation { get { return identifiantStation; } }
        public string NomStation { get { return nomStation; } }
        public double Longitude { get { return longitude; } }
        public double Latitude { get { return latitude; } }
        public int Libelle_ligne
        {
            get { return libelle_ligne; }

        }
        #endregion
    }
}
