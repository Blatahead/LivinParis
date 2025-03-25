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
    }
}
