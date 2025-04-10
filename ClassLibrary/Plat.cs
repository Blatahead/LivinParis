using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;

namespace ClassLibrary
{
    public class Plat<T>: Plat_du_Jour<T>
    {
        int numPlat;
        string nomPlat;
        int ndpPlat;
        string typePlat;
        string nationalitePlat;
        string datePeremption;
        float prixPlat;
        string ingredients;
        string regimeAlimentaire;
        string photo;
        string dateFabrication;


        #region constructeur
        public Plat(int id_Cuisinier, int idUser, string mdp, string adresse_mail, string nomCuisinier, string prenomCuisinier, string adresseCuisinier, List<T> liste_commandes, List<T> liste_commandes_pretes, List<T> liste_commandes_livrees, List<T> liste_de_plats, int numPlatJ, string nomPlatJ, int ndpPlatJ, string typePlatJ, string nationalitePlatJ, string datePeremptionJ, float prixPlatJ, string ingredientsJ, string regimeAlimentaireJ, string photoJ, string dateFabricationJ,  int numPlat, string nomPlat, int ndpPlat, string typePlat, string nationalitePlat, string datePeremption, float prixPlat, string ingredients, string regimeAlimentaire,string photo,string dateFabrication): base(id_Cuisinier, idUser, mdp, adresse_mail,nomCuisinier, prenomCuisinier, adresseCuisinier, liste_commandes,liste_commandes_pretes,liste_commandes_livrees, liste_de_plats, numPlatJ, nomPlatJ, ndpPlatJ, typePlatJ, nationalitePlatJ, datePeremptionJ, prixPlatJ, ingredientsJ, regimeAlimentaireJ, photoJ, dateFabricationJ)
        {
            this.numPlat = numPlat;
            this.nomPlat = nomPlat;
            this.ndpPlat = ndpPlat;
            this.typePlat = typePlat;
            this.nationalitePlat = nationalitePlat;
            this.datePeremption = datePeremption;
            this.prixPlat = prixPlat;
            this.ingredients = ingredients;
            this.regimeAlimentaire = regimeAlimentaire;
            this.photo = photo;
            this.dateFabrication = dateFabrication;
        }
        #endregion
        #region propriétés
        public int NumPlat
        {
            get { return numPlat; }
        }
        public string NomPlat
        { get { return nomPlat; } }
        public string TypePlat { get { return typePlat; } }
        public string NationalitePlat { get {return nationalitePlat; } }
        public string DatePeremption { get { return datePeremption; } }
        public string DateFabrication { get { return dateFabrication; } }   
        public float PrixPlat
        { get { return prixPlat; } }
        public string Ingredients
        {
            get { return ingredients; }
        }
        public string RegimeAlimentaire { get { return regimeAlimentaire; } }
        public string Photo
        { get { return photo; } }

        public int NdpPlat
        { get { return ndpPlat; } }
        #endregion

        #region Méthodes

        /// <summary>
        /// Méthode permettant de créer un plat dans la table 'Plat'
        /// </summary>
        /// <param name="p1"></param>
        public void CreerPlat(Plat<T> p1)
        {
            ConnexionDB.ConnectToDatabase();
            string demande = "INSERT INTO Plat (Num_plat, Nom_plat, Nombre_de_personne_plat, Type_plat, Nationalite_plat, Date_peremption_plat, prix_plat, Ingredients_plat, Regime_alimentaire_plat, Photo_plat, Date_fabrication_plat, Num_platJ, Id_Cuisinier) VALUES ("+p1.numPlat+","+p1.nomPlat+","+p1.ndpPlat+","+p1.typePlat+","+p1.nationalitePlat+","+p1.datePeremption+","+p1.prixPlat+","+p1.ingredients+","+p1.regimeAlimentaire+","+p1.photo+","+p1.dateFabrication+","+p1.NumPlatJ+","+p1.Id_Cuisinier+")";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }
        /// <summary>
        /// Méthode permettant de modifier un plat dans la table 'Plat'
        /// </summary>
        /// <param name="p1"></param>
        public void ModifierPlat(Plat<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "UPDATE SET Plat Num_plat="+p1.numPlat+", Nom_plat="+p1.nomPlat+", Nombre_de_personne_plat="+p1.ndpPlat+", Type_plat, Nationalite_plat="+p1.nationalitePlat+", Date_peremption_plat="+p1.datePeremption+", prix_plat="+p1.prixPlat+", Ingredients_plat="+p1.ingredients+", Regime_alimentaire_plat="+p1.regimeAlimentaire+", Photo_plat="+p1.photo+", Date_fabrication_plat="+p1.dateFabrication+" WHERE Num_plat="+p1.numPlat+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }
        /// <summary>
        /// Méthode supprimant un plat de la table 'Plat' en s'assurant d'abord que toutes les clés étrangères liées dans les autres tables soient préalablement supprimées
        /// </summary>
        /// <param name="p1"></param>
        public void DeletePlat(Plat<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "DELETE FROM Plat WHERE Num_plat="+p1.numPlat+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }
        #endregion
    }
}