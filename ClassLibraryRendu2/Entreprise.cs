using MySql.Data.MySqlClient;
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

        public void CreerEntreprise(Entreprise<T> p1)
        {
            ConnexionDB.ConnectToDatabase();
            string demande = "INSERT INTO Entreprise (Num_siret,Nom_entreprise,Nom_referent,Adresse_entreprise,) VALUES ("+p1.numeroSiret+","+p1.nomEntreprise+","+p1.nomReferent+","+p1.adresseEntreprise+")";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }
        public void ModifierEntreprise(Entreprise<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "UPDATE SET Entreprise Num_siret="+p1.numeroSiret+", Nom_entreprise="+p1.nomEntreprise+", Nom_referent="+p1.nomReferent+" WHERE Adresse_entreprise="+p1.adresseEntreprise+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }

        public void DeleteEntreprise(Entreprise<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "DELETE FROM Entreprise WHERE Num_siret="+p1.numeroSiret+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }







    }
}
