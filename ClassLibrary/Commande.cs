using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;

namespace ClassLibrary
{
    public class Commande<T> : Plat<T>
    {
        #region Attributs
        int numeroCommande;
        float prixCommande;
        int noteCommande;
        string liste_plats;
        #endregion

        #region constructeur
        public Commande(int id_Cuisinier, int idUser, string mdp, string adresse_mail, string nomCuisinier, string prenomCuisinier, string adresseCuisinier, List<T> liste_de_plats, List<T> liste_commandes, List<T> liste_commandes_pretes, List<T> liste_commandes_livrees, int numPlatJ, string nomPlatJ, int ndpPlatJ, string typePlatJ, string nationalitePlatJ, string datePeremptionJ, float prixPlatJ, string ingredientsJ, string regimeAlimentaireJ, string photoJ, string dateFabricationJ, int numPlat, string nomPlat, int ndpPlat, string typePlat, string nationalitePlat, string datePeremption, float prixPlat, string ingredients, string regimeAlimentaire, string photo, string dateFabrication, int numeroCommande, float prixCommande, int noteCommande, string liste_plats) : base(id_Cuisinier, idUser, mdp, adresse_mail, nomCuisinier, prenomCuisinier, adresseCuisinier, liste_de_plats, liste_commandes, liste_commandes_pretes, liste_commandes_livrees, numPlatJ, nomPlatJ, ndpPlatJ, typePlatJ, nationalitePlatJ, datePeremptionJ, prixPlatJ, ingredientsJ, regimeAlimentaireJ, photoJ, dateFabricationJ, numPlat, nomPlat, ndpPlat, typePlat, nationalitePlat, datePeremption, prixPlat, ingredients, regimeAlimentaire, photo, dateFabrication)
        {
            this.numeroCommande = numeroCommande;
            this.prixCommande = prixCommande;
            this.noteCommande = noteCommande;
            this.liste_plats+=NumPlat+";";
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

        public string Liste_plats
        {
            get { return liste_plats; }
            set { liste_plats=value; }
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
            string demande = "INSERT INTO Commande (Num_commande, Prix_commande, Note_commande, liste_plats, Id_Utilisateur) VALUES ("+p1.numeroCommande+","+p1.prixCommande+","+p1.noteCommande+","+p1.liste_plats+","+p1.IdUser+")";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }

        /// <summary>
        /// Méthode permettant de modifier une commande dans la table 'Commande'
        /// </summary>
        /// <param name="p1"></param>

        public void ModifierCommande(Commande<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "UPDATE Commande SET Num_commande="+p1.numeroCommande+", Prix_commande="+ p1.prixCommande+", Note_commande="+p1.noteCommande+", liste_plats="+p1.liste_plats+", WHERE Num_commande="+p1.numeroCommande+";";
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