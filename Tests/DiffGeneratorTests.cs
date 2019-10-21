using System;
using System.Collections.Generic;
using System.Linq;
using DiffGenerator2.DTOs;
using DiffGenerator2.Interfaces;
using DiffGenerator2.Services;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class DiffGeneratorTests
    {
        [TestMethod]
        public void When_excel_product_matches_eip_product_there_are_no_mismatches()
        {
            //arrange
            var logService = new Mock<ILogService>();
            var eipProduct = new List<I07>
            {
                EipProductCodeAMakerA()
            };

            var excelProduct = new List<ExcelProductData>
            {
                ExcelProductCodeAMakerA()
            };

            var excelBlockData = new List<ExcelBlockData>
            {
                new ExcelBlockData
                {
                    Date = new DateTime(2019, 7, 7),
                    ProductData = excelProduct,
                    SheetName = "A"
                }
            };

            var generator = new DiffGenerator(logService.Object);

            var onlyMonthCheckSheets = new List<SheetCheckBoxItem>();
            //assess
            var result = generator.GenerateDiffReport(eipProduct, excelBlockData, onlyMonthCheckSheets);

            Assert.IsTrue(!result.Mismatches.Any());
        }

        [TestMethod]
        public void When_excel_product_does_not_match_eip_product_there_is_mismatch()
        {
            //arrange
            var logService = new Mock<ILogService>();
            var eipProduct = new List<I07>
            {
                EipProductCodeAMakerA()
            };

            var excelProduct = new List<ExcelProductData>
            {
                ExcelProductCodeAMakerB()
            };

            var excelBlockData = new List<ExcelBlockData>
            {
                new ExcelBlockData
                {
                    Date = new DateTime(2019, 7, 7),
                    ProductData = excelProduct,
                    SheetName = "A"
                }
            };

            var generator = new DiffGenerator(logService.Object);
            var onlyMonthCheckSheets = new List<SheetCheckBoxItem>();
            //assess
            var result = generator.GenerateDiffReport(eipProduct, excelBlockData, onlyMonthCheckSheets);
            var mismatches = result.Mismatches.ToList();

            Assert.IsTrue(mismatches.Count == 1 
                && mismatches[0].ExcelData.Maker != mismatches[0].EipData.Maker);
        }

        [TestMethod]
        public void When_excel_has_more_products_then_eip___products_missing_from_eip_is_set()
        {
            //arrange
            var logService = new Mock<ILogService>();
            var eipProduct = new List<I07>
            {
                EipProductCodeAMakerA()
            };

            var excelProduct = new List<ExcelProductData>
            {
                ExcelProductCodeAMakerA(),
                ExcelProductCodeBMakerB()

            };

            var excelBlockData = new List<ExcelBlockData>
            {
                new ExcelBlockData
                {
                    Date = new DateTime(2019, 7, 7),
                    ProductData = excelProduct,
                    SheetName = "A"
                }
            };

            var generator = new DiffGenerator(logService.Object);
            var onlyMonthCheckSheets = new List<SheetCheckBoxItem>();
            //assess
            var result = generator.GenerateDiffReport(eipProduct, excelBlockData, onlyMonthCheckSheets);
            var missing = result.ProductsMissingFromEip.ToList();

            Assert.IsTrue(!result.Mismatches.Any()
                && missing.Count == 1
                && missing[0].Code == "B");
        }

        [TestMethod]
        public void When_eip_has_more_products_then_excel___products_missing_from_excel_is_set()
        {
            //arrange
            var logService = new Mock<ILogService>();
            var eipProduct = new List<I07>
            {
                EipProductCodeAMakerA(),
                EipProductCodeBMakerA()
            };

            var excelProduct = new List<ExcelProductData>
            {
                ExcelProductCodeAMakerA()
            };

            var excelBlockData = new List<ExcelBlockData>
            {
                new ExcelBlockData
                {
                    Date = new DateTime(2019, 7, 7),
                    ProductData = excelProduct,
                    SheetName = "A"
                }
            };

            var generator = new DiffGenerator(logService.Object);
            var onlyMonthCheckSheets = new List<SheetCheckBoxItem>();
            //assess
            var result = generator.GenerateDiffReport(eipProduct, excelBlockData, onlyMonthCheckSheets);
            var missing = result.ProductsMissingFromExcel.ToList();

            Assert.IsTrue(!result.Mismatches.Any()
                && missing.Count == 1
                && missing[0].Code.First() == "B");
        }

        [TestMethod]
        public void When_eip_has_more_products_then_excel_and_mismatches__two_fields_are_set()
        {
            //arrange
            var logService = new Mock<ILogService>();
            var eipProduct = new List<I07>
            {
                EipProductCodeAMakerA(),
                EipProductCodeBMakerA()
            };

            var excelProduct = new List<ExcelProductData>
            {
                ExcelProductCodeAMakerB()
            };

            var excelBlockData = new List<ExcelBlockData>
            {
                new ExcelBlockData
                {
                    Date = new DateTime(2019, 7, 7),
                    ProductData = excelProduct,
                    SheetName = "A"
                }
            };

            var generator = new DiffGenerator(logService.Object);
            var onlyMonthCheckSheets = new List<SheetCheckBoxItem>();
            //assess
            var result = generator.GenerateDiffReport(eipProduct, excelBlockData, onlyMonthCheckSheets);
            var missing = result.ProductsMissingFromExcel.ToList();
            var mismatches = result.Mismatches.ToList();

            Assert.IsTrue(missing.Count == 1
                && missing[0].Code.First() == "B"
                && mismatches.Count == 1
                && mismatches[0].ExcelData.Maker != mismatches[0].EipData.Maker);
        }

        [TestMethod]
        public void When_excel_has_more_products_then_eip_and_mismatches__two_fields_are_set()
        {
            //arrange
            var logService = new Mock<ILogService>();
            var eipProduct = new List<I07>
            {
                EipProductCodeAMakerA()
            };

            var excelProduct = new List<ExcelProductData>
            {
                ExcelProductCodeAMakerB(),
                ExcelProductCodeBMakerB()
            };

            var excelBlockData = new List<ExcelBlockData>
            {
                new ExcelBlockData
                {
                    Date = new DateTime(2019, 7, 7),
                    ProductData = excelProduct,
                    SheetName = "A"
                }
            };

            var generator = new DiffGenerator(logService.Object);
            var onlyMonthCheckSheets = new List<SheetCheckBoxItem>();
            //assess
            var result = generator.GenerateDiffReport(eipProduct, excelBlockData, onlyMonthCheckSheets);
            var missing = result.ProductsMissingFromEip.ToList();
            var mismatches = result.Mismatches.ToList();

            Assert.IsTrue(missing.Count == 1
                && missing[0].Code == "B"
                && mismatches.Count == 1
                && mismatches[0].ExcelData.Maker != mismatches[0].EipData.Maker);
        }

        [TestMethod]
        public void When_there_are_all_kinds_of_mismatches_diff_report_is_set_fully()
        {
            //arrange
            var logService = new Mock<ILogService>();
            var eipProduct = new List<I07>
            {
                EipProductCodeAMakerA(),
                EipProductCodeCMakerA()
            };

            var excelProduct = new List<ExcelProductData>
            {
                ExcelProductCodeAMakerB(),
                ExcelProductCodeBMakerB()
            };

            var excelBlockData = new List<ExcelBlockData>
            {
                new ExcelBlockData
                {
                    Date = new DateTime(2019, 7, 7),
                    ProductData = excelProduct,
                    SheetName = "A"
                }
            };

            var generator = new DiffGenerator(logService.Object);
            var onlyMonthCheckSheets = new List<SheetCheckBoxItem>();
            //assess
            var result = generator.GenerateDiffReport(eipProduct, excelBlockData, onlyMonthCheckSheets);
            var eipMissing = result.ProductsMissingFromEip.ToList();
            var excelMissing = result.ProductsMissingFromExcel.ToList();
            var mismatches = result.Mismatches.ToList();

            Assert.IsTrue(eipMissing.Count == 1
                && eipMissing[0].Code == "B"
                && mismatches.Count == 1
                && excelMissing.Count == 1
                && excelMissing[0].Code.First() == "C"
                && mismatches[0].ExcelData.Maker != mismatches[0].EipData.Maker);
        }

        [TestMethod]
        public void When_special_sheets_are_selected_day_mismatch_is_not_counted()
        {
            //arrange
            var logService = new Mock<ILogService>();
            var eipProduct = new List<I07>
            {
                EipProductCodeAMakerA()
            };

            var product = ExcelProductCodeAMakerA();
            product.Date = product.Date.AddDays(5);
            var excelProduct = new List<ExcelProductData>
            {
                product
            };

            var excelBlockData = new List<ExcelBlockData>
            {
                new ExcelBlockData
                {
                    Date = new DateTime(2019, 7, 7),
                    ProductData = excelProduct,
                    SheetName = "sheet"
                }
            };

            var generator = new DiffGenerator(logService.Object);
            var onlyMonthCheckSheets = new List<SheetCheckBoxItem> 
            { 
                new SheetCheckBoxItem { IsChecked = true, Name = "sheet" }
            };
            //assess
            var result = generator.GenerateDiffReport(eipProduct, excelBlockData, onlyMonthCheckSheets);
            var mismatches = result.Mismatches.ToList();

            Assert.IsTrue(mismatches.Count == 0);
        }

        [TestMethod]
        public void When_special_sheets_are_not_selected_day_mismatch_is_counted()
        {
            //arrange
            var logService = new Mock<ILogService>();
            var eipProduct = new List<I07>
            {
                EipProductCodeAMakerA()
            };

            var product = ExcelProductCodeAMakerA();
            product.Date = product.Date.AddDays(5);
            var excelProduct = new List<ExcelProductData>
            {
                product
            };

            var excelBlockData = new List<ExcelBlockData>
            {
                new ExcelBlockData
                {
                    Date = new DateTime(2019, 7, 7),
                    ProductData = excelProduct,
                    SheetName = "sheet"
                }
            };

            var generator = new DiffGenerator(logService.Object);
            var onlyMonthCheckSheets = new List<SheetCheckBoxItem>();
            //assess
            var result = generator.GenerateDiffReport(eipProduct, excelBlockData, onlyMonthCheckSheets);
            var mismatches = result.Mismatches.ToList();

            Assert.IsTrue(mismatches.Count == 1);
        }

        private ExcelProductData ExcelProductCodeAMakerA()
        {
            return new ExcelProductData
            {
                Maker = "A",
                Code = "A",
                Name = "A",
                AmountFirstHalf = 1,
                AmountSecondHalf = 0,
                Date = new DateTime(2019, 7, 7),
                Details = "A",
                CellBackgroundColors = new List<string>(),
                Comments = new List<string>(),
                HasShapes = false
            };
        }

        private ExcelProductData ExcelProductCodeAMakerB()
        {
            return new ExcelProductData
            {
                Maker = "B",
                Code = "A",
                Name = "A",
                AmountFirstHalf = 1,
                AmountSecondHalf = 0,
                Date = new DateTime(2019, 7, 7),
                Details = "A",
                CellBackgroundColors = new List<string>(),
                Comments = new List<string>(),
                HasShapes = false
            };
        }

        private ExcelProductData ExcelProductCodeBMakerB()
        {
            return new ExcelProductData
            {
                Maker = "B",
                Code = "B",
                Name = "B",
                AmountFirstHalf = 1,
                AmountSecondHalf = 0,
                Date = new DateTime(2019, 7, 7),
                Details = "B",
                CellBackgroundColors = new List<string>(),
                Comments = new List<string>(),
                HasShapes = false
            };
        }

        private I07 EipProductCodeAMakerA()
        {
            return new I07
            {
                Maker = "A",
                Code = new string[] { "A" },
                Name = "A",
                Amount = 1,
                DateDateTime = new DateTime(2019, 7, 7),
                Details1 = "A",
                Details2 = "A",
            };
        }

        private I07 EipProductCodeBMakerA()
        {
            return new I07
            {
                Maker = "A",
                Code = new string[] { "B" },
                Name = "A",
                Amount = 1,
                DateDateTime = new DateTime(2019, 7, 7),
                Details1 = "A",
                Details2 = "A",
            };
        }
        private I07 EipProductCodeCMakerA()
        {
            return new I07
            {
                Maker = "A",
                Code = new string[] { "C" },
                Name = "A",
                Amount = 1,
                DateDateTime = new DateTime(2019, 7, 7),
                Details1 = "A",
                Details2 = "A",
            };
        }

    }
}
