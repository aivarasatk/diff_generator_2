using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.DTOs
{
    public class Mismatch
    {
        public string SheetName { get; set; }
        public DateTime BlockDate { get; set; }
        public I07 EipData { get; set; }
        public ExcelProductData ExcelData { get; set; }

    }
}
