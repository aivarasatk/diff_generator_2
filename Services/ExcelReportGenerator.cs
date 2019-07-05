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
            var path = $"{Directory.GetCurrentDirectory()}\\Reports\\report_{DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")}.xlsx";
            var fileInfo = new FileInfo(path);
            fileInfo.Directory.Create();//creating a dir if does not exist

            using (var excelPackage = new ExcelPackage(fileInfo))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("report");
                InitializeTableHeaders(worksheet);
                var lastRow = GenerateMismatchPart(worksheet, diffReport.Mismatches);
                SetFilterForTable(worksheet, lastRow - 1);

                SetSeparator(worksheet.Cells[lastRow, ExcelReport.ColumnOffset, lastRow, ExcelReport.DataEndColumn -1]);
                SetMissingFromEipHeader(worksheet, ++lastRow);
                lastRow = GenerateMissingEipProductPart(worksheet, diffReport.ProductsMissingFromEip, ++lastRow);

                SetSeparator(worksheet.Cells[lastRow, ExcelReport.ColumnOffset, lastRow, ExcelReport.DataEndColumn -1]);
                SetMissingFromExcelHeader(worksheet, lastRow);
                GenerateMissingExcelProductPart(worksheet, diffReport.ProductsMissingFromExcel, ++lastRow);
                
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
            SetBackgroundColor(tableNameRange, ExcelReport.RowColoring());
            ++currRow;

            //set table headers
            var headerRange = worksheet.Cells[currRow, columnOffset, currRow, columnOffset + columnSpan - 1];
            headerRange.Style.Font.Bold = true;
            SetBorderAndBackgroundStyleForRange(headerRange, false);
            
            foreach (var name in headerNames)
            {
                worksheet.SetValue(currRow, columnOffset++, name);
            }

            
        }

        private void SetFilterForTable(ExcelWorksheet worksheet, int lastRow)
        {
            worksheet.Cells[ExcelReport.DataStartRowOffset - 1, ExcelReport.ColumnOffset, lastRow, ExcelReport.DataEndColumn - 1]
                     .AutoFilter = true;
        }

        private int GenerateMismatchPart(ExcelWorksheet worksheet, IEnumerable<Mismatch> mismatches)
        {
            var groupedMismatchesBySheet = mismatches.OrderByDescending(m => m.SheetName).ThenBy(m => m.BlockDate).GroupBy(m => m.SheetName);

            var addedRows = ExcelReport.DataStartRowOffset;
            foreach(var groupedMismatch in groupedMismatchesBySheet)
            {
                worksheet.SetValue(addedRows++, ExcelReport.ColumnOffset, groupedMismatch.Key);

                var columnSpan = 0;
                var errorCells = new List<Tuple<int, int>>();
                foreach(var item in groupedMismatch.AsEnumerable())
                {
                    var currColumn = ExcelReport.ColumnOffset;
                    worksheet.SetValue(addedRows, currColumn++, item.ExcelData.Code);
                    if(item.ExcelData.Code != item.EipData.Code.First())
                    {
                        errorCells.Add(new Tuple<int,int>(addedRows, currColumn -1));
                    }

                    worksheet.SetValue(addedRows, currColumn++, item.ExcelData.Name);
                    if (item.ExcelData.Name != item.EipData.Name)
                    {
                        errorCells.Add(new Tuple<int, int>(addedRows, currColumn - 1));
                    }

                    worksheet.SetValue(addedRows, currColumn++, item.ExcelData.Maker);
                    if (item.ExcelData.Maker != item.EipData.Maker)
                    {
                        errorCells.Add(new Tuple<int, int>(addedRows, currColumn - 1));
                    }

                    worksheet.SetValue(addedRows, currColumn++, item.ExcelData.Date.ToString("yyyy-MM-dd"));
                    if (item.ExcelData.Date.ToString("yyyy-MM-dd") != item.EipData.DateDateTime.ToString("yyyy-MM-dd"))
                    {
                        errorCells.Add(new Tuple<int, int>(addedRows, currColumn - 1));
                    }

                    worksheet.SetValue(addedRows, currColumn++, item.ExcelData.AmountFirstHalf + item.ExcelData.AmountSecondHalf);
                    if (item.ExcelData.AmountFirstHalf + item.ExcelData.AmountSecondHalf != item.EipData.Amount)
                    {
                        errorCells.Add(new Tuple<int, int>(addedRows, currColumn - 1));
                    }

                    worksheet.SetValue(addedRows, currColumn++, item.ExcelData.Details);
                    worksheet.SetValue(addedRows, currColumn++, item.ExcelData.HasShapes ? "Taip" : "Ne");
                    worksheet.SetValue(addedRows, currColumn++, item.ExcelData.CellBackgroundColors.Any() ? "Taip" : "Ne");

                    columnSpan = currColumn - 1 - ExcelReport.ColumnOffset;
                    SetBorderAndBackgroundStyleForRange(worksheet.Cells[addedRows, ExcelReport.ColumnOffset, addedRows, columnSpan], addedRows % 2 != 0);
                    ++currColumn;//skip one column

                    worksheet.SetValue(addedRows, currColumn++, item.EipData.Code.First());
                    if (item.ExcelData.Code != item.EipData.Code.First())
                    {
                        errorCells.Add(new Tuple<int, int>(addedRows, currColumn - 1));
                    }

                    worksheet.SetValue(addedRows, currColumn++, item.EipData.Name);
                    if (item.ExcelData.Name != item.EipData.Name)
                    {
                        errorCells.Add(new Tuple<int, int>(addedRows, currColumn - 1));
                    }

                    worksheet.SetValue(addedRows, currColumn++, item.EipData.Maker);
                    if (item.ExcelData.Maker != item.EipData.Maker)
                    {
                        errorCells.Add(new Tuple<int, int>(addedRows, currColumn - 1));
                    }

                    worksheet.SetValue(addedRows, currColumn++, item.EipData.DateDateTime.ToString("yyyy-MM-dd"));
                    if (item.ExcelData.Date.ToString("yyyy-MM-dd") != item.EipData.DateDateTime.ToString("yyyy-MM-dd"))
                    {
                        errorCells.Add(new Tuple<int, int>(addedRows, currColumn - 1));
                    }

                    worksheet.SetValue(addedRows, currColumn++, item.EipData.Amount);
                    if (item.ExcelData.AmountFirstHalf + item.ExcelData.AmountSecondHalf != item.EipData.Amount)
                    {
                        errorCells.Add(new Tuple<int, int>(addedRows, currColumn - 1));
                    }

                    worksheet.SetValue(addedRows, currColumn++, item.EipData.Details1);
                    worksheet.SetValue(addedRows, currColumn++, item.EipData.Details2);

                    columnSpan = currColumn;
                    SetBorderAndBackgroundStyleForRange(worksheet.Cells[addedRows, ExcelReport.ColumnOffset, addedRows, columnSpan], addedRows % 2 != 0);
                    ++addedRows;
                }
                SetErrorColoring(worksheet, errorCells);
                ++addedRows;
            }
            return addedRows;
        }

        private void SetErrorColoring(ExcelWorksheet worksheet, IEnumerable<Tuple<int,int>> errorCells)
        {
            foreach(var error in errorCells)
            {
                var cell = worksheet.Cells[error.Item1, error.Item2];
                SetBackgroundColor(cell, ExcelReport.ErrorColoring());
            }
        }

        private void SetBorderAndBackgroundStyleForRange(ExcelRange excelRange, bool setBackgroundColor)
        {
            foreach(var cell in excelRange)
            {
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                if (setBackgroundColor)
                {
                    SetBackgroundColor(cell, ExcelReport.RowColoring());
                }        
            }
        }

        private void SetSeparator(ExcelRange range)
        {
            SetBackgroundColor(range, ExcelReport.SeparatorColoring());
        }

        private void SetMissingFromEipHeader(ExcelWorksheet worksheet, int currenRow)
        {
            SetSeparator(worksheet.Cells[currenRow, ExcelReport.ColumnOffset, currenRow, ExcelReport.ColumnOffset + ExcelReport.ExcelProductColumnSpan -1]);
            worksheet.SetValue(currenRow, ExcelReport.ColumnOffset, ExcelReport.ProductsMissingFromEip);
            worksheet.Cells[currenRow, ExcelReport.ColumnOffset].Style.Font.Bold = true;
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

                worksheet.SetValue(lastRow, currColumn++, item.Code.First());
                worksheet.SetValue(lastRow, currColumn++, item.Name);
                worksheet.SetValue(lastRow, currColumn++, item.Maker);
                worksheet.SetValue(lastRow, currColumn++, item.DateDateTime.ToString("yyyy-MM-dd"));
                worksheet.SetValue(lastRow, currColumn++, item.Amount);
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
            foreach (var item in productsMissingFromEip)
            {
                var currColumn = ExcelReport.ColumnOffset;

                worksheet.SetValue(lastRow, currColumn++, item.Code);
                worksheet.SetValue(lastRow, currColumn++, item.Name);
                worksheet.SetValue(lastRow, currColumn++, item.Maker);
                worksheet.SetValue(lastRow, currColumn++, item.Date.ToString("yyyy-MM-dd"));
                worksheet.SetValue(lastRow, currColumn++, item.AmountFirstHalf + item.AmountSecondHalf);
                worksheet.SetValue(lastRow, currColumn++, item.Details);

                var range = worksheet.Cells[lastRow,
                                            ExcelReport.ColumnOffset,
                                            lastRow,
                                            ExcelReport.ColumnOffset + ExcelReport.ExcelProductColumnSpan];
                SetBorderAndBackgroundStyleForRange(range, lastRow % 2 != 0);
                ++lastRow;
            }
            return lastRow;
        }

        private void SetMissingFromExcelHeader(ExcelWorksheet worksheet, int currentRow)
        {
            worksheet.SetValue(currentRow, ExcelReport.EipDataStartColumn, ExcelReport.ProductsMissingFromExcel);
            worksheet.Cells[currentRow, ExcelReport.EipDataStartColumn].Style.Font.Bold = true;
        }
    }
}
