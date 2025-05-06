using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Lien
    {
        #region Proprietes
        public Noeud Noeud1 { get; }
        public Noeud Noeud2 { get; }
        public string Libelle { get; }
        #endregion

        #region Constructeur
        public Lien(Noeud n1, Noeud n2, string libelle)
        {
            Noeud1 = n1;
            Noeud2 = n2;
            Libelle = libelle;
        }
        #endregion

        #region Methode
        public override string ToString() => $"{Noeud1} — {Libelle} — {Noeud2}";
        #endregion
    }
}