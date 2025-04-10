using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;

namespace ClassLibrary
{
    public class DistanceComparer : IComparer<(double distance, int id)> { 
        public int Compare((double distance, int id) x, (double distance, int id) y) { 
            int comp = x.distance.CompareTo(y.distance); 
            if (comp == 0) 
                return x.id.CompareTo(y.id); 
            return comp; 
        } 
    }
}