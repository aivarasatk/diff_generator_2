using DiffGenerator2.Constants;
using DiffGenerator2.DTOs;
using DiffGenerator2.Interfaces;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DiffGenerator2.Services
{
    public class ExcelReader : IExcelReader
    {
        private ExcelPackage _excelPackage;
        public SheetNavigation GetSheetNavigation(string sheetName)
        {
            var worksheet = _excelPackage.Workbook.Worksheets.FirstOrDefault(sheet => sheet.Name == sheetName);
            var start = worksheet.Dimension.Start;
            var end = worksheet.Dimension.End;
            return null;
        }

        public IEnumerable<string> GetAvailableSheetNames(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            _excelPackage = new ExcelPackage(fileInfo);

            var workbook = _excelPackage.Workbook;
            foreach(var workSheet in workbook.Worksheets)
            {
                yield return workSheet.Name;
            }
        }
    }
}
