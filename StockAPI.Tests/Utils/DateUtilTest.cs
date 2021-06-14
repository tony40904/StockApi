using System;
using System.Collections.Generic;
using NUnit.Framework;
using StockApi.Utils;

namespace StockApi.Tests.Utils
{
    [TestFixture]
    public class DateUtilTest
    {
        [TestCase("20210101")]
        [TestCase("00010101")]
        [TestCase("99990101")]
        [TestCase("20000229")]
        public void AcceptCheckFormat(string dateString)
        {
            // Arrange

            // Act
            bool result = DateUtil.CheckFormat(dateString);

            // Assert
            Assert.True(result);
        }

        [TestCase("20210001")]
        [TestCase("20210100")]
        [TestCase("20210132")]
        [TestCase("20211301")]
        [TestCase("1231301")]
        [TestCase("202101001")]
        [TestCase("19000229")]
        [TestCase("20210229")]
        [TestCase("aaaaaaaa")]
        [TestCase(null)]
        public void RejectCheckFormat(string dateString)
        {
            // Arrange

            // Act
            bool result = DateUtil.CheckFormat(dateString);

            // Assert
            Assert.False(result);
        }

        [TestCase(1)]
        public void TestGetPreviousDate(int n)
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(-n);
            var target = $"{date.ToString("yyyy")}{date.ToString("MM")}{date.ToString("dd")}";

            // Act
            string result = DateUtil.GetPreviousDate(n);

            // Assert
            Assert.AreEqual(target, result);
        }

        [TestCase("20210604", "110年06月04日")]
        public void TestStandardToChineseFormat(string date, string expected)
        {
            // Arrange

            // Act
            string result = DateUtil.StandardToChineseFormat(date);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestCase("110年06月04日", "20210604")]
        public void TestChineseToStandardFormat(string date, string expected)
        {
            // Arrange

            // Act
            string result = DateUtil.ChineseToStandardFormat(date);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestGetDateWithStandardFormat()
        {
            // Arrange
            DateTime expected = new DateTime(2021, 06, 01);
            var date = "20210601";
            // Act
            DateTime result = DateUtil.GetDate(date);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestGetDateWithChineseFormat()
        {
            // Arrange
            DateTime expected = new DateTime(2021, 06, 01);
            var date = "110年06月01日";
            // Act
            DateTime result = DateUtil.GetDate(date, isChinese: true);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestGetMonthsInRange()
        {
            // Arrange
            var start = "20201130";
            var end = "20210401";
            string[] expected = { "202011", "202012", "202101", "202102", "202103", "202104" };
            // Act
            var result = DateUtil.GetMonthsInRange(start, end);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
