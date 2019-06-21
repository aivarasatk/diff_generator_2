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
        SheetNavigation GetSheetNavigation(string sheetName);
        IEnumerable<string> GetAvailableSheetNames(string fileName);
        IEnumerable<ExcelProductData> GetExcelProductData();

    }
}
