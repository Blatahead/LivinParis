using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Cuisinier<T>:Utilisateur<T>
    {
        string nomCuisinier;
        string prenomCuisinier;
        int numeroCuisinier;
        string adresseCuisinier;

        #region constructeur
        public Cuisinier(string identifiant, string mdp, int numero, string adresse_mail, string nomCuisinier, string prenomCuisinier, int numeroCuisinier, string adresseCuisinier): base(identifiant, mdp, numero, adresse_mail)
        {
            this.nomCuisinier=nomCuisinier;
            this.prenomCuisinier=nomCuisinier;
            this.numeroCuisinier=numeroCuisinier;
            this.adresseCuisinier=adresseCuisinier;

        }
        #endregion
        #region propriétés
        public string NomCuisinier
        {
            get { return nomCuisinier; }
        }
        public string PrenomCuisinier
        {
            get { return prenomCuisinier; }
        }
        public int NumeroCuisinier
        {
            set { numeroCuisinier=value; }
        }
        public string AdresseCuisinier
        {
            set { adresseCuisinier=value; }
        }
        #endregion

    }
}
