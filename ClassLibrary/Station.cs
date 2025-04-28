using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Reflection;
using ClassLibrary;

namespace ClassLibrary
{
    public class Station<T>
    {
        int identifiantStation;
        string nomStation;
        double longitude;
        double latitude;
        string libelle_ligne;
        int code_postal;
        int temps_2_station;
        int depart;
        int arrivee;
        int sens;
        double distance;
        

        #region constructeur
        public Station(int identifiantStation, string nomStation, double longitude, double latitude, string libelle_ligne, int code_postal, int temps_2_station, int depart, int arrivee, int sens, double distance)
        {
            this.identifiantStation = identifiantStation;
            this.nomStation = nomStation;
            this.longitude = longitude;
            this.latitude = latitude;
            this.libelle_ligne = libelle_ligne;
            this.code_postal=code_postal;
            this.temps_2_station=temps_2_station;
            this.depart=depart;
            this.arrivee=arrivee;
            this.sens=sens;
            this.distance=distance;
        }

        public Station(int identifiantStation, string nomStation, double longitude, double latitude, string libelle_ligne, int depart, int arrivee, double distance)
        {
            this.identifiantStation = identifiantStation;
            this.nomStation = nomStation;
            this.longitude = longitude;
            this.latitude = latitude;
            this.libelle_ligne = libelle_ligne;
            this.depart = depart;
            this.arrivee = arrivee;
            this.distance = distance;
        }
        #endregion

        #region propriétés
        public int IdentifiantStation { get { return identifiantStation; }  set { identifiantStation=value; } }
        public string NomStation { get { return nomStation; } set { nomStation=value; } }
        public double Longitude { get { return longitude; } set { Longitude=value; } }
        public double Latitude { get { return latitude; } set { Latitude=value; } }
        public string Libelle_ligne
        {
            get { return libelle_ligne; }
            set { libelle_ligne=value; }

        }
        public int Code_postal
        {
            get { return code_postal; }
            set { code_postal=value; }

        }
        public int Temps_2_station
        {
            get { return temps_2_station; }
            set { temps_2_station=value; }
        }
        public int Depart
        { get { return depart; } set { depart=value; } }
        public int Arrivee
        {
            get { return arrivee; }
            set { arrivee=value; }
        }
        public int Sens
        {
            get { return sens; }
            set { sens=value; }
        }
        public double Distance
        { get { return distance; } set { distance=value; } }    




        #endregion

        #region Méthodes


        /// <summary>
        /// Méthode permettant de créer une station dans la table 'Station'
        /// </summary>
        /// <param name="p1"></param>
        public void CreerStation(Station<T> p1)
        {
            ConnexionDB.ConnectToDatabase();
            string demande = "INSERT INTO Station (ID_station, Libelle_station, Longitude, Latitude, Libelle_ligne, Depart, Arrivee, Sens, Temps_2_stations, Commune_code, Distance) VALUES (" + p1.identifiantStation+","+p1.nomStation+","+p1.longitude+","+p1.latitude+","+p1.libelle_ligne+","+p1.Depart+","+p1.Arrivee+","+p1.Sens+","+p1.temps_2_station+","+p1.code_postal+","+p1.distance+")";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }

        /// <summary>
        /// Méthode permettant de modifier une station dans la table 'Station'
        /// </summary>
        /// <param name="p1"></param>
        public void ModifierStation(Station<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "UPDATE Station SET ID_station="+p1.identifiantStation+ ", Libelle_station=" + p1.nomStation+", Longitude="+p1.longitude+", Latitude="+p1.latitude+", Libelle_ligne="+p1.libelle_ligne+", Depart="+p1.depart+", Arrivee="+p1.arrivee+", Sens="+p1.sens+", Temps_2_stations="+p1.temps_2_station+", Commune_code="+p1.code_postal+", Distance="+p1.distance+" WHERE ID_station="+p1.identifiantStation+";";
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
        
        /// <summary>
        /// Méthode retournant le nom de la station la plus proche d'un particulier
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public async void StationProcheParticulier(Station<T> p1, Particulier<T> p2)
        {
            var (latitude, longitude)= await Convertisseur_coordonnees.GetCoordinatesAsync(p2.AdresseParticulier);
            {
                string plusCourt = "";
                double Min = 10000;
                foreach (PropertyInfo prop in this.GetType().GetProperties())
                {
                    double distance = 2*6371*Math.Asin(Math.Sqrt(Math.Pow(Math.Sin((latitude-p1.latitude)/2), 2)+ Math.Cos(p1.latitude)*Math.Cos(latitude)*Math.Pow(Math.Sin((longitude-p1.longitude)/2), 2)));
                    if (Min>distance)
                    {
                        Min=distance;
                        plusCourt=p1.nomStation;
                    }
                }
            }
        }

        /// <summary>
        /// Méthode retournant le nom de la station la plus proche d'une entreprise
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public async void StationProcheEntreprise(Station<T> p1, Entreprise<T> p2)
        {
            var (latitude, longitude)= await Convertisseur_coordonnees.GetCoordinatesAsync(p2.AdresseEntreprise);
            {
                string plusCourt = "";
                double Min = 10000;
                foreach (PropertyInfo prop in this.GetType().GetProperties())
                {
                    double distance = 2*6371*Math.Asin(Math.Sqrt(Math.Pow(Math.Sin((latitude-p1.latitude)/2), 2)+ Math.Cos(p1.latitude)*Math.Cos(latitude)*Math.Pow(Math.Sin((longitude-p1.longitude)/2), 2)));
                    if (Min>distance)
                    {
                        Min=distance;
                        plusCourt=p1.nomStation;
                    }
                }
            }
        }

        /// <summary>
        /// Méthode retournant le nom de la station la plus proche d'un cuisinier
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public async void StationProcheCuisinier(Station<T> p1, Cuisinier<T> p2)
        {
            var (latitude, longitude)= await Convertisseur_coordonnees.GetCoordinatesAsync(p2.AdresseCuisinier);
            {
                string plusCourt = "";
                double Min = 10000;
                foreach (PropertyInfo prop in this.GetType().GetProperties())
                {
                    double distance = 2*6371*Math.Asin(Math.Sqrt(Math.Pow(Math.Sin((latitude-p1.latitude)/2), 2)+ Math.Cos(p1.latitude)*Math.Cos(latitude)*Math.Pow(Math.Sin((longitude-p1.longitude)/2), 2)));
                    if (Min>distance)
                    {
                        Min=distance;
                        plusCourt=p1.nomStation;
                    }
                }
            }
        }


        /// <summary>
        /// Méthode calculant la distance entre deux stations
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public static double CalculDistance2stations(StationNoeud p1, StationNoeud p2)
        {
            double lat1 = ConvertDegToRad(p1.Latitude);
            double lon1 = ConvertDegToRad(p1.Longitude);
            double lat2 = ConvertDegToRad(p2.Latitude);
            double lon2 = ConvertDegToRad(p2.Longitude);

            return 2 * 6371 * Math.Asin(Math.Sqrt(
                Math.Pow(Math.Sin((lat2 - lat1) / 2), 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Pow(Math.Sin((lon2 - lon1) / 2), 2)
            ));
        }

        private static double ConvertDegToRad(double deg) => deg * Math.PI / 180;

        #endregion
    }
}