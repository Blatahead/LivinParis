namespace ClassLibraryRendu2
{
    public class Utilisateur<T>
    {
        string identifiant;
        string mdp;
        int numero;
        string adresse_mail;

        #region constructeur
        public Utilisateur(string identifiant, string mdp, int numero, string adresse_mail)
        {
            this.identifiant = identifiant;
            this.mdp=mdp;
            this.numero=numero;
            this.adresse_mail= adresse_mail;
        }
        #endregion
        #region propriétés
        public string Identifiant
        {
            get { return identifiant; }
            set { identifiant=value; }
        }
        public string Mdp
        {
            set { mdp=value; }
        }
        public int Numero
        {
            set { numero=value; }
        }
        public string Adresse_mail
        {
            set { adresse_mail=value; }
        }
        #endregion



    }
}
