using DiffGenerator2.Constants;
using DiffGenerator2.DTOs;
using DiffGenerator2.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Services
{
    public class ExcelReportGenerator : IExcelReportGenerator
    {
        public void GenerateReport(DiffReport diffReport)
        {
            var path = Path.Combine($"{Directory.GetCurrentDirectory()}\\Reports", $"report_{DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")}.xlsx");
            var fileInfo = new FileInfo(path);
            fileInfo.Directory.Create();//creating a dir if does not exist

            using (var excelPackage = new ExcelPackage(fileInfo))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("report");

                InitializeTableHeaders(worksheet);
                var lastRow = GenerateMismatchPart(worksheet, diffReport.Mismatches);
                SetFilterForTable(worksheet, lastRow - 1);//filtering only mismatches

                SetSeparator(worksheet.Cells[lastRow, ExcelReport.ColumnOffset, lastRow, ExcelReport.DataEndColumn -1]);
                SetMissingFromEipHeader(worksheet, ++lastRow, diffReport.CheckedRange);
                lastRow = GenerateMissingEipProductPart(worksheet, diffReport.ProductsMissingFromEip, ++lastRow);

                SetSeparator(worksheet.Cells[lastRow, ExcelReport.ColumnOffset, lastRow, ExcelReport.DataEndColumn -1]);
                SetEipHeader(worksheet, lastRow, ExcelReport.ProductsMissingFromExcel, diffReport.CheckedRange);
                lastRow = GenerateMissingExcelProductPart(worksheet, diffReport.ProductsMissingFromExcel, ++lastRow);

                SetSeparator(worksheet.Cells[++lastRow, ExcelReport.ColumnOffset, lastRow, ExcelReport.DataEndColumn - 1]);
                SetOutOfRangeEiplHeader(worksheet, lastRow, diffReport.CheckedRange);
                lastRow = GenerateOutOfRangeProductPart(worksheet, diffReport.ProductsOutOfSelectedRange, ++lastRow);

                excelPackage.Save();
            }
        }

        private void InitializeTableHeaders(ExcelWorksheet worksheet)
        {
            InitializeTable(worksheet, ExcelReport.ColumnOffset, ExcelReport.ExcelTable, 
                            ExcelReport.ExcelTableHeaders, ExcelReport.ExcelProductColumnSpan);
            InitializeTable(worksheet, ExcelReport.EipDataStartColumn, ExcelReport.EipTable,
                            ExcelReport.EipTableHeaders, ExcelReport.EipProductColumnSpan);
        }

        private void InitializeTable(ExcelWorksheet worksheet,int columnOffset, string tableName,
                                     IEnumerable<string> headerNames, int columnSpan)
        {
            //set table name
            var currRow = ExcelReport.HeaderStartRowOffset;
            worksheet.SetValue(currRow, columnOffset, tableName);
            var tableNameRange = worksheet.Cells[currRow, columnOffset, currRow, columnOffset + ExcelReport.TableNameColumnSpan - 1];
            tableNameRange.Style.Font.Bold = true;
            SetBackgroundColor(tableNameRange, ExcelReport.RowColoring);
            ++currRow;

            //set table headers
            var headerRange = worksheet.Cells[currRow, columnOffset, currRow, columnOffset + columnSpan - 1];
            headerRange.Style.Font.Bold = true;
            SetBorderAndBackgroundStyleForRange(headerRange, false);
            
            foreach (var name in headerNames)
            {
                worksheet.SetValue(currRow, columnOffset++, name);
            }
            headerRange.AutoFitColumns();
        }

        private void SetFilterForTable(ExcelWorksheet worksheet, int lastRow)
        {
            var rowAboveData = ExcelReport.DataStartRowOffset - 1;
            worksheet.Cells[rowAboveData, ExcelReport.ColumnOffset, lastRow, ExcelReport.DataEndColumn - 1]
                     .AutoFilter = true;
        }

        private int GenerateMismatchPart(ExcelWorksheet worksheet, IEnumerable<Mismatch> mismatches)
        {
            //must sort at least by sheet
            var groupedMismatchesBySheet = mismatches.OrderByDescending(m => m.SheetName).ThenBy(m => m.EipData.Name).GroupBy(m => m.SheetName);

            var addedRows = ExcelReport.DataStartRowOffset;
            foreach(var groupedMismatch in groupedMismatchesBySheet)
            {
                worksheet.SetValue(addedRows++, ExcelReport.ColumnOffset, groupedMismatch.Key);

                var columnSpan = 0;
                var potentialErrorCells = new List<Tuple<int, int>>();
                var potentialWarningCells = new List<Tuple<int, int>>();
                foreach (var item in groupedMismatch.AsEnumerable())
                {
                    var currColumn = ExcelReport.ColumnOffset;

                    worksheet.SetValue(addedRows, currColumn++, item.ExcelData.Maker);
                    potentialErrorCells.Add(item.ExcelData.Maker != item.EipData.Maker? CellLocation(addedRows, currColumn - 1) : null);

                    worksheet.SetValue(addedRows, currColumn++, item.ExcelData.Code);
                    potentialErrorCells.Add(item.ExcelData.Code != item.EipData.Code.First() ? CellLocation(addedRows, currColumn - 1) : null);

                    worksheet.SetValue(addedRows, currColumn++, item.ExcelData.Name);
                    potentialErrorCells.Add(item.ExcelData.Name != item.EipData.Name ? CellLocation(addedRows, currColumn - 1) : null);

                    worksheet.SetValue(addedRows, currColumn++, item.ExcelData.AmountFirstHalf + item.ExcelData.AmountSecondHalf);
                    var amountsNotEqual = item.ExcelData.AmountFirstHalf + item.ExcelData.AmountSecondHalf != item.EipData.Amount;
                    potentialErrorCells.Add(amountsNotEqual ? CellLocation(addedRows, currColumn - 1) : null);

                    worksheet.SetValue(addedRows, currColumn++, item.ExcelData.Date.ToString("yyyy-MM-dd"));
                    var datesDontMatch = item.ExcelData.Date.ToString("yyyy-MM-dd") != item.EipData.DateDateTime.ToString("yyyy-MM-dd");
                    potentialErrorCells.Add(datesDontMatch ? CellLocation(addedRows, currColumn - 1) : null);

                    worksheet.SetValue(addedRows, currColumn++, item.ExcelData.Details);

                    worksheet.SetValue(addedRows, currColumn++, item.ExcelData.HasShapes ? ExcelReport.HasShapes: ExcelReport.DoesNotHaveShapes);
                    potentialWarningCells.Add(item.ExcelData.HasShapes ? CellLocation(addedRows, currColumn - 1) : null);

                    worksheet.SetValue(addedRows, currColumn++, item.ExcelData.CellBackgroundColors.Any() ? ExcelReport.HasBgColors : ExcelReport.DoesNotHaveBgColors);
                    potentialWarningCells.Add(item.ExcelData.CellBackgroundColors.Any() ? CellLocation(addedRows, currColumn - 1) : null);

                    //style columns before separator
                    columnSpan = currColumn - 1 - ExcelReport.ColumnOffset;
                    SetBorderAndBackgroundStyleForRange(worksheet.Cells[addedRows, ExcelReport.ColumnOffset, addedRows, columnSpan], addedRows % 2 != 0);
                    ++currColumn;//skip one column

                    worksheet.SetValue(addedRows, currColumn++, item.EipData.Maker);
                    potentialErrorCells.Add(item.ExcelData.Maker != item.EipData.Maker ? CellLocation(addedRows, currColumn - 1) : null);

                    worksheet.SetValue(addedRows, currColumn++, item.EipData.Code.First());
                    potentialErrorCells.Add(item.ExcelData.Code != item.EipData.Code.First() ? CellLocation(addedRows, currColumn - 1) : null);

                    worksheet.SetValue(addedRows, currColumn++, item.EipData.Name);
                    potentialErrorCells.Add(item.ExcelData.Name != item.EipData.Name ? CellLocation(addedRows, currColumn - 1) : null);

                    worksheet.SetValue(addedRows, currColumn++, item.EipData.Amount);
                    amountsNotEqual = item.ExcelData.AmountFirstHalf + item.ExcelData.AmountSecondHalf != item.EipData.Amount;
                    potentialErrorCells.Add(amountsNotEqual ? CellLocation(addedRows, currColumn - 1) : null);

                    worksheet.SetValue(addedRows, currColumn++, item.EipData.DateDateTime.ToString("yyyy-MM-dd"));
                    datesDontMatch = item.ExcelData.Date.ToString("yyyy-MM-dd") != item.EipData.DateDateTime.ToString("yyyy-MM-dd");
                    potentialErrorCells.Add(datesDontMatch ? CellLocation(addedRows, currColumn - 1) : null);

                    worksheet.SetValue(addedRows, currColumn++, item.EipData.Details1);
                    worksheet.SetValue(addedRows, currColumn++, item.EipData.Details2);

                    columnSpan = currColumn;
                    SetBorderAndBackgroundStyleForRange(worksheet.Cells[addedRows, ExcelReport.ColumnOffset, addedRows, columnSpan], addedRows % 2 != 0);
                    ++addedRows;
                }

                var errorCells = potentialErrorCells.Where(e => e != null);
                var warningCells = potentialWarningCells.Where(cell => cell != null);

                SetMismatchColoring(worksheet, errorCells, ExcelReport.ErrorColoring);
                SetMismatchColoring(worksheet, warningCells, ExcelReport.WarningColoring);
                ++addedRows;
            }
            return addedRows;
        }

        private Tuple<int, int> CellLocation(int currRow, int currColumn)
        {
            return new Tuple<int, int>(currRow, currColumn);
        }

        private void SetMismatchColoring(ExcelWorksheet worksheet, IEnumerable<Tuple<int,int>> errorCells, Color coloring)
        {
            foreach(var error in errorCells)
            {
                var cell = worksheet.Cells[error.Item1, error.Item2];
                SetBackgroundColor(cell, coloring);
            }
        }

        private void SetBorderAndBackgroundStyleForRange(ExcelRange excelRange, bool setBackgroundColor)
        {
            foreach(var cell in excelRange)
            {
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                if (setBackgroundColor)
                {
                    SetBackgroundColor(cell, ExcelReport.RowColoring);
                }        
            }
        }

        private void SetSeparator(ExcelRange range)
        {
            SetBackgroundColor(range, ExcelReport.SeparatorColoring);
        }

        private void SetMissingFromEipHeader(ExcelWorksheet worksheet, int currenRow, string checkedRange)
        {
            SetSeparator(worksheet.Cells[currenRow, ExcelReport.ColumnOffset, currenRow, ExcelReport.ColumnOffset + ExcelReport.ExcelProductColumnSpan -1]);
            worksheet.SetValue(currenRow, ExcelReport.ColumnOffset, ExcelReport.ProductsMissingFromEip);
            worksheet.SetValue(currenRow, ExcelReport.ColumnOffset + 1, checkedRange);
            worksheet.Cells[currenRow, ExcelReport.ColumnOffset, currenRow, ExcelReport.ColumnOffset + 1].Style.Font.Bold = true;
        }

        private void SetBackgroundColor(ExcelRangeBase range, Color color)
        {
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(color);
        }

        private int GenerateMissingExcelProductPart(ExcelWorksheet worksheet, IEnumerable<I07> productsMissingFromExcel, int lastRow)
        {
            foreach (var item in productsMissingFromExcel)
            {
                var currColumn = ExcelReport.EipDataStartColumn;

                worksheet.SetValue(lastRow, currColumn++, item.Maker);
                worksheet.SetValue(lastRow, currColumn++, item.Code.First());
                worksheet.SetValue(lastRow, currColumn++, item.Name);
                worksheet.SetValue(lastRow, currColumn++, item.Amount);
                worksheet.SetValue(lastRow, currColumn++, item.DateDateTime.ToString("yyyy-MM-dd"));
                worksheet.SetValue(lastRow, currColumn++, item.Details1);
                worksheet.SetValue(lastRow, currColumn++, item.Details2);

                var range = worksheet.Cells[lastRow,
                                            ExcelReport.EipDataStartColumn,
                                            lastRow,
                                            ExcelReport.EipDataStartColumn + ExcelReport.EipProductColumnSpan];
                SetBorderAndBackgroundStyleForRange(range, lastRow % 2 != 0);
                ++lastRow;
            }
            return lastRow;
        }
        private int GenerateMissingEipProductPart(ExcelWorksheet worksheet, IEnumerable<ExcelProductData> productsMissingFromEip, int lastRow)
        {
            var potentialWarningCells = new List<Tuple<int, int>>();
            foreach (var item in productsMissingFromEip)
            {
                var currColumn = ExcelReport.ColumnOffset;

                worksheet.SetValue(lastRow, currColumn++, item.Maker);
                worksheet.SetValue(lastRow, currColumn++, item.Code);
                worksheet.SetValue(lastRow, currColumn++, item.Name);
                worksheet.SetValue(lastRow, currColumn++, item.AmountFirstHalf + item.AmountSecondHalf);
                worksheet.SetValue(lastRow, currColumn++, item.Date.ToString("yyyy-MM-dd"));
                worksheet.SetValue(lastRow, currColumn++, item.Details);

                worksheet.SetValue(lastRow, currColumn++, item.HasShapes ? ExcelReport.HasShapes : ExcelReport.DoesNotHaveShapes);
                potentialWarningCells.Add(item.HasShapes ? CellLocation(lastRow, currColumn - 1) : null);

                worksheet.SetValue(lastRow, currColumn++, item.CellBackgroundColors.Any() ? ExcelReport.HasBgColors : ExcelReport.DoesNotHaveBgColors);
                potentialWarningCells.Add(item.CellBackgroundColors.Any() ? CellLocation(lastRow, currColumn - 1) : null);

                var range = worksheet.Cells[lastRow,
                                            ExcelReport.ColumnOffset,
                                            lastRow,
                                            ExcelReport.ColumnOffset + ExcelReport.ExcelProductColumnSpan];
                SetBorderAndBackgroundStyleForRange(range, lastRow % 2 != 0);
                ++lastRow;
            }
            SetMismatchColoring(worksheet, potentialWarningCells.Where(c => c != null), ExcelReport.WarningColoring);
            return lastRow;
        }

        private void SetEipHeader(ExcelWorksheet worksheet, int currentRow, string header, string checkedRange)
        {
            worksheet.SetValue(currentRow, ExcelReport.EipDataStartColumn, header);
            worksheet.SetValue(currentRow, ExcelReport.EipDataStartColumn + 1, checkedRange);
            worksheet.Cells[currentRow, ExcelReport.EipDataStartColumn, currentRow, ExcelReport.EipDataStartColumn + 1].Style.Font.Bold = true;
        }

        private void SetOutOfRangeEiplHeader(ExcelWorksheet worksheet, int currentRow, string checkedRange)
        {
            SetEipHeader(worksheet, currentRow, ExcelReport.ProductsOutOfRange, checkedRange);
        }

        private int GenerateOutOfRangeProductPart(ExcelWorksheet worksheet, IEnumerable<I07> productsOutOfSelectedRange, int lastRow)
        {
            return GenerateMissingExcelProductPart(worksheet, productsOutOfSelectedRange, lastRow);
        }
    }
}
