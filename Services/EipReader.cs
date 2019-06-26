using DiffGenerator2.DTOs;
using DiffGenerator2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;

namespace DiffGenerator2.Services
{
    public class EipReader : IEipReader
    {
        private ILogService _logService;

        public EipReader(ILogService logService)
        {
            _logService = logService;
        }

        private string[] ReadEipFile(string eipFileName)
        {
            return System.IO.File.ReadAllLines(eipFileName);
        }

        private string TruncateStringAfterChar(string input, char pivot)
        {
            int index = input.IndexOf(pivot);
            if (index > 0)
            {
                return input.Substring(index);
            }
            if (index == -1 && !string.IsNullOrWhiteSpace(input))
            {
                _logService.Error($"Bad eip file: line doesn't have a pivot. Input: '{input}' Pivot: '{pivot}'");
                throw new Exception("Bad eip file: line doesn't have a pivot but has non-whitespace symbols");
            }
            return input;
        }

        private IEnumerable<string> PrunedEipFile(string eipFileName)
        {
            var lines = ReadEipFile(eipFileName);
            foreach (string line in lines)
            {
                yield return TruncateStringAfterChar(line, '<');
            }
        }

        public IEnumerable<I07> GetEipContents(string eipFileName)
        {
            var prunedEipFile = string.Join("", PrunedEipFile(eipFileName));

            //work with cleaned file contents
            _logService.Information($"Deserliazing eip file");
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(I06));
                using (StringReader stringReader = new StringReader(prunedEipFile))
                {
                    return ((I06)xmlSerializer.Deserialize(stringReader)).I07;
                }
            }
            catch (Exception ex)
            {
                _logService.Error($"Deserializing failed",ex);
                throw;
            }
        }
    }
}

