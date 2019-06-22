using DiffGenerator2.Constants;
using DiffGenerator2.DTOs;
using DiffGenerator2.Interfaces;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public IEnumerable<string> GetAvailableSheetNames(string fileName)//TODO: ASYNC/AWAIT. this can take some time and blocks UI
        {
            var fileInfo = new FileInfo(fileName);
            _excelPackage = new ExcelPackage(fileInfo);

            var workbook = _excelPackage.Workbook;
            foreach (var workSheet in workbook.Worksheets)
            {
                yield return workSheet.Name;
            }
        }

        public IEnumerable<ExcelBlockData> GetExcelProductData(IEnumerable<SheetCheckBoxItem> selectedSheets)
        {
            if(_excelPackage == null)
            {
                throw new ArgumentNullException(nameof(_excelPackage));
            }

            var sheetNavigationDictionary = GetSheetNavigations(selectedSheets);
            foreach (var sheetNavigation in sheetNavigationDictionary)
            {
                _logService.Information($"Getting which blocks to read for {sheetNavigation.Key}");
                var blockDataToRead = GetBlocksToRead(sheetNavigation).ToList();

            }
            
            return null;
        }

        private IDictionary<string, SheetNavigation> GetSheetNavigations(IEnumerable<SheetCheckBoxItem> sheetItems)
        {
            var sheetDictionary = new Dictionary<string, SheetNavigation>();
            foreach (var sheetItem in sheetItems)
            {
                _logService.Information($"Getting sheet navigation for {sheetItem.Name}");
                var worksheet = _excelPackage.Workbook.Worksheets.FirstOrDefault(sheet => sheet.Name == sheetItem.Name);
                var sheetNavigation = SheetNavigation(worksheet);

                if (sheetNavigation == null)
                {
                    throw new Exception($"Could not read sheet navigation for { sheetItem.Name}");
                }
                sheetDictionary.Add(sheetItem.Name, sheetNavigation);
            }
            return sheetDictionary;

        }

        private SheetNavigation SheetNavigation(ExcelWorksheet worksheet)
        {
            var navigationRowId = GetNavigationRowId();
            var sheetNavigation = new SheetNavigation();
            for (var columnId = 1; columnId <= worksheet.Dimension.Columns; ++columnId)
            {
                if (AllSheetNavigationFieldsSet(sheetNavigation))
                {
                    return sheetNavigation;
                }

                var value = worksheet.GetValue<string>(navigationRowId, columnId);
                if (value == null)
                {
                    continue;
                }

                if (value == SheetNavigationIdentifiers.GamybosPadalinys)
                {
                    sheetNavigation.MakerColumn = columnId;
                }
                if (value.Contains(SheetNavigationIdentifiers.Kodas))
                {
                    sheetNavigation.CodeColumn = columnId;
                    sheetNavigation = SheetNavigationWithDataAndHeaderIds(sheetNavigation, value);
                }
                if (value == SheetNavigationIdentifiers.PreparatoPavadinimas)
                {
                    sheetNavigation.NameColumn = columnId;
                }
            }
            return null;
        }

        private int GetNavigationRowId()
        {
            if (!Int32.TryParse(ConfigurationManager.AppSettings["SheetNavigationDataRow"], out var navigationRowId))
            {
                _logService.Error("App config 'SheetNavigationDataRow' is not a numeric value");
                throw new ArgumentException("App config 'SheetNavigationDataRow' is not a numeric value");
            }
            return navigationRowId;
        }

        private bool AllSheetNavigationFieldsSet(SheetNavigation sheetNavigation)
        {
            return sheetNavigation.MakerColumn != 0
                   && sheetNavigation.NameColumn != 0
                   && sheetNavigation.CodeColumn != 0;
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
            catch (Exception ex)
            {
                _logService.Error("Could not convert Kodas numeric values", ex);
                throw;
            }
        }

        private IEnumerable<BlockDataColumns> GetBlocksToRead(KeyValuePair<string, SheetNavigation> sheetNavigation)
        {
            var workbook = _excelPackage.Workbook.Worksheets.First(sh => sh.Name == sheetNavigation.Key);
            var navigationRowId = GetNavigationRowId();
            var blockStartColumn = GetBlockStartColumn(workbook, navigationRowId);

            var blockDataColumns = new BlockDataColumns();
            for (int i = blockStartColumn; i < workbook.Dimension.Columns; ++i)
            {
                var value = workbook.GetValue<string>(navigationRowId, i);
                if(value == null)
                {
                    continue;
                }

                if (AllBlockDataColumnsSet(blockDataColumns))
                {
                    yield return blockDataColumns;
                    blockDataColumns = new BlockDataColumns();
                }

                //if END was found but blockDataColumns contains some valid values - row is malformed
                if(value.ToLower() == ExcelDataBlockColumnNaming.EndLowerCase)
                {
                    if(AllBlockDataColumnsUnset(blockDataColumns))
                    {
                        yield break;
                    }
                    throw new Exception($"Malformed row that identifies which blocks to read for {workbook.Name}");
                }

                if (value == ExcelDataBlockColumnNaming.AmountFirstHalf)
                {
                    blockDataColumns.AmountFirstHalf = i;
                }
                else if (value == ExcelDataBlockColumnNaming.AmountSecondHalf)
                {
                    blockDataColumns.AmountSecondHalf = i;
                }
                else if (value == ExcelDataBlockColumnNaming.Date)
                {
                    blockDataColumns.Date = i;
                }
                else if (value == ExcelDataBlockColumnNaming.Comments)
                {
                    blockDataColumns.Comments = i;
                }
            }
            throw new Exception($"Malformed row that identifies which blocks to read for {workbook.Name}");
        }

        private bool AllBlockDataColumnsSet(BlockDataColumns blockDataColumns)
        {
            return blockDataColumns.AmountFirstHalf != 0
                && blockDataColumns.AmountSecondHalf != 0
                && blockDataColumns.Date != 0
                && blockDataColumns.Comments != 0;
        }

        private bool AllBlockDataColumnsUnset(BlockDataColumns blockDataColumns)
        {
            return blockDataColumns.AmountFirstHalf == 0
                && blockDataColumns.AmountSecondHalf == 0
                && blockDataColumns.Date == 0
                && blockDataColumns.Comments == 0;
        }

        private int GetBlockStartColumn(ExcelWorksheet workbook, int navigationRowId)
        {
            for(var i = 1; i < workbook.Dimension.Columns; ++i)
            {
                var cellValue = workbook.GetValue<string>(navigationRowId, i);
                if(cellValue != null && CellValueContainsBlockColumnNaming(cellValue.Trim()))
                {
                    return i;
                }
            }
            throw new Exception($"Could not find blocks to read for {workbook.Name}");
        }

        private bool CellValueContainsBlockColumnNaming(string cellValue)
        {
            return cellValue == ExcelDataBlockColumnNaming.AmountFirstHalf
                || cellValue == ExcelDataBlockColumnNaming.AmountSecondHalf
                || cellValue == ExcelDataBlockColumnNaming.Date
                || cellValue == ExcelDataBlockColumnNaming.Comments;
        }
    }
}
