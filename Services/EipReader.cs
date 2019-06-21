using DiffGenerator2.DTOs;
using DiffGenerator2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Services
{
    public class EipReader : IEipReader
    {
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
            if(index == -1 && string.IsNullOrWhiteSpace(input)) {
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

        public IEnumerable<EipDTO> GetEipContents(string eipFileName)
        {
            var prunedEipFile = PrunedEipFile(eipFileName);
  
            return null;
            //work with cleaned file contentsq
        }
    }
}
