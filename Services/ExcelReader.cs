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
using System.Configuration;

namespace DiffGenerator2.Services
{
    public class ExcelReader : IExcelReader
    {
        private ExcelPackage _excelPackage;
        private ILogService _logService;
        public ExcelReader(ILogService logService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        public SheetNavigation GetSheetNavigation(string sheetName)
        {
            if(_excelPackage == null)
            {
                throw new ArgumentNullException(nameof(_excelPackage));
            }

            if (sheetName == null)
            {
                _logService.Error("Sheet name is null. Cannot get SheetNavigation");
                throw new ArgumentNullException(nameof(sheetName));
            }
            try
            {
                var worksheet = _excelPackage.Workbook.Worksheets.FirstOrDefault(sheet => sheet.Name == sheetName);
                return SheetNavigation(worksheet);
            }
            catch (Exception ex)
            {
                _logService.Error("Error while getting SheetNavigation ", ex);
                throw;
            }
            
        }

        private SheetNavigation SheetNavigation(ExcelWorksheet worksheet)
        {
            if(!Int32.TryParse(ConfigurationManager.AppSettings["SheetNavigationDataRow"], out var navigationRowId))
            {
                _logService.Error("App config 'SheetNavigationDataRow' is not a numeric value");
                throw new ArgumentException("App config 'SheetNavigationDataRow' is not a numeric value");
            }
            var sheetNavigation = new SheetNavigation();
            for(var columnId = 1; columnId <= worksheet.Dimension.Columns; ++columnId)
            {
                if (sheetNavigation.PadalinysColumn != 0 && sheetNavigation.PavadinimasColumn != 0
                    && sheetNavigation.KodasColumn != 0)
                {
                    return sheetNavigation;
                }
                var value = worksheet.GetValue<string>(navigationRowId, columnId);
                if(value == null)
                {
                    continue;
                }
                if (value == SheetNavigationIdentifiers.GamybosPadalinys)
                {
                    sheetNavigation.PadalinysColumn = columnId;
                }
                if(value.Contains(SheetNavigationIdentifiers.Kodas))
                {
                    sheetNavigation.KodasColumn = columnId;
                    sheetNavigation = SheetNavigationWithDataAndHeaderIds(sheetNavigation, value);
                }
                if(value == SheetNavigationIdentifiers.PreparatoPavadinimas)
                {
                    sheetNavigation.PavadinimasColumn = columnId;
                }
            }
            return null;
        }

        private SheetNavigation SheetNavigationWithDataAndHeaderIds(SheetNavigation sheetNavigation, string excelValue)
        {
            var splitValues = excelValue.Split('-');
            try
            {
                sheetNavigation.BlockHeaderRow = Int32.Parse(splitValues[1]);
                sheetNavigation.DataStartRow = Int32.Parse(splitValues[2]);
                return sheetNavigation;
            }
            catch(Exception ex)
            {
                _logService.Error("Could not convert Kodas numeric values", ex);
                throw;
            }
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

        public IEnumerable<ExcelProductData> GetExcelProductData()
        {
            throw new NotImplementedException();
        }
    }
}
