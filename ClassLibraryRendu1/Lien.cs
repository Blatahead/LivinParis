using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu1
{
    public class Lien
    {
        Noeud source;
        Noeud destination;
        public Noeud Source { get; set; }
        public Noeud Destination { get; set; }

        public Lien(Noeud source1, Noeud destination1)
        {
            this.source = source1;
            this.destination = destination1;
        }
    }
}
