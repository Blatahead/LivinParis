using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;

namespace ClassLibrary
{
    public class Entreprise<T> : Client<T>
    {
        int numeroSiret;
        string nomEntreprise;
        string nomReferent;
        string adresseEntreprise;

        #region constructeur
        public Entreprise(int idUser, string mdp, string adresse_mail, int idClient, int numeroSiret, string nomEntreprise, string nomReferent, string adresseEntreprise) : base(idUser, mdp, adresse_mail, idClient)
        {
            this.numeroSiret = numeroSiret;
            this.nomEntreprise=nomEntreprise;
            this.nomReferent=nomReferent;
            this.adresseEntreprise=adresseEntreprise;
        }
        #endregion
        #region propriétés
        public int NumeroSiret
        {
            get { return numeroSiret; }
        }


        public string NomEntreprise
        {
            get { return nomEntreprise; }
            set { nomEntreprise=value; }
        }

        public string NomReferent
        {
            set { nomReferent=value; }
        }
        public string AdresseEntreprise
        {
            set { adresseEntreprise=value; }
            get {  return adresseEntreprise; }
        }
        #endregion

        #region Méthodes

        /// <summary>
        /// Méthode permettant de créer une entreprise dans la table 'Entreprise'
        /// </summary>
        /// <param name="p1"></param>
        public void CreerEntreprise(Entreprise<T> p1)
        {
            ConnexionDB.ConnectToDatabase();
            string demande = "INSERT INTO Entreprise (Num_siret,Nom_entreprise,Nom_referent,Adresse_entreprise) VALUES ("+p1.numeroSiret+","+p1.nomEntreprise+","+p1.nomReferent+","+p1.adresseEntreprise+")";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }
        /// <summary>
        /// Méthode permettant de modifier une entreprise dans la table 'Entreprise'
        /// </summary>
        /// <param name="p1"></param>
        public void ModifierEntreprise(Entreprise<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "UPDATE Entreprise SET Num_siret="+p1.numeroSiret+", Nom_entreprise="+p1.nomEntreprise+", Nom_referent="+p1.nomReferent+" WHERE Id_Client="+p1.IdClient+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }

        /// <summary>
        /// Méthode supprimant une entreprise de la table 'Entreprise' en s'assurant d'abord que toutes les clés étrangères liées dans les autres tables soient préalablement supprimées
        /// </summary>
        /// <param name="p1"></param>
        public void DeleteEntreprise(Entreprise<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "DELETE FROM Entreprise WHERE Id_Client="+p1.IdClient+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }
        #endregion
    }
}