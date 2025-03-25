using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Entreprise<T>
    {
        int numeroSiret;
        string nomEntreprise;
        string nomReferent;
        string adresseEntreprise;

        #region constructeur
        public Entreprise(int numeroSiret, string nomEntreprise, string nomReferent, string adresseEntreprise)
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
        }
        #endregion






    }
}
