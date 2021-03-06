﻿using DiffGenerator2.Constants;
using DiffGenerator2.DTOs;
using DiffGenerator2.Interfaces;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;
using OfficeOpenXml.Drawing;
using System.Data;

namespace DiffGenerator2.Services
{
    public class ExcelReader : IExcelReader
    {
        private static ExcelPackage _excelPackage;
        private ILogService _logService;
        public ExcelReader(ILogService logService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        public IEnumerable<string> GetAvailableSheetNames(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            _excelPackage = new ExcelPackage(fileInfo);

            var workbook = _excelPackage.Workbook;
            foreach (var workSheet in workbook.Worksheets)
            {
                yield return workSheet.Name;
            }
        }

        public IEnumerable<ExcelBlockData> GetExcelProductData(string fileName, IEnumerable<SheetCheckBoxItem> selectedSheets)
        {
            _excelPackage = GetExcelPackage(fileName);

            _logService.Information("Getting excel product data");

            var sheetNavigationDictionary = GetSheetNavigations(selectedSheets);
            foreach (var sheetNavigation in sheetNavigationDictionary)
            {
                _logService.Information($"Getting which blocks to read for {sheetNavigation.Key}");
                var blocksToRead = GetBlocksToRead(sheetNavigation);

                _logService.Information($"Starting to read product data");
                foreach(var blockData in blocksToRead)
                {
                    yield return GetBlockData(blockData, sheetNavigation);
                }
            }
        }

        private ExcelPackage GetExcelPackage(string fileName)
        {
            if (_excelPackage == null)
            {
                var fileInfo = new FileInfo(fileName);
                return new ExcelPackage(fileInfo);
            }
            return _excelPackage;
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
                sheetNavigation.BlockHeaderRow = int.Parse(splitValues[1]);
                sheetNavigation.DataStartRow = int.Parse(splitValues[2]);
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
            for (int i = blockStartColumn; i <= workbook.Dimension.Columns; ++i)
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
                    throw new Exception($"Malformed row that identifies which blocks to read for {workbook.Name}." +
                                        $" Found END token before all header columns had their column identifiers");
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
            throw new Exception($"Malformed row that identifies which blocks to read for {workbook.Name} - did not find END token");
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

        private ExcelBlockData GetBlockData(BlockDataColumns blockDataColumns, KeyValuePair<string, SheetNavigation> sheetNavigation)
        {
            var sheet = _excelPackage.Workbook.Worksheets.First(sh => sh.Name == sheetNavigation.Key);
            var blockHeader = GetBlockHeader(sheet, sheetNavigation.Value, blockDataColumns);

            _logService.Information($"Parsing block header for '{blockHeader}'");
            var parasedBlockHeader = ExcelParser.ParsedBlockHeader(blockHeader);

            return new ExcelBlockData
            {
                SheetName = sheetNavigation.Key,
                Date = parasedBlockHeader,
                ProductData = GetProductDataForBlock(sheet, sheetNavigation.Value, blockDataColumns, parasedBlockHeader)
            };
        }

        private IEnumerable<ExcelProductData> GetProductDataForBlock(ExcelWorksheet sheet, 
            SheetNavigation sheetNavigation, BlockDataColumns blockDataColumns, DateTime blockDateHeader)
        {
            _logService.Information($"Getting product data for {blockDateHeader:yyyy-MM-dd}");
            for (var i = sheetNavigation.DataStartRow; i < sheet.Dimension.Rows; ++i)
            {
                var maker = sheet.GetValue<string>(i, sheetNavigation.MakerColumn);
                var code = sheet.GetValue<string>(i, sheetNavigation.CodeColumn);
                var name = sheet.GetValue<string>(i, sheetNavigation.NameColumn);

                if (EndOfData(new List<string> { maker, code, name }))
                    yield break;

                if (code is null)
                    continue;

                var amountFirstHalfCell = sheet.Cells[i, blockDataColumns.AmountFirstHalf];
                var amountSecondHalfCell = sheet.Cells[i, blockDataColumns.AmountSecondHalf];
                var dateCell = sheet.Cells[i, blockDataColumns.Date];
                var commentsCell = sheet.Cells[i, blockDataColumns.Comments];
                var details = commentsCell.GetValue<string>() ?? string.Empty;
                var dataCells = new List<ExcelRange> { amountFirstHalfCell, amountSecondHalfCell, dateCell, commentsCell };

                var allCellsEmpty = AllCellsEmpty(dataCells);
                var cellComments = GetCellComments(dataCells);
                var cellBackgroundColors = GetCellBackgroundColors(dataCells);
                var cellsHaveShapes = CellsHaveShapes(dataCells, sheet.Drawings);

                if (IsSkipableRow(allCellsEmpty, cellComments, cellBackgroundColors))
                    continue;

                //we care if maker or name is null ONLY if we DON'T skip the row
                if (maker is null)
                    throw new NoNullAllowedException($"'Gamybos padalinys' cannot be empty. {sheet.Name};{blockDateHeader:yyyy-MM-dd};{code}");

                if (name is null)
                    throw new NoNullAllowedException($"'Pavadinimas' cannot be empty. {sheet.Name};{blockDateHeader:yyyy-MM-dd};{code}");

                var amountFirstHalfText = amountFirstHalfCell.GetValue<string>();
                var amountSecondHalfText = amountSecondHalfCell.GetValue<string>();

                var amountFirstHalf = 0;
                var amountSecondHalf = 0;
                
                try
                {
                    amountFirstHalf = ExcelParser.GetAmountIntValue(amountFirstHalfText);
                    amountSecondHalf = ExcelParser.GetAmountIntValue(amountSecondHalfText);
                }
                catch(Exception ex)
                {
                    throw new FormatException($"Failed to parse amount for:{Environment.NewLine} {sheet.Name} {blockDateHeader:yyyy-MM-dd} {code} {name} {ex.Message}", ex);
                }

                yield return new ExcelProductData
                {
                    Maker = maker.Trim(),
                    Code = code.Trim(),
                    Name = name.Trim(),
                    AmountFirstHalf = amountFirstHalf,
                    AmountSecondHalf = amountSecondHalf,
                    Date = SetProductDate(amountFirstHalfCell, amountSecondHalfCell, blockDateHeader),
                    Details = details.Trim(),
                    OrderNumber = ExcelParser.ExtractOrderNumber(details.Trim()),
                    Comments = cellComments,
                    CellBackgroundColors = cellBackgroundColors,
                    HasShapes = cellsHaveShapes
                };
            }
        }

        private bool IsSkipableRow(bool allCellsEmpty, IEnumerable<string> cellComments, IEnumerable<string> cellBackgroundColors)
            => allCellsEmpty && !cellComments.Any() && !cellBackgroundColors.Any();

        private DateTime SetProductDate(ExcelRange amountFirstHalfCell, ExcelRange amountSecondHalfCell, DateTime headerDate)
        {
            if(amountFirstHalfCell.Value != null)
            {
                if(amountSecondHalfCell.Value != null)
                {
                    return new DateTime(headerDate.Year, headerDate.Month, 15);
                }
                else
                {
                    return new DateTime(headerDate.Year, headerDate.Month, 7);
                }
            }
            //if amountSecondHalf is not null 22th day is ok, if it is - 22th is still ok
            return new DateTime(headerDate.Year, headerDate.Month, 22);
        }

        private bool CellsHaveShapes(List<ExcelRange> dataCells, ExcelDrawings drawings)
        {
            foreach(var cell in dataCells)
            {
                if(drawings.Any(d => (d.From.Row == cell.Start.Row - 1 && d.From.Column == cell.Start.Column - 1)//for some reason Drawing index starts from 0
                                     || (d.To.Row == cell.Start.Row - 1 && d.To.Column == cell.Start.Column - 1)))
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerable<string> GetCellBackgroundColors(List<ExcelRange> dataCells)
        {
            foreach(var cell in dataCells)
            {
                if (!DefaultCellBackgroundColor(cell))
                {
                    yield return cell.Style.Fill.BackgroundColor.Rgb;
                }
            }
        }

        private IEnumerable<string> GetCellComments(List<ExcelRange> dataCells)
        {
            foreach(var cell in dataCells)
            {
                if(cell.Comment != null)
                {
                    yield return cell.Comment.Text;
                }
            }
        }

        private bool AllCellsEmpty(IEnumerable<ExcelRange> cells)
        {
            foreach(var cell in cells)
            {
                if(cell.Value != null)
                {
                    return false;
                }
            }
            return true;
        }

        private bool DefaultCellBackgroundColor(ExcelRange cell)
        {
            return cell.Style.Fill.BackgroundColor.Rgb == null
                || cell.Style.Fill.BackgroundColor.Rgb == string.Empty
                || ExcludedColors.Colors.Contains(cell.Style.Fill.BackgroundColor.Rgb);
        }

        private bool EndOfData(IEnumerable<string> data) => data.All(d => d == null);

        private string GetBlockHeader(ExcelWorksheet sheet, SheetNavigation value, BlockDataColumns blockData)
        {
            var amountFirstHalfValue = sheet.GetValue<string>(value.BlockHeaderRow, blockData.AmountFirstHalf);
            var amountSecondHalfValue = sheet.GetValue<string>(value.BlockHeaderRow, blockData.AmountSecondHalf);
            var dateValue = sheet.GetValue<string>(value.BlockHeaderRow, blockData.Date);
            var commentsValue = sheet.GetValue<string>(value.BlockHeaderRow, blockData.Comments);

            return amountFirstHalfValue ?? 
                   amountSecondHalfValue ??
                   dateValue ??
                   commentsValue ?? throw new Exception($"Could not find block header for {sheet.Name}. AmountFirstHalf column ID: {blockData.AmountFirstHalf}");
        }
    }
}
