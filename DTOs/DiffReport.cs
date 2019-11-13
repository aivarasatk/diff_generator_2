using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.DTOs
{
    public class DiffReport
    {
        public IEnumerable<Mismatch> Mismatches { get; set; }
        public IEnumerable<I07> ProductsMissingFromExcel { get; set; }
        public IEnumerable<ExcelProductData> ProductsMissingFromEip { get; set; }
        public IEnumerable<I07> ProductsOutOfSelectedRange { get; set; }

        public string CheckedRange { get; set; }

    }
}