using System.Collections.Generic;
using NUnit.Framework;
using StockApi.Utils;

namespace StockApi.Tests.Utils
{
    [TestFixture]
    public class DataProcessUtilTest
    {
        [TestCase("20201101", "20201201", 0, 0)]
        [TestCase("20201101", "20201220", 0, 1)]
        [TestCase("20201101", "20210102", 0, 2)]
        [TestCase("20201101", "20210214", 0, 4)]
        [TestCase("20201101", "20210215", 0, 5)]
        [TestCase("20210102", "20210214", 2, 2)]
        [TestCase("20210102", "20210215", 2, 3)]
        [TestCase("20210210", "20210210", 3, 1)]
        [TestCase("20210215", "20210510", 4, 1)]
        [TestCase("20210216", "20210510", 0, 0)]
        public void TestPruneData(string start, string end, int expectedIndex, int expectedCount)
        {
            // Arrange
            var datas = new List<List<string>>();
            datas.Add(new List<string>(new string[] { "109年12月20日", "1" }));
            datas.Add(new List<string>(new string[] { "110年01月01日", "2" }));
            datas.Add(new List<string>(new string[] { "110年01月03日", "3" }));
            datas.Add(new List<string>(new string[] { "110年02月10日", "4" }));
            datas.Add(new List<string>(new string[] { "110年02月15日", "5" }));

            var expected = datas.GetRange(expectedIndex, expectedCount);


            // Act
            DataProcessUtil.PruneData(datas, start, end);

            // Assert
            Assert.AreEqual(expected, datas);
        }

        [Test]
        public void TestFillEmptyFields()
        {
            // Arrange
            var datas = new List<List<string>>();
            datas.Add(new List<string>(new string[] { "1A", "1B", "1C" }));
            datas.Add(new List<string>(new string[] { "2A", "2B", "2C" }));
            datas.Add(new List<string>(new string[] { "3A", "3B", "3C" }));

            var originalFields = new List<string>(new string[] { "A", "B", "C" });

            var newFields = new List<string>(new string[] { "D", "C", "B" });

            var expected = new List<List<string>>();
            expected.Add(new List<string>(new string[] { null, "1C", "1B" }));
            expected.Add(new List<string>(new string[] { null, "2C", "2B" }));
            expected.Add(new List<string>(new string[] { null, "3C", "3B" }));

            // Act
            DataProcessUtil.transformFields(datas, originalFields, newFields);

            // Assert
            Assert.AreEqual(expected, datas);
        }

        [TestCase("A", true, 0, 2)]
        [TestCase("A", false, 3, 6)]
        [TestCase("B", true, 0, 0)]
        [TestCase("B", false, 3, 5)]
        [TestCase("C", true, 1, 5)]
        [TestCase("C", false, 0, 6)]
        [TestCase("D", true, 1, 2)]
        [TestCase("D", false, 3, 5)]
        public void TestGetLongestIncreasingInterval(string targetField, bool distinct, int expectedBeginIndex, int expectedEndIndex)
        {
            // Arrange
            var fields = new List<string>(new string[] { "A", "B", "C", "D" });
            var datas = new List<List<string>>();
            datas.Add(new List<string>(new string[] { "1", "4", "1", "1" }));
            datas.Add(new List<string>(new string[] { "2", "3", "1", "-" }));
            datas.Add(new List<string>(new string[] { "5", "3", "2", "2" }));
            datas.Add(new List<string>(new string[] { "3", "2", "3", "-" }));
            datas.Add(new List<string>(new string[] { "4", "2", "4", "-" }));
            datas.Add(new List<string>(new string[] { "4", "2", "5", "4" }));
            datas.Add(new List<string>(new string[] { "6", "1", "5", "-" }));

            var expected = new int[] { expectedBeginIndex, expectedEndIndex };

            // Act
            int[] result = DataProcessUtil.GetLongestIncreasingInterval(datas, fields, targetField, distinct);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
