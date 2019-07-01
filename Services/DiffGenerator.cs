﻿using DiffGenerator2.DTOs;
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

        public DiffReport GenerateDiffReport(IList<I07> eipData, IList<ExcelBlockData> excelData)
        {
            _logService.Information("Generating diff report");

            var mismatches = new List<Mismatch>();
            var excelProductsMissingFromEip = new List<ExcelProductData>();
            var existingEipProductsInExcel = new List<I07>();
            foreach (var excelBlock in excelData)
            {
                
                foreach(var excelProduct in excelBlock.ProductData)
                {
                    var eipProduct = eipData.FirstOrDefault(data => data.Code.First() == excelProduct.Code 
                                                                 && data.DateDateTime.Year == excelProduct.Date.Year
                                                                 && data.DateDateTime.Month == excelProduct.Date.Month);
                    if(eipProduct == null)
                    {
                        excelProductsMissingFromEip.Add(excelProduct);
                        continue;
                    }
                    existingEipProductsInExcel.Add(eipProduct);
                    if(HasMismatch(excelProduct, eipProduct))
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

        private bool HasMismatch(ExcelProductData excelProduct, I07 eipProduct)
        {
            return excelProduct.AmountFirstHalf + excelProduct.AmountSecondHalf != eipProduct.Amount
                || excelProduct.Maker != eipProduct.Maker
                || excelProduct.Name != eipProduct.Name
                || excelProduct.Date.Day != eipProduct.DateDateTime.Day;
        }
    }
}