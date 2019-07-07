using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Constants
{
    public static class ExcelReport
    {
        public const int ColumnOffset = 2;// zero based
        public const int HeaderStartRowOffset = 2;// zero based
        public const int DataStartRowOffset = 5;// zero based
        public const int ExcelProductColumnSpan = 8;
        public const int EipProductColumnSpan = 7;
        public const int ColumnSpanBetweenTables = 1;
        public const int RowsToSkipAfterMismatches = 1;
        public const int TableNameColumnSpan = 2;

        public const int EipDataStartColumn = ColumnOffset + ExcelProductColumnSpan + ColumnSpanBetweenTables;
        public const int DataEndColumn = ColumnOffset + ExcelProductColumnSpan + ColumnSpanBetweenTables + EipProductColumnSpan;

        public static readonly Color RowColoring = Color.LightGray;
        public static readonly Color ErrorColoring =  Color.Red;
        public static readonly Color WarningColoring = Color.DarkOrange;
        public static readonly Color SeparatorColoring = Color.Yellow;

        public const string ExcelTable = "GP planas (Excelis)";
        public const string EipTable = "GEN planas (Rivilė)";

        public static readonly string[] ExcelTableHeaders = new string[]
        {
            "Gamybos pad.",
            "Kodas",
            "Pavadinimas",
            "Kiekis",
            "Data",
            "Pastabos",
            "Turi rodyklę",
            "Turi spalvą"
        };

        public static readonly string[] EipTableHeaders = new string[]
        {
            "Gamybos pad.",
            "Kodas",
            "Pavadinimas",
            "Kiekis",
            "Data",
            "Aprašymas 1",
            "Aprašymas 2"
        };


        public const string ProductsMissingFromExcel = "Užsakymai, kurių nėra GP plane(Excel)";
        public const string ProductsMissingFromEip = "Užsakymai, kurių nėra GEN plane (Rivilėje)";
    }
}
