using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Reflection;

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
            string demande = "UPDATE Station SET ID_station="+p1.identifiantStation+", Nom_station="+p1.nomStation+", Longitude="+p1.longitude+", Latitude="+p1.latitude+", Libelle_ligne="+p1.libelle_ligne+" WHERE Nom_station="+p1.nomStation+";";
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
        static void CalculDistance2stations(Station<T> p1, Station<T> p2)
            {
                double distance = 2*6371*Math.Asin(Math.Sqrt(Math.Pow(Math.Sin((p2.latitude-p1.latitude)/2), 2)+ Math.Cos(p1.latitude)*Math.Cos(p2.latitude)*Math.Pow(Math.Sin((p2.longitude-p1.longitude)/2), 2)));


            }

        #endregion








    }
}
