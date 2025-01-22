using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using DotNetEnv;
using System.Diagnostics;

namespace LivinParis
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // MySQL en parallèle
            var databaseTask = Task.Run(() => ConnectToDatabase());

            Application.Run(new Form1());
        }

        static void ConnectToDatabase()
        {
            Env.Load("../../../../.env");

            string HOST = Environment.GetEnvironmentVariable("HOST");
            string USER = Environment.GetEnvironmentVariable("USER");
            string PWD = Environment.GetEnvironmentVariable("PWD");
            string DataBase = "world";
            Debug.WriteLine(HOST);

            string myConnectionString = $"server={HOST};uid={USER};pwd={PWD};database={DataBase}";

            try
            {
                using var myConnection = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString);
                myConnection.Open();
                MessageBox.Show("Connexion réussie !");

                MySqlCommand myCommand = new MySqlCommand
                {
                    Connection = myConnection,
                    CommandText = @"SELECT * FROM city WHERE ID = @cityId;"
                };
                myCommand.Parameters.AddWithValue("@cityId", 1);

                using var myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    var id = myReader.GetInt32("ID");
                    var name = myReader.GetString("Name");
                    var countryCode = myReader.GetString("CountryCode");
                    var population = myReader.GetInt32("Population");

                    MessageBox.Show($"ID: {id}, Name: {name}, CountryCode: {countryCode}, Population: {population}");
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}