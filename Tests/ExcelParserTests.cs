using DiffGenerator2.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class ExcelParserTests
    {
        [DataRow("U840_TK601-3", 840)]
        [DataRow("U839_TK508-3", 839)]
        [DataRow("u840_TK601-3", 840)]
        [DataRow("u839_TK508-3", 839)]
        [DataRow("U941_TK075", 941)]
        [DataRow("U41_TK075", 41)]
        [DataRow("U896_TK021", 896)]
        [DataRow("U970-2_TK831", 970)]
        [DataRow("U9708-2_TK831", 9708)]
        [DataRow("U9888_TK831", 9888)]
        [DataTestMethod]
        public void WithValidDetailsValueGetParsedIntoNumber(string details, int? result)
        {
            Assert.AreEqual(result, ExcelParser.ExtractOrderNumber(details));
        }

        [DataRow("somethingU9888_TK831")]
        [DataRow("9888-2_TK831")]
        [DataRow("aU9888_TK831")]
        [DataRow("pastaba")]
        [DataTestMethod]
        public void WhenDetailsDoNotStartWithPatternReturnsNull(string details)
        {
            Assert.AreEqual(null, ExcelParser.ExtractOrderNumber(details));
        }

    }
}
