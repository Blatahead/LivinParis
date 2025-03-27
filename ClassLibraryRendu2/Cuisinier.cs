using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Cuisinier<T>:Utilisateur<T>
    {
        int id_Cuisinier;
        string nom;
        string prenom;
        string adresseCuisinier;
        List<T> liste_commandes;
        List<T> liste_commandes_pretes;
        List<T> liste_commandes_livrees;

        #region constructeur
        public Cuisinier(int id_Cuisinier, int idUser, string mdp, string adresse_mail, string nomCuisinier, string prenomCuisinier, string adresseCuisinier, List<T> liste_commandes, List<T> liste_commandes_pretes, List<T> liste_commandes_livrees) : base(idUser, mdp, adresse_mail)
        {
            this.id_Cuisinier = id_Cuisinier;
            this.nom=nomCuisinier;
            this.prenom=nomCuisinier;
            this.adresseCuisinier=adresseCuisinier;
            this.liste_commandes=liste_commandes;
            this.liste_commandes_pretes=liste_commandes_pretes;
            this.liste_commandes_livrees=liste_commandes_livrees;
        }
        #endregion
        #region propriétés
        public string Nom
        {
            get { return nom; }
        }
        public string Prenom
        {
            get { return prenom; }
        }
        public string AdresseCuisinier
        {
            set { adresseCuisinier=value; }
        }

        public List<T> Liste_commandes
        {
            get { return liste_commandes; }
        }
        public List<T> Liste_commandes_pretes
        {
            get { return liste_commandes_pretes; }
        }
        public List<T> Liste_commandes_livrees
        {
            get { return liste_commandes_livrees; }
        }





        #endregion

        public void CreerCuisinier(Cuisinier<T> p1)
        {
            ConnexionDB.ConnectToDatabase();
            string demande = "INSERT INTO Cuisinier (Id_Cuisinier,Prenom_cuisinier,Nom_particulier,Adresse_cuisinier) VALUES ("+p1.id_Cuisinier+","+p1.prenom+","+p1.nom+","+p1.adresseCuisinier+")";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }

        public void ModifierCuisinier(Cuisinier<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "UPDATE SET Cuisinier Id_Cuisinier="+p1.id_Cuisinier+", Prenom_cuisinier="+p1.prenom+", Nom_particulier="+p1.nom+", Adresse_cuisinier="+p1.adresseCuisinier+" WHERE Id_Cuisinier="+p1.id_Cuisinier+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }

        public void DeleteCuisinier(Cuisinier<T> p1)
        {


            ConnexionDB.ConnectToDatabase();
            string demande = "DELETE FROM Cuisinier WHERE ="+p1.id_Cuisinier+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }








    }
}
