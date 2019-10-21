using DiffGenerator2.Constants;
using DiffGenerator2.DTOs;
using DiffGenerator2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Services
{
    public class DiffGenerator : IDiffGenerator
    {
        private readonly ILogService _logService;

        public DiffGenerator(ILogService logService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        public DiffReport GenerateDiffReport(IList<I07> eipData, IList<ExcelBlockData> excelData, IList<SheetCheckBoxItem> checkMonthOnlySheets)
        {
            _logService.Information("Generating diff report");

            var mismatches = new List<Mismatch>();
            var excelProductsMissingFromEip = new List<ExcelProductData>();
            var existingEipProductsInExcel = new List<I07>();
            foreach (var excelBlock in excelData)
            {
                var toCheckMonthOnly = checkMonthOnlySheets.Any(sheet => sheet.Name == excelBlock.SheetName);
                foreach(var excelProduct in excelBlock.ProductData)
                {
                    var eipProduct = eipData.FirstOrDefault(data => data.Code.First() == excelProduct.Code 
                                                                 && data.DateDateTime.Year == excelProduct.Date.Year
                                                                 && data.DateDateTime.Month == excelProduct.Date.Month);
                    if(ProductIsFinished(eipProduct, excelProduct.CellBackgroundColors))
                    {
                        continue;//do not generate mismatch because its missing for a valid reason
                    }
                    else if(!ExcelProductIsFoundInEip(eipProduct))
                    {
                        excelProductsMissingFromEip.Add(excelProduct);
                        continue;//do not generate mismatch, since its missing
                    }

                    existingEipProductsInExcel.Add(eipProduct);
                    if(HasMismatch(excelProduct, eipProduct, toCheckMonthOnly))
                    {
                        mismatches.Add(new Mismatch
                        {
                            BlockDate = excelBlock.Date,
                            SheetName = excelBlock.SheetName,
                            EipData = eipProduct,
                            ExcelData = excelProduct
                        });
                    }
                }
            }

            return new DiffReport
            {
                Mismatches = mismatches,
                ProductsMissingFromEip = excelProductsMissingFromEip,
                ProductsMissingFromExcel = eipData.Where(e => !existingEipProductsInExcel.Contains(e))
            };
        }

        private bool ExcelProductIsFoundInEip(I07 eipProduct) => eipProduct != null;

        private bool ProductIsFinished(I07 eipProduct, IEnumerable<string> cellBackgroundColors)
        {
            return eipProduct == null && cellBackgroundColors.Any(c => ExcludedColors.MarkedInExcelAsDone.Contains(c));
        }

        private bool HasMismatch(ExcelProductData excelProduct, I07 eipProduct, bool checkMonthOnly)
        {
            var isMismatchWithoutDay = excelProduct.AmountFirstHalf + excelProduct.AmountSecondHalf != eipProduct.Amount
                                        || excelProduct.Maker != eipProduct.Maker
                                        || excelProduct.Name != eipProduct.Name;
            var isMismatchWithDay = isMismatchWithoutDay || excelProduct.Date.Day != eipProduct.DateDateTime.Day;

            return checkMonthOnly ?  isMismatchWithoutDay : isMismatchWithDay;
        }
    }
}
