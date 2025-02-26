using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu1
{
    public class Lien
    {
        #region Attributs
        Noeud source;
        Noeud destination;
        #endregion

        #region Proprietes
        public Noeud Source 
        {
            get { return this.source;}
            set { this.source = value;} 
        }
        public Noeud Destination
        {
            get { return this.destination;}
            set { this.destination = value;}
        }
        #endregion

        #region Constructeur
        public Lien(Noeud source1, Noeud destination1)
        {
            this.source = source1;
            this.destination = destination1;
        }
        #endregion
    }
}