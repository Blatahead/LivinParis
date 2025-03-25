using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Particulier<T>
    {
        int numeroParticulier;
        string prenomParticulier;
        string nomParticulier;
        string adresseParticulier;
        #region constructeur
        public Particulier(int numeroParticulier, string prenomParticulier, string nomParticulier, string adresseParticulier)
        {
            this.numeroParticulier = numeroParticulier;
            this.prenomParticulier= prenomParticulier;
            this.nomParticulier= nomParticulier;
            this.adresseParticulier=adresseParticulier;
        }
        #endregion 
        #region propriétés
        public int NumeroParticulier
        {
            set { numeroParticulier=value; }
        }
        public string PrenomParticulier
        {
            set { prenomParticulier=value; }
        }
        public string NomParticulier
        {
            set { nomParticulier=value; }
        }
        public string AdresseParticulier
        {
            set { adresseParticulier=value; }
        }
        #endregion
    }
}
