using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.DTOs
{
    public class OrderReport
    {
        public IEnumerable<int> BelowLowerBound { get; set; }
        public IEnumerable<int> MissingInInterval { get; set; }
        public IEnumerable<int> AboveUpperBound { get; set; }
    }
}
