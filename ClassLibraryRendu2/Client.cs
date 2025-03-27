using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Client<T>: Utilisateur<T>
    {
        int idClient;

        #region constructeur
        public Client(int idUser, string mdp, string adresse_mail,int idClient): base(idUser,mdp,adresse_mail)
        {
            this.idClient=idClient;
        }
        #endregion
        #region propriétés
        public int IdClient
        {
            set { idClient=value; }
        }
        #endregion

    }
    public void CreerClient(Client<T> p1)
    {
        ConnexionDB.ConnectToDatabase();
        string demande = "INSERT INTO Utilisateur (Id_utilisateur, Mdp, Mail_utilisateur) VALUES ("+p1.idUser+","+p1.mdp+","+p1.adresse_mail+")";
        using (MySqlCommand cmd = new MySqlCommand(demande)) ;


    }

    public void ModifierClient(Client<T> p1)
    {

        ConnexionDB.ConnectToDatabase();
        string demande = "UPDATE SET Utilisateur Id_Utilisateur="+p1.idUser+", Mdp="+p1.mdp+" WHERE Adresse_mail="+p1.adresse_mail+";";
        using (MySqlCommand cmd = new MySqlCommand(demande)) ;

    }

    public void DeleteClient(Client<T> p1)
    {
        
        
            ConnexionDB.ConnectToDatabase();
            string demande = "DELETE FROM Client_ WHERE ="+p1.idUser+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        
    }













}
