using DiffGenerator2.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Interfaces
{
    public interface IDiffGenerator
    {
        DiffReport GenerateDiffReport(IList<I07> eipData, IList<ExcelBlockData> excelData, IList<SheetCheckBoxItem> checkMonthOnlySheets);
    }
}
