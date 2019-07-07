using System;
using System.Collections.Generic;
using System.Linq;
using DiffGenerator2.DTOs;
using DiffGenerator2.Interfaces;
using DiffGenerator2.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests
{
    [TestClass]
    public class EipReaderTests
    {
        [TestMethod]
        public void With_correct_eip_contents_returns_valid_collection()
        {
            //arrange
            var logService = new Mock<ILogService>();
            var reader = new EipReader(logService.Object);
            var content = new string[]
            {
                "<I06>",

                    "<I07>",
                    "<I07_KODAS>AAA  </I07_KODAS>",
                    "000067<I07_KODAS>AAA  </I07_KODAS>",
                    "000068<I07_PAV>AAA</I07_PAV>",
                    "000069<I07_KODAS_IS>AAA          </I07_KODAS_IS>",
                    "000071<I07_KIEKIS>         1</I07_KIEKIS>",
                    "000076<I07_GALIOJA_IKI>2019.01.01 00:00</I07_GALIOJA_IKI>                                                                                                                                    ",
                    "<I07_APRASYMAS1>AAA</I07_APRASYMAS1>",
                    "<I07_APRASYMAS2>AAA</I07_APRASYMAS2>",
                    "</I07>",

                    "<I07>",
                    "<I07_KODAS>BBB  </I07_KODAS>",
                    "000067<I07_KODAS>BBB  </I07_KODAS>",
                    "000068<I07_PAV>BBB</I07_PAV>",
                    "000069<I07_KODAS_IS>BBB          </I07_KODAS_IS>",
                    "000071<I07_KIEKIS>         1</I07_KIEKIS>",
                    "000076<I07_GALIOJA_IKI>2019.01.01 00:00</I07_GALIOJA_IKI>                                                                                                                                    ",
                    "<I07_APRASYMAS1>BBB</I07_APRASYMAS1>",
                    "<I07_APRASYMAS2>BBB</I07_APRASYMAS2>",
                    "</I07>",

                "</I06>"
            };
            var expectedResult = new List<I07>
            {
                new I07
                {
                    Code = new string[]{"AAA", "AAA"},
                    Name = "AAA",
                    Maker = "AAA",
                    DateDateTime = new DateTime(2019, 1, 1, 0,0,0),
                    Amount =1,
                    Details1 = "AAA",
                    Details2 = "AAA",
                },
                new I07
                {
                    Code = new string[]{"BBB", "BBB"},
                    Name = "BBB",
                    Maker = "BBB",
                    DateDateTime = new DateTime(2019, 1, 1, 0,0,0),
                    Amount = 1,
                    Details1 = "BBB",
                    Details2 = "BBB",
                }
            };
            //assess

            var result = reader.GetParsedEipContents(content).ToList();
            //assert
            for (int i = 0; i < result.Count; ++i)
            {
                Assert.IsTrue(expectedResult[i].Code.First() == result[i].Code.First()
                              && expectedResult[i].Name == result[i].Name
                              && expectedResult[i].Maker == result[i].Maker
                              && expectedResult[i].DateDateTime.Date == result[i].DateDateTime.Date
                              && expectedResult[i].Amount == result[i].Amount
                              && expectedResult[i].Details1 == result[i].Details1
                              && expectedResult[i].Details2 == result[i].Details2);
            }
        }
        [TestMethod]
        public void With_inccorrect_eip_contents_throws_an_InvalidOperationException()
        {
            //arrange
            var logService = new Mock<ILogService>();
            var reader = new EipReader(logService.Object);
            var content = new string[]
            {
                "<I06",
                "</I06>"
            };

            //assess
            //assert
            Assert.ThrowsException<InvalidOperationException>(() => reader.GetParsedEipContents(content));
        }

        [TestMethod]
        public void With_missing_I06t_throws_exception()
        {
            //arrange
            var logService = new Mock<ILogService>();
            var reader = new EipReader(logService.Object);
            var content = new string[]
            {
                "<I05>",
                "</I05>"
            };

            //assess
            //assert
            Assert.ThrowsException<InvalidOperationException>(() => reader.GetParsedEipContents(content));
        }


        [TestMethod]
        public void With_empty_I07_returns_emty_collection()
        {
            //arrange
            var logService = new Mock<ILogService>();
            var reader = new EipReader(logService.Object);
            var content = new string[]
            {
                "<I06>",

                "</I06>"
            };

            //assess
            var collection = reader.GetParsedEipContents(content);
            //assert
            Assert.IsTrue(!collection.Any());
        }
    }
}
