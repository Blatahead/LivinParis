﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;

namespace ClassLibrary
{
    public class StationDTO
    {
        #region Proprietes
        public int Id { get; set; }
        public string Nom { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Ligne { get; set; }
        public int CodePostal { get; set; }
        public int TempsVersStation { get; set; }
        public int Depart { get; set; }
        public int Arrivee { get; set; }
        public int Sens { get; set; }
        public double Distance { get; set; }
        #endregion
    }
}