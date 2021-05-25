using DiffGenerator2.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class OrderDiffGeneratorTests
    {
        [TestMethod]
        public void OrderNumsBelowRangeGenerareBelowRangeObject()
        {
            //Arrange
            var orderNums = new[] { 1, 2, 3, 4, 5 };

            var sut = new OrderDiffGenerator();

            //Act
            var orderReport = sut.InclusionReport(orderNums, 6, 10);

            //Assert
            Assert.AreEqual(5, orderReport.BelowLowerBound.Count());
        }

        [TestMethod]
        public void OrderNumsNotInRangeGenerateMissingIntervalObject()
        {
            //Arrange
            var orderNums = new[] { 1, 2, 3, 4, 5 };

            var sut = new OrderDiffGenerator();

            //Act
            var orderReport = sut.InclusionReport(orderNums, 6, 10);

            //Assert
            Assert.AreEqual(5, orderReport.MissingInInterval.Count());
        }

        [TestMethod]
        public void OrderNumsAboveRangeGenerateAboveIntervalObject()
        {
            //Arrange
            var orderNums = new[] { 11, 12, 13, 14, 15 };

            var sut = new OrderDiffGenerator();

            //Act
            var orderReport = sut.InclusionReport(orderNums, 6, 10);

            //Assert
            Assert.AreEqual(5, orderReport.AboveUpperBound.Count());
        }

        [TestMethod]
        public void WhenAllOrderNumbersAreInIntervalNoObjectIsGenerated()
        {
            //Arrange
            var orderNums = new[] { 6, 7, 8, 9, 10 };

            var sut = new OrderDiffGenerator();

            //Act
            var orderReport = sut.InclusionReport(orderNums, 6, 10);

            //Assert
            Assert.IsFalse(orderReport.BelowLowerBound.Any());
            Assert.IsFalse(orderReport.MissingInInterval.Any());
            Assert.IsFalse(orderReport.AboveUpperBound.Any());
        }

        [TestMethod]
        public void WhenAllOrderNumbersAreBelowAndAboveIntervalTwoObjectsAreGenerated()
        {
            //Arrange
            var orderNums = new[] { 1, 2, 3, 11, 12, 13 };

            var sut = new OrderDiffGenerator();

            //Act
            var orderReport = sut.InclusionReport(orderNums, 6, 10);

            //Assert
            Assert.AreEqual(3, orderReport.BelowLowerBound.Count());
            Assert.AreEqual(3, orderReport.AboveUpperBound.Count());
        }
    }
}
