using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.DTOs
{
    public class ExcelBlockData
    {
        public DateTime Date { get; set; }
        public IEnumerable<ExcelProductData> ProductData { get; set; }
    }
}
