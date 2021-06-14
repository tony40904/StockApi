using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using StockApi.Services;

namespace StockApi.Tests.Services
{
    [TestFixture]
    public class StockApiServiceTest
    {
        [Test]
        public async Task TestRetrieveByCorrectStockNoAndCorrectDateRange()
        {
            // Arrange
            var stockApiService = new StockApiService();
            var stockNo = "1101";
            var start = "20210601";
            var end = "20210610";

            var expectedCount = 8;
            var expectedStat = "OK";

            // Act
            Dictionary<string, Object> result = await stockApiService.RetrieveByStockNo(stockNo, start, end);
            var resultStat = (string)result.GetValueOrDefault("stat");
            var resultDatas = (List<List<string>>)result.GetValueOrDefault("datas");

            // Assert
            Assert.AreEqual(expectedStat, resultStat);
            Assert.AreEqual(expectedCount, resultDatas.Count);
        }

        [Test]
        public async Task TestRetrieveByWrongStockNo()
        {
            // Arrange
            var stockApiService = new StockApiService();
            var stockNo = "0000";
            var start = "20210601";
            var end = "20210610";

            var expectedStat = "No data";

            // Act
            Dictionary<string, Object> result = await stockApiService.RetrieveByStockNo(stockNo, start, end);
            var resultStat = (string)result.GetValueOrDefault("stat");

            // Assert
            Assert.AreEqual(expectedStat, resultStat);
        }

        [Test]
        public async Task TestRetrieveByStockNoWithWrongDateRange()
        {
            // Arrange
            var stockApiService = new StockApiService();
            var stockNo = "1101";
            var start = "20210602";
            var end = "20210601";

            var expectedStat = "No data";

            // Act
            Dictionary<string, Object> result = await stockApiService.RetrieveByStockNo(stockNo, start, end);
            var resultStat = (string)result.GetValueOrDefault("stat");

            // Assert
            Assert.AreEqual(expectedStat, resultStat);
        }

        [Test]
        public async Task TestRetrieveByDate()
        {
            // Arrange
            var stockApiService = new StockApiService();
            var date = "20210601";
            var comparingField = "本益比";

            var expectedStat = "OK";
            var expectedField = new string[] { "證券代號", "證券名稱", "本益比" };

            // Act
            Dictionary<string, Object> result = await stockApiService.RetrieveByDate(date, comparingField);
            var resultStat = (string)result.GetValueOrDefault("stat");
            var resultFields = (List<string>)result.GetValueOrDefault("fields");
            var resultDatas = (List<List<string>>)result.GetValueOrDefault("datas");

            // Assert
            Assert.AreEqual(expectedStat, resultStat);
            Assert.AreEqual(expectedField, resultFields);

            var isSorted = true;
            for (var i = 0; i < resultDatas.Count - 1; i++)
            {
                double curValue = resultDatas[i][2] == "-" ? double.MinValue : double.Parse(resultDatas[i][2]);
                double nextValue = resultDatas[i + 1][2] == "-" ? double.MinValue : double.Parse(resultDatas[i + 1][2]);
                if (curValue < nextValue)
                {
                    isSorted = false;
                }
            }
            Assert.True(isSorted);
        }

        [Test]
        public async Task TestRetrieveByWrongDate()
        {
            // Arrange
            var stockApiService = new StockApiService();
            var date = "20210532";
            var comparingField = "本益比";

            var expectedStat = "Error: Date format is wrong";

            // Act
            Dictionary<string, Object> result = await stockApiService.RetrieveByDate(date, comparingField);
            var resultStat = (string)result.GetValueOrDefault("stat");

            // Assert
            Assert.AreEqual(expectedStat, resultStat);
        }

        [Test]
        public async Task TestGetIncreasingInterval()
        {
            // Arrange
            var stockApiService = new StockApiService();
            var stockNo = "1101";
            var start = "20210501";
            var end = "20210530";
            var targetField = "殖利率(%)";

            var expected = new Dictionary<string, string>();
            expected.Add("stat", "OK");
            expected.Add("field", "殖利率(%)");
            expected.Add("begin", "110年05月10日");
            expected.Add("end", "110年05月13日");
            expected.Add("number of days", "3");
            expected.Add("number of trading days", "3");

            // Act
            Dictionary<string, string> result = await stockApiService.GetIncreasingInterval(stockNo, start, end, targetField);

            // Assert
            Assert.AreEqual(expected, result);

        }

        [Test]
        public async Task TestGetIncreasingIntervalWithWrongStockNo()
        {
            // Arrange
            var stockApiService = new StockApiService();
            var stockNo = "0000";
            var start = "20210501";
            var end = "20210530";
            var targetField = "殖利率(%)";

            var expected = new Dictionary<string, string>();
            expected.Add("stat", "No data");

            // Act
            Dictionary<string, string> result = await stockApiService.GetIncreasingInterval(stockNo, start, end, targetField);

            // Assert
            Assert.AreEqual(expected, result);

        }

        [Test]
        public async Task TestGetIncreasingIntervalWithWrongDateRange()
        {
            // Arrange
            var stockApiService = new StockApiService();
            var stockNo = "1101";
            var start = "20210500";
            var end = "20210530";
            var targetField = "殖利率(%)";

            var expected = new Dictionary<string, string>();
            expected.Add("stat", "Error: Date format is wrong");

            // Act
            Dictionary<string, string> result = await stockApiService.GetIncreasingInterval(stockNo, start, end, targetField);

            // Assert
            Assert.AreEqual(expected, result);

        }
    }
}
