using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Plat
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


    }
}
