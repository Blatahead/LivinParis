using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Client<T> : Utilisateur<T>
    {
        int idClient;

        #region constructeur
        public Client(int idUser, string mdp, string adresse_mail, int idClient) : base(idUser, mdp, adresse_mail)
        {
            this.idClient=idClient;
        }
        #endregion
        #region propriétés
        public int IdClient
        {
            get { return idClient; }
            set { idClient=value; }
        }
        #endregion


        public void CreerClient(Client<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "INSERT INTO Client_ (Id_Client) VALUES ("+p1.idClient+")";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;
            


        }

        public void ModifierClient(Client<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "UPDATE SET Client_ Id_CLient="+p1.idClient+" WHERE Id_CLient="+p1.idClient+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }

        public void DeleteClient(Client<T> p1)
        {


            ConnexionDB.ConnectToDatabase();
            try
            {
                string demande1 = "DELETE FROM Particulier WHERE Id_Client="+p1.idClient+";";
                string demande2 = "DELETE FROM Entreprise WHERE Id_Client ="+p1.idClient+";";
                using (MySqlCommand cmd = new MySqlCommand(demande1)) ;
                using (MySqlCommand cmd = new MySqlCommand(demande2)) ;


            }
            catch
            {
                Exception exception = null;
            }
            string demande = "DELETE FROM Client_ WHERE ="+p1.idClient+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }

    }












}
