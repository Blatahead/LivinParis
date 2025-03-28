using MySql.Data.MySqlClient;
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

        #region constructeur
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


        #region Méthodes


        /// <summary>
        /// Méthode permettant de créer une station dans la table 'Station'
        /// </summary>
        /// <param name="p1"></param>
        public void CreerStation(Station<T> p1)
        {
            ConnexionDB.ConnectToDatabase();
            string demande = "INSERT INTO Station (ID_station, Nom_station, Longitude, Latitude, Libelle_ligne) VALUES ("+p1.identifiantStation+","+p1.nomStation+","+p1.longitude+","+p1.latitude+","+p1.libelle_ligne+")";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }

        /// <summary>
        /// Méthode permettant de modifier une station dans la table 'Station'
        /// </summary>
        /// <param name="p1"></param>
        public void ModifierStation(Station<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "UPDATE SET Station ID_station="+p1.identifiantStation+", Nom_station="+p1.nomStation+", Longitude="+p1.longitude+", Latitude="+p1.latitude+", Libelle_ligne="+p1.libelle_ligne+" WHERE Nom_station="+p1.nomStation+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }

        /// <summary>
        /// Méthode supprimant une station de la table 'Station' en s'assurant d'abord que toutes les clés étrangères liées dans les autres tables soient préalablement supprimées
        /// </summary>
        /// <param name="p1"></param>
        public void DeleteStation(Station<T> p1)
        {


            ConnexionDB.ConnectToDatabase();
            string demande = "DELETE FROM Station WHERE ="+p1.nomStation+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }
        #endregion






    }
}
