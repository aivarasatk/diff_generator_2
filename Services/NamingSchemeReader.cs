using DiffGenerator2.Constants;
using DiffGenerator2.DTOs;
using DiffGenerator2.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Services
{
    public class NamingSchemeReader : INamingSchemeReader
    {
        private readonly ILogService _logService;
        public NamingSchemeReader(ILogService logService)
        {
            _logService = logService;
        }
        public IList<ColumnNamingScheme> GetColumnNamingScheme(string fileName)
        {
            var fileContents = System.IO.File.ReadAllText(fileName);
            var splitContents = fileContents.Split(NamingSchemeSplitter.Main);

            var columnNamingScheme = new List<ColumnNamingScheme>();
            foreach(var naming in splitContents)
            {
                var splitNaming = naming.Split(NamingSchemeSplitter.Secondary);
                try
                {
                    columnNamingScheme.Add(new ColumnNamingScheme
                    {
                        Name = splitNaming[0],
                        Id = Convert.ToInt32(splitNaming[1])
                    });
                }
                catch(Exception ex)
                {
                    _logService.Error("Could not convert naming scheme string value to int", ex);
                    throw;
                }
            }
            return columnNamingScheme;
        }
    }
}
