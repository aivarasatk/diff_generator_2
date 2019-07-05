using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Constants
{
    public static class ExcludedColors
    {
        private static List<string> _colors = new List<string>
        {
            "FFF0F0F0", "FFFFFF00"
        };

        public static List<string> Colors { get { return _colors; } }
    }
}
