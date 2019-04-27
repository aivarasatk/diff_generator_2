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
        public SheetNavigation GetExcelSheetNavigation(string fileName)
        {
            using (var excelWorkbook = new ExcelPackage(new FileInfo(fileName)).Workbook)
            {
                throw new NotImplementedException();
            }
        }
    }
}
