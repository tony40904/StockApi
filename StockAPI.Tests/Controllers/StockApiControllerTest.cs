using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using StockApi.Controllers;
using StockApi.Services;

namespace StockAPI.Tests.Controllers
{
    [TestFixture]
    public class StockApiControllerTest
    {
        [Test]
        public async Task TestGetStocksForCurrentNDays()
        {
            // Arrange
            IStockApiService stockApiService = Substitute.For<IStockApiService>();
            var stockApiController = new StockApiController(stockApiService);

            var stockApiServiceFakeReturn = new Dictionary<string, Object>();
            stockApiServiceFakeReturn.Add("stat", "OK");
            stockApiServiceFakeReturn.Add("fields", new List<string>(new string[] { "A", "B", "C" }));
            var fakeDatas = new List<List<string>>();
            fakeDatas.Add(new List<string>(new string[] { "1", "2", "3" }));
            fakeDatas.Add(new List<string>(new string[] { "4", "5", "6" }));
            stockApiServiceFakeReturn.Add("datas", fakeDatas);

            stockApiService.RetrieveByStockNo("123", 3).Returns(stockApiServiceFakeReturn);

            var expected = "\"A\",\"B\",\"C\"\n" +
                "\"1\",\"2\",\"3\"\n" +
                "\"4\",\"5\",\"6\"\n";

            // Act
            string result = await stockApiController.GetStocksForCurrentNDays("1101", 3);

            // Assert
            await stockApiService.Received().RetrieveByStockNo("1101", 3);
            Assert.AreEqual(expected, result);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public async Task TestGetStocksForCurrentNDaysWithInvalidN(int n)
        {
            // Arrange
            IStockApiService stockApiService = Substitute.For<IStockApiService>();
            var stockApiController = new StockApiController(stockApiService);

            var expected = "N should be a positive integer.";

            // Act
            string result = await stockApiController.GetStocksForCurrentNDays("1101", n);

            // Assert
            await stockApiService.DidNotReceiveWithAnyArgs().RetrieveByStockNo(default(string), default(int));
            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task TestGetSortedStocks()
        {
            // Arrange
            IStockApiService stockApiService = Substitute.For<IStockApiService>();
            var stockApiController = new StockApiController(stockApiService);

            var stockApiServiceFakeReturn = new Dictionary<string, Object>();
            stockApiServiceFakeReturn.Add("stat", "OK");
            stockApiServiceFakeReturn.Add("fields", new List<string>(new string[] { "A", "B" }));
            var fakeDatas = new List<List<string>>();
            fakeDatas.Add(new List<string>(new string[] { "1", "2" }));
            fakeDatas.Add(new List<string>(new string[] { "3", "4" }));
            fakeDatas.Add(new List<string>(new string[] { "5", "6" }));
            fakeDatas.Add(new List<string>(new string[] { "7", "8" }));
            stockApiServiceFakeReturn.Add("datas", fakeDatas);

            stockApiService.RetrieveByDate("20210101", "本益比").Returns(stockApiServiceFakeReturn);

            var expected = "\"A\",\"B\"\n" +
                "\"1\",\"2\"\n" +
                "\"3\",\"4\"\n";

            // Act
            string result = await stockApiController.GetSortedStocks("20210101", 2, "PER");

            // Assert
            await stockApiService.Received().RetrieveByDate("20210101", "本益比");
            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task TestGetSortedStocksWithReversedData()
        {
            // Arrange
            IStockApiService stockApiService = Substitute.For<IStockApiService>();
            var stockApiController = new StockApiController(stockApiService);

            var stockApiServiceFakeReturn = new Dictionary<string, Object>();
            stockApiServiceFakeReturn.Add("stat", "OK");
            stockApiServiceFakeReturn.Add("fields", new List<string>(new string[] { "A", "B" }));
            var fakeDatas = new List<List<string>>();
            fakeDatas.Add(new List<string>(new string[] { "1", "2" }));
            fakeDatas.Add(new List<string>(new string[] { "3", "4" }));
            fakeDatas.Add(new List<string>(new string[] { "5", "6" }));
            fakeDatas.Add(new List<string>(new string[] { "7", "8" }));
            stockApiServiceFakeReturn.Add("datas", fakeDatas);

            stockApiService.RetrieveByDate("20210101", "本益比").Returns(stockApiServiceFakeReturn);

            var expected = "\"A\",\"B\"\n" +
                "\"7\",\"8\"\n" +
                "\"5\",\"6\"\n";

            // Act
            string result = await stockApiController.GetSortedStocks("20210101", 2, "PER", reverse: true);

            // Assert
            await stockApiService.Received().RetrieveByDate("20210101", "本益比");
            Assert.AreEqual(expected, result);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public async Task TestGetSortedStocksWithInvalidN(int n)
        {
            // Arrange
            IStockApiService stockApiService = Substitute.For<IStockApiService>();
            var stockApiController = new StockApiController(stockApiService);

            var expected = "N should be a positive integer.";

            // Act
            string result = await stockApiController.GetSortedStocks("20210101", n, "PER");

            // Assert
            await stockApiService.DidNotReceiveWithAnyArgs().RetrieveByDate(default(string), default(string));
            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task TestGetSortedStocksWithInvalidDate()
        {
            // Arrange
            IStockApiService stockApiService = Substitute.For<IStockApiService>();
            var stockApiController = new StockApiController(stockApiService);

            var stockApiServiceFakeReturn = new Dictionary<string, Object>();
            stockApiServiceFakeReturn.Add("stat", "Error: Date format is wrong");

            stockApiService.RetrieveByDate("20210230", "本益比").Returns(stockApiServiceFakeReturn);

            var expected = "Error: Date format is wrong";

            // Act
            string result = await stockApiController.GetSortedStocks("20210230", 2, "PER");

            // Assert
            await stockApiService.Received().RetrieveByDate("20210230", "本益比");
            Assert.AreEqual(expected, result);
        }

        [TestCase("ABC")]
        public async Task TestGetSortedStocksWithInvalidSortBy(string sortBy)
        {
            // Arrange
            IStockApiService stockApiService = Substitute.For<IStockApiService>();
            var stockApiController = new StockApiController(stockApiService);

            var expected = "Invalid sortBy value";

            // Act
            string result = await stockApiController.GetSortedStocks("20210101", 2, sortBy);

            // Assert
            await stockApiService.DidNotReceiveWithAnyArgs().RetrieveByDate(default(string), default(string));
            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task TestGetIncreacingInterval()
        {
            // Arrange
            IStockApiService stockApiService = Substitute.For<IStockApiService>();
            var stockApiController = new StockApiController(stockApiService);

            var stockNo = "123";
            var field = "PER";
            var from = "20210501";
            var to = "20210530";

            var stockApiServiceFakeReturn = new Dictionary<string, string>();
            stockApiServiceFakeReturn.Add("stat", "OK");
            stockApiServiceFakeReturn.Add("other field", "other data");

            stockApiService.GetIncreasingInterval(stockNo, from, to, "本益比").Returns(stockApiServiceFakeReturn);

            var expected = stockApiServiceFakeReturn;


            // Act
            Dictionary<string, string> result = await stockApiController.GetIncreacingInterval(stockNo, field, from, to);

            // Assert
            await stockApiService.Received().GetIncreasingInterval(stockNo, from, to, "本益比");
            Assert.AreEqual(expected, result);
        }

        [TestCase("ABC")]
        public async Task TestGetIncreacingIntervalWithInvalidFieldName(string field)
        {
            // Arrange
            IStockApiService stockApiService = Substitute.For<IStockApiService>();
            var stockApiController = new StockApiController(stockApiService);

            var stockNo = "123";
            var from = "20210501";
            var to = "20210530";

            var expected = new Dictionary<string, string>();
            expected.Add("stat", "Invalid field value");

            // Act
            Dictionary<string, string> result = await stockApiController.GetIncreacingInterval(stockNo, field, from, to);

            // Assert
            await stockApiService.DidNotReceiveWithAnyArgs().GetIncreasingInterval(default(string), default(string), default(string), default(string));
            Assert.AreEqual(expected, result);
        }
    }
}
