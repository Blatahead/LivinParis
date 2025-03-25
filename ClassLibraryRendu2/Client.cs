using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Client<T>: Utilisateur<T>
    {
        int idClient;

        #region constructeur
        public Client(int idUser, string mdp, string adresse_mail,int idClient): base(idUser,mdp,adresse_mail)
        {
            this.idClient=idClient;
        }
        #endregion
        #region propriétés
        public int IdClient
        {
            set { idClient=value; }
        }
        #endregion

    }
}
