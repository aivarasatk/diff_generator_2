using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.DTOs
{
    public class SheetNavigation
    {
        public int BlockHeaderRow { get; set; }
        public int KodasColumn { get; set; }
        public int PavadinimasColumn { get; set; }
        public int PadalinysColumn { get; set; }
        public int DataStartRow{ get; set; }
    }
}
