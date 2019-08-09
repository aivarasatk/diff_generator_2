using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Constants
{
    public static class ExcludedColors
    {
        public static readonly List<string> Colors = new List<string>
        {
            "FFF0F0F0"//light gray
        };

        public static readonly List<string> MarkedInExcelAsDone = new List<string>
        {
            "FF008000"//green
        };
    }
}
