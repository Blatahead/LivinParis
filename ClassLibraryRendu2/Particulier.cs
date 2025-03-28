using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Particulier<T>: Client<T>
    {
        int idParticulier;
        string prenomParticulier;
        string nomParticulier;
        string adresseParticulier;
        #region constructeur
        public Particulier(int idUser, string mdp, string adresse_mail, int idClient,int numeroParticulier, string prenomParticulier, string nomParticulier, string adresseParticulier):base(idUser, mdp, adresse_mail,idClient)
        {
            this.idParticulier = numeroParticulier;
            this.prenomParticulier= prenomParticulier;
            this.nomParticulier= nomParticulier;
            this.adresseParticulier=adresseParticulier;
        }
        #endregion 
        #region propriétés
        public int IdParticulier
        {
            set { idParticulier=value; }
        }
        public string PrenomParticulier
        {
            set { prenomParticulier=value; }
        }
        public string NomParticulier
        {
            set { nomParticulier=value; }
        }
        public string AdresseParticulier
        {
            set { adresseParticulier=value; }
        }
        #endregion


        public void CreerParticulier(Particulier<T> p1)
        {
            ConnexionDB.ConnectToDatabase();
            string demande = "INSERT INTO Particulier (Id_particulier, Prenom_particulier, Nom_particulier, Adresse_particulier) VALUES ("+p1.idParticulier+","+p1.prenomParticulier+","+p1.nomParticulier+","+p1.adresseParticulier+")";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }
        public void ModifierParticulier(Particulier<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "UPDATE SET Particulier Id_particulier="+p1.idParticulier+", Prenom_particulier="+p1.prenomParticulier+", Nom_particulier="+p1.nomParticulier+" WHERE Adresse_particulier="+p1.adresseParticulier+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }

        public void DeleteParticulier(Particulier<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "DELETE FROM Particulier WHERE id_particulier="+p1.idParticulier+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }
    }
}
