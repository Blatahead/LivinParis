using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

  

namespace ClassLibraryRendu2
{
    public class Utilisateur<T>
    {
        int idUser;
        string mdp;
        string adresse_mail;

        #region constructeur
        public Utilisateur(int idUser, string mdp, string adresse_mail)
        {
            this.idUser = idUser;   
            this.mdp=mdp;
            this.adresse_mail= adresse_mail;
        }
        #endregion
        #region propriétés
        public int IdUser
        {
            get { return idUser; }
            set { idUser=value; }
        }
        public string Mdp
        {
            set { mdp=value; }
        }
        public string Adresse_mail
        {
            set { adresse_mail=value; }
        }
        #endregion

        public void CreerUser(Utilisateur<T> p1)
        {
            this.idUser=p1.idUser;
            this.mdp=p1.mdp;
            this.adresse_mail = p1.adresse_mail;

        }

        public void ModifierUser(Utilisateur<T> p1)
        {
            if (this.idUser!=p1.idUser)
            {
                this.idUser=p1.idUser;
            }
            if (this.mdp!=p1.mdp)
            {
                this.mdp=p1.mdp;
            }
            if(this.adresse_mail!=p1.adresse_mail)
            {
                this.adresse_mail=p1.adresse_mail;
            }
        }

        public void DeleteUser(Utilisateur<T> p1)
        {
            if (p1.idUser==this.idUser && this.adresse_mail==p1.adresse_mail)
            {
               
                

            }
        }

    }
}
