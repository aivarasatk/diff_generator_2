using DiffGenerator2.Constants;
using System;
using System.Text.RegularExpressions;

namespace DiffGenerator2.Services
{
    public static class ExcelParser
    {
        public static int? ExtractOrderNumber(string details)
        {
            if (string.IsNullOrWhiteSpace(details))
                return null;

            const string orderRegex = "^[u,U][0-9]+";// match 'u' or 'U' at the beginning of the string and then one or more numbers]

            var regex = new Regex(orderRegex);
            var match = regex.Match(details);

            if (match.Success)
            {
                var orderNumber = match.Value.Substring(1);
                return int.Parse(orderNumber);
            }

            return null;
        }

        public static int GetAmountIntValue(string amountText)
        {
            if (string.IsNullOrEmpty(amountText) || string.IsNullOrWhiteSpace(amountText))
            {
                return 0;
            }

            var cleanAmountText = amountText;
            foreach (var postfix in AllowedAmountPostfixes.Values)
            {
                var index = amountText.IndexOf(postfix);
                if (index != -1)
                {
                    cleanAmountText = amountText.Remove(index, postfix.Length);
                    break;
                }
            }

            if (int.TryParse(cleanAmountText, out var res))
            {
                return res;
            }

            throw new ArgumentException($"'{amountText}' is not a valid number");
        }

        public static DateTime ParsedBlockHeader(string blockHeader)
        {
            var splitHeader = blockHeader.Trim().Split(' ');
            var numericMonthValue = GetMonthNumber(splitHeader[0]);
            var numericYearValue = GetYearNumber(splitHeader[1]);
            return new DateTime(numericYearValue, numericMonthValue, 1);
        }

        private static int GetYearNumber(string year) => int.Parse(year);

        private static int GetMonthNumber(string month)
        {
            switch (month.ToLower())
            {
                case "sausis": return 1;
                case "vasaris": return 2;
                case "kovas": return 3;
                case "balandis": return 4;
                case "gegužė": return 5;
                case "birželis": return 6;
                case "liepa": return 7;
                case "rugpjūtis": return 8;
                case "rugsėjis": return 9;
                case "spalis": return 10;
                case "lapkritis": return 11;
                case "gruodis": return 12;
                default: throw new Exception($"Header month '{month}' is not a valid month");
            }
        }
    }
}
