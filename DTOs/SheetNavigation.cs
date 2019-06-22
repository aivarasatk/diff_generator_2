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
        public int CodeColumn { get; set; }
        public int NameColumn { get; set; }
        public int MakerColumn { get; set; }
        public int DataStartRow{ get; set; }
    }
}
