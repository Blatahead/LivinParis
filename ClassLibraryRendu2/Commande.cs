using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class Commande<T>
    {
        int numeroCommande;
        float prixCommande;
        int noteCommande;
        #region constructeur
        public Commande(int numeroCommande, int prixCommande, int noteCommande)
        {
            this.numeroCommande = numeroCommande;
            this.prixCommande = prixCommande;
            this.noteCommande = noteCommande;
        }
        #endregion
        #region propriétés
        public int NumeroCommande
        {
            get { return numeroCommande; }
        }
        public float PrixCommande
        {
            get { return prixCommande; }
        }
        public int NoteCommande
        {
            get { return noteCommande; }
            set { noteCommande=value; }
        }
        #endregion

        public void CreerCommande(Commande<T> p1)
        {
            ConnexionDB.ConnectToDatabase();
            string demande = "INSERT INTO Commande (Num_commande, Prix_commande, Note_commande) VALUES ("+p1.numeroCommande+","+p1.prixCommande+","+p1.noteCommande+")";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }

        public void ModifierCommande(Commande<T> p1)
        {

            ConnexionDB.ConnectToDatabase();
            string demande = "UPDATE SET Commande Num_commande="+p1.numeroCommande+", Prix_commande="+ p1.prixCommande+", Note_commande="+p1.noteCommande+" WHERE Num_commande="+p1.numeroCommande+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;

        }

        public void DeleteCommande(Commande<T> p1)
        { 

            ConnexionDB.ConnectToDatabase();
            string demande = "DELETE FROM Commande WHERE ="+p1.numeroCommande+";";
            using (MySqlCommand cmd = new MySqlCommand(demande)) ;


        }










    }
}
