using DotNetEnv;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;

namespace ClassLibrary
{
    public class ConnexionDB
    {
        #region Méthode
        /// <summary>
        /// Instancie une connexion MySql à la database depuis le fichier .env
        /// </summary>
        public static void ConnectToDatabase()
        {
            Env.Load("../../../../.env");

            string HOST = Environment.GetEnvironmentVariable("HOST");
            string USER = Environment.GetEnvironmentVariable("USER");
            string PWD = Environment.GetEnvironmentVariable("PWD");
            string DataBase = "antomath";
            Debug.WriteLine(HOST);

            string myConnectionString = $"server={HOST};uid={USER};pwd={PWD};database={DataBase}";

            try
            {
                using var myConnection = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString);
                myConnection.Open();

                MySqlCommand myCommand = new MySqlCommand
                {
                    Connection = myConnection,
                };
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
            }
        }
        #endregion
    }
}