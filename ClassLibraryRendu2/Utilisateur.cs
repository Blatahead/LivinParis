using MySql.Data.MySqlClient;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
  

namespace ClassLibraryRendu2
{
    public class Utilisateur<T>
    {
        int idUser;
        string mdp;
        string adresse_mail;

        #region constructeur
        public Utilisateur(int idUser, string mdp, string adresse_mail)
        {
            this.idUser = idUser;   
            this.mdp=mdp;
            this.adresse_mail= adresse_mail;
        }
        #endregion
        #region propriétés
        public int IdUser
        {
            get { return idUser; }
            set { idUser=value; }
        }
        public string Mdp
        {
            set { mdp=value; }
        }
        public string Adresse_mail
        {
            set { adresse_mail=value; }
        }
        #endregion

        public void CreerUser(Utilisateur<T> p1)
        {
            ConnexionDB.ConnectToDatabase();
            string demande = "INSERT INTO Utilisateur (Id_utilisateur, Mdp, Mail_utilisateur) VALUES ("+p1.idUser+","+p1.mdp+","+p1.adresse_mail+")";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;
            

        }

        public void ModifierUser(Utilisateur<T> p1)
        {
            
            ConnexionDB.ConnectToDatabase();
            string demande = "UPDATE SET Utilisateur Id_Utilisateur="+p1.idUser+", Mdp="+p1.mdp+" WHERE Adresse_mail="+p1.adresse_mail+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }

        public void DeleteUser(Utilisateur<T> p1)
        {
            
                ConnexionDB.ConnectToDatabase();
                string demande = "DELETE FROM Utilisateur WHERE Id_Utilisateur="+p1.idUser+";";
                using (MySqlCommand cmd = new MySqlCommand(demande)) ;
       
        }
    }
}
