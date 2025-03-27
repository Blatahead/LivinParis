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
        int numeroCuisinier;
        string adresseCuisinier;

        #region constructeur
        public Cuisinier(int id_Cuisinier, int idUser, string mdp, string adresse_mail, string nomCuisinier, string prenomCuisinier, int numeroCuisinier, string adresseCuisinier): base(idUser, mdp, adresse_mail)
        {
            this.id_Cuisinier = id_Cuisinier;
            this.nom=nomCuisinier;
            this.prenom=nomCuisinier;
            this.numeroCuisinier=numeroCuisinier;
            this.adresseCuisinier=adresseCuisinier;

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
