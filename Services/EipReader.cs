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
using System.Text.RegularExpressions;

namespace DiffGenerator2.Services
{
    public class EipReader : IEipReader
    {
        private ILogService _logService;

        // filters control characters but allows only properly-formed surrogate sequences
        private static Regex _invalidXMLChars = new Regex(
            @"(?<![\uD800-\uDBFF])[\uDC00-\uDFFF]|[\uD800-\uDBFF](?![\uDC00-\uDFFF])|[\x00-\x08\x0B\x0C\x0E-\x1F\x7F-\x9F\uFEFF\uFFFE\uFFFF]",
            RegexOptions.Compiled);

        public EipReader(ILogService logService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
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
                throw new Exception($"Bad eip file: line doesn't have a pivot. Input: '{input}' Pivot: '{pivot}'");
            }
            return input;
        }

        public string[] GetEipContents(string eipFileName)
        {
            return File.ReadAllLines(eipFileName, Encoding.GetEncoding("iso-8859-13"));
        }              

        public IEnumerable<I07> GetParsedEipContents(IEnumerable<string> content)
        {
            var prunedEipFile = string.Join("", PrunedEipFile(content));
            var fileWithEscapedChars = EscapeCharacters(prunedEipFile);
            _logService.Information($"Deserliazing eip file");
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(I06));
                using (StringReader stringReader = new StringReader(fileWithEscapedChars))
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

        /// <summary>
        /// removes any unusual unicode characters that can't be encoded into XML
        /// </summary>
        public static string RemoveInvalidXMLChars(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return _invalidXMLChars.Replace(text, "");
        }

        private IEnumerable<string> PrunedEipFile(IEnumerable<string> lines)
        {
            foreach (string line in lines)
            {
                yield return TruncateStringAfterChar(line, '<');
            }
        }

        private string EscapeCharacters(string prunedEipFile)
        {
            var charsToEscape = new char[]
            {
                '&'
            };

            foreach(var charToEscape in charsToEscape)
            {
                prunedEipFile = prunedEipFile.Replace(charToEscape.ToString(), "&amp;");
            }
            return prunedEipFile;
        }
    }
}

