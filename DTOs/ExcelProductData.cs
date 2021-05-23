using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.DTOs
{
    public class ExcelProductData
    {
        public string Maker { get; set; } //Gamybos padalinys
        public string Code { get; set; }  //Kodas
        public string Name { get; set; }  //Preparato pavadinimas
        public int AmountFirstHalf { get; set; } //I Puse
        public int AmountSecondHalf { get; set; } //II Puse 
        public DateTime Date { get; set; } //Data
        public string Details { get; set; } //Pastabos
        public int? OrderNumber { get; set; } // Number extracted from Details
        public IEnumerable<string> Comments { get; set; } //langeliu komentarai
        public bool HasShapes { get; set; }
        public IEnumerable<string> CellBackgroundColors { get; set; }
    }
}
