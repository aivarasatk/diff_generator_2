using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Constants
{
    public static class AllowedAmountPostfixes
    {
        public static readonly List<string> Values = new List<string>
        {
            "l", "L", "tab.", "kap.", "cap."
        };
    }
}
