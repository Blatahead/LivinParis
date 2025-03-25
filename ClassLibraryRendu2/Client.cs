using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Client<T>: Utilisateur<T>
    {
        int numeroClient;

        #region constructeur
        public Client(string identifiant, string mdp, int numero, string adresse_mail,int numeroClient): base(identifiant,mdp,numero,adresse_mail)
        {
            this.numeroClient=numeroClient;
        }
        #endregion
        #region propriétés
        public int NumeroClient
        {
            set { numeroClient=value; }
        }
        #endregion

    }
}
