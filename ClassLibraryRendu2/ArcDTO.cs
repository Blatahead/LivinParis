using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRendu2
{
    public class ArcDTO
    {
        public int SourceId { get; set; }
        public int DestinationId { get; set; }
        public double Distance { get; set; }
        public string Ligne { get; set; }

        public double SourceLat { get; set; }
        public double DestLat { get; set; }
        public double SourceLong { get; set; }
        public double DestLong { get; set; }
    }

}
