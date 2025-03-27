using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Plat<T>
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
        public Plat(int numPlat, string nomPlat, int ndpPlat, string typePlat, string nationalitePlat, string datePeremption, float prixPlat, string ingredients, string regimeAlimentaire,string photo,string dateFabrication)
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
        public void CreerPlat(Plat<T> p1)
        {
            ConnexionDB.ConnectToDatabase();
            string demande = "INSERT INTO Plat (Num_plat, Nom_plat, Nombre_de_personne_plat, Type_plat, Nationalite_plat, Date_peremption_plat, prix_plat, Ingredients_plat, Regime_alimentaire_plat, Photo_plat, Dae_fabrication_plat) VALUES ("+p1.numPlat+","+p1.nomPlat+","+p1.ndpPlat+","+p1.typePlat+","+p1.nationalitePlat+","+p1.datePeremption+","+p1.prixPlat+","+p1.ingredients+","+p1.regimeAlimentaire+","+p1.photo+","+p1.dateFabrication+")";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }
        public void ModifierPlat(Plat<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "UPDATE SET Plat Num_plat="+p1.numPlat+", Nom_plat="+p1.nomPlat+", Nombre_de_personne_plat="+p1.ndpPlat+", Type_plat, Nationalite_plat="+p1.nationalitePlat+", Date_peremption_plat="+p1.datePeremption+", prix_plat="+p1.prixPlat+", Ingredients_plat="+p1.ingredients+", Regime_alimentaire_plat="+p1.regimeAlimentaire+", Photo_plat="+p1.photo+", Date_fabrication_plat="+p1.dateFabrication+" WHERE Num_plat="+p1.numPlat+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }

        public void DeletePlat(Plat<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "DELETE FROM Plat WHERE Num_plat="+p1.numPlat+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }










    }
}
