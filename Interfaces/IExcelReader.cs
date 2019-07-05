using DiffGenerator2.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Interfaces
{
    public interface IExcelReader
    {
        IEnumerable<string> GetAvailableSheetNames(string fileName);
        IEnumerable<ExcelBlockData> GetExcelProductData(string fileName, IEnumerable<SheetCheckBoxItem> sheetItems);
    }
}
