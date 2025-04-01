using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Cuisinier<T> : Utilisateur<T>
    {
        int id_Cuisinier;
        string nom;
        string prenom;
        string adresseCuisinier;
        List<T> liste_commandes;
        List<T> liste_commandes_pretes;
        List<T> liste_commandes_livrees;
        List<T> liste_de_plats;


        #region constructeur
        public Cuisinier(int id_Cuisinier, int idUser, string mdp, string adresse_mail, string nomCuisinier, string prenomCuisinier, string adresseCuisinier, List<T> liste_commandes, List<T> liste_commandes_pretes, List<T> liste_commandes_livrees, List<T> liste_de_plats) : base(idUser, mdp, adresse_mail)
        {
            this.id_Cuisinier = id_Cuisinier;
            this.nom=nomCuisinier;
            this.prenom=nomCuisinier;
            this.adresseCuisinier=adresseCuisinier;
            this.liste_commandes=liste_commandes;
            this.liste_commandes_pretes=liste_commandes_pretes;
            this.liste_commandes_livrees=liste_commandes_livrees;
            this.liste_de_plats=liste_de_plats;

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
        public int Id_Cuisinier
        {
            get { return id_Cuisinier; }
            set { id_Cuisinier= value; }
        }
        public List<T> Liste_de_plats
        {
            get { return liste_de_plats; }
            set { liste_de_plats=value; }
        }





        #endregion

        #region Méthodes

        /// <summary>
        /// Méthode permettant de créer un cuisinier dans la table 'Cuisinier'
        /// </summary>
        /// <param name="p1"></param>
        public void CreerCuisinier(Cuisinier<T> p1)
        {
            ConnexionDB.ConnectToDatabase();
            string demande = "INSERT INTO Cuisinier (Id_Cuisinier,Prenom_cuisinier,Nom_particulier,Adresse_cuisinier,Liste_commandes,Liste_commandes_pretes, Liste_commandes_livrees, Liste_de_plats, Id_Utilisateur) VALUES ("+p1.id_Cuisinier+","+p1.prenom+","+p1.nom+","+p1.adresseCuisinier+","+p1.liste_commandes+","+p1.liste_commandes_pretes+","+p1.liste_commandes_livrees+","+p1.liste_de_plats+","+p1.IdUser+"+)";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }


        /// <summary>
        /// Méthode permettant de modifier un cuisinier dans la table 'Cuisinier'
        /// </summary>
        /// <param name="p1"></param>
        public void ModifierCuisinier(Cuisinier<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "UPDATE Cuisinier SET Id_Cuisinier="+p1.id_Cuisinier+", Prenom_cuisinier="+p1.prenom+", Nom_particulier="+p1.nom+", Adresse_cuisinier="+p1.adresseCuisinier+", Liste_commandes="+p1.liste_commandes+", Liste_commandes_pretes="+p1.liste_commandes_pretes+", Liste_commandes_livrees="+p1.liste_commandes_livrees+", Liste_de_plats="+p1.liste_de_plats+", WHERE Id_Cuisinier="+p1.id_Cuisinier+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }


        /// <summary>
        /// Méthode supprimant un cuisinier de la table 'Cuisinier' en s'assurant d'abord que toutes les clés étrangères liées dans les autres tables soient préalablement supprimées
        /// </summary>
        /// <param name="p1"></param>
        public void DeleteCuisinier(Cuisinier<T> p1)
        {


            ConnexionDB.ConnectToDatabase();
            try
            {
                string demande1 = "DELETE FROM Plat WHERE id_Cuisinier="+p1.id_Cuisinier+";";
                using (MySqlCommand cmd = new MySqlCommand(demande1)) ;

            }
            catch
            {
                Exception exception = null;
            }
            string demande = "DELETE FROM Cuisinier WHERE Id_Cuisinier="+p1.id_Cuisinier+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }
        #endregion








    }
}
