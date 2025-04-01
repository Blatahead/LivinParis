using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Commande<T> : Utilisateur<T>
    {
        int numeroCommande;
        float prixCommande;
        int noteCommande;
        List<T> liste_plats;
        #region constructeur
        public Commande(int idUser, string mdp, string adresse_mail, int numeroCommande, int prixCommande, int noteCommande, List<T> liste_plats) : base(idUser, mdp, adresse_mail)
        {
            this.numeroCommande = numeroCommande;
            this.prixCommande = prixCommande;
            this.noteCommande = noteCommande;
            this.liste_plats=liste_plats;
        }
        #endregion
        #region propriétés
        public int NumeroCommande
        {
            get { return numeroCommande; }
        }
        public float PrixCommande
        {
            get { return prixCommande; }
        }
        public int NoteCommande
        {
            get { return noteCommande; }
            set { noteCommande=value; }
        }
        #endregion


        #region Méthodes
        /// <summary>
        /// Méthode permettant de créer une commande dans la table 'Commande'
        /// </summary>
        /// <param name="p1"></param>
        public void CreerCommande(Commande<T> p1)
        {
            ConnexionDB.ConnectToDatabase();
            string demande = "INSERT INTO Commande (Num_commande, Prix_commande, Note_commande, Id_Utilisateur) VALUES ("+p1.numeroCommande+","+p1.prixCommande+","+p1.noteCommande+","+p1.IdUser+")";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }

        /// <summary>
        /// Méthode permettant de modifier une commande dans la table 'Commande'
        /// </summary>
        /// <param name="p1"></param>

        public void ModifierCommande(Commande<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "UPDATE Commande SET Num_commande="+p1.numeroCommande+", Prix_commande="+ p1.prixCommande+", Note_commande="+p1.noteCommande+" WHERE Num_commande="+p1.numeroCommande+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }

        /// <summary>
        /// Méthode supprimant une commande de la table 'Commande' en s'assurant d'abord que toutes les clés étrangères liées dans les autres tables soient préalablement supprimées
        /// </summary>
        /// <param name="p1"></param>
        public void DeleteCommande(Commande<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "DELETE FROM Commande WHERE ="+p1.numeroCommande+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }
        #endregion










    }
}
