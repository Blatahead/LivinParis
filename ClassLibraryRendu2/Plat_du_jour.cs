using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Plat_du_Jour<T>: Cuisinier<T>
    {
        int numPlatJ;
        string nomPlatJ;
        int ndpPlatJ;
        string typePlatJ;
        string nationalitePlatJ;
        string datePeremptionJ;
        float prixPlatJ;
        string ingredientsJ;
        string regimeAlimentaireJ;
        string photoJ;
        string dateFabricationJ;

        #region propriétés
        public Plat_du_Jour(int id_Cuisinier, int idUser, string mdp, string adresse_mail, string nomCuisinier, string prenomCuisinier, string adresseCuisinier, List<T> liste_commandes, List<T> liste_commandes_pretes, List<T> liste_commandes_livrees, List<T> liste_de_plats,int numPlat, string nomPlat, int ndpPlat, string typePlat, string nationalitePlat, string datePeremption, float prixPlat, string ingredients, string regimeAlimentaire, string photo, string dateFabrication) : base(id_Cuisinier, idUser, mdp, adresse_mail, nomCuisinier, prenomCuisinier, adresseCuisinier, liste_commandes, liste_commandes_pretes, liste_commandes_livrees, liste_de_plats)
        {
            this.numPlatJ = numPlat;
            this.nomPlatJ = nomPlat;
            this.ndpPlatJ = ndpPlat;
            this.typePlatJ = typePlat;
            this.nationalitePlatJ = nationalitePlat;
            this.datePeremptionJ = datePeremption;
            this.prixPlatJ= prixPlat;
            this.ingredientsJ = ingredients;
            this.regimeAlimentaireJ = regimeAlimentaire;
            this.photoJ = photo;
            this.dateFabricationJ = dateFabrication;
        }
        #endregion
        #region propriétés
        public int NumPlatJ
        {
            get { return numPlatJ; }
            set { numPlatJ = value; }
        }
        public string NomPlatJ
        { get { return nomPlatJ; } }
        public string TypePlat { get { return typePlatJ; } }
        public string NationalitePlatJ { get { return nationalitePlatJ; } }
        public string DatePeremptionJ { get { return datePeremptionJ; } }
        public string DateFabricationJ { get { return dateFabricationJ; } }
        public float PrixPlatJ
        { get { return prixPlatJ; } }
        public string IngredientsJ
        {
            get { return ingredientsJ; }
        }
        public string RegimeAlimentaireJ { get { return regimeAlimentaireJ; } }
        public string PhotoJ
        { get { return photoJ; } }

        public int NdpPlatJ
        { get { return ndpPlatJ; } }
        #endregion

        #region Méthodes

        /// <summary>
        /// Méthode permettant de créer un plat dans la table 'Plat'
        /// </summary>
        /// <param name="p1"></param>
        public void CreerPlat(Plat_du_Jour<T> p1)
        {
            ConnexionDB.ConnectToDatabase();
            string demande = "INSERT INTO Plat (Num_plat, Nom_plat, Nombre_de_personne_plat, Type_plat, Nationalite_plat, Date_peremption_plat, prix_plat, Ingredients_plat, Regime_alimentaire_plat, Photo_plat, Date_fabrication_plat, Id_Cuisinier) VALUES ("+p1.numPlatJ+","+p1.nomPlatJ+","+p1.ndpPlatJ+","+p1.typePlatJ+","+p1.nationalitePlatJ+","+p1.datePeremptionJ+","+p1.prixPlatJ+","+p1.ingredientsJ+","+p1.regimeAlimentaireJ+","+p1.photoJ+","+p1.dateFabricationJ+","+p1.Id_Cuisinier+")";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }
        /// <summary>
        /// Méthode permettant de modifier un plat dans la table 'Plat'
        /// </summary>
        /// <param name="p1"></param>
        public void ModifierPlat(Plat_du_Jour<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "UPDATE Plat SET Num_plat="+p1.numPlatJ+", Nom_plat="+p1.nomPlatJ+", Nombre_de_personne_plat="+p1.ndpPlatJ+", Type_plat, Nationalite_plat="+p1.nationalitePlatJ+", Date_peremption_plat="+p1.datePeremptionJ+", prix_plat="+p1.prixPlatJ+", Ingredients_plat="+p1.ingredientsJ+", Regime_alimentaire_plat="+p1.regimeAlimentaireJ+", Photo_plat="+p1.photoJ+", Date_fabrication_plat="+p1.dateFabricationJ+" WHERE Num_plat="+p1.numPlatJ+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }
        /// <summary>
        /// Méthode supprimant un plat de la table 'Plat' en s'assurant d'abord que toutes les clés étrangères liées dans les autres tables soient préalablement supprimées
        /// </summary>
        /// <param name="p1"></param>
        public void DeletePlat(Plat_du_Jour<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "DELETE FROM Plat WHERE Num_plat="+p1.numPlatJ+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }
        #endregion








    }
}
