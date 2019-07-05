﻿using DiffGenerator2.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Interfaces
{
    public interface IExcelReportGenerator
    {
        void GenerateReport(DiffReport diffReport);
    }
}