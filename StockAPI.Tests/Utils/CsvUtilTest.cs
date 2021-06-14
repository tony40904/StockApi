using System;
using System.Collections.Generic;
using NUnit.Framework;
using StockApi.Utils;

namespace StockApi.Tests.Utils
{
    [TestFixture]
    public class CsvUtilTest
    {
        [Test]
        public void TestMakeCsv()
        {
            // Arrange
            var datas = new List<List<string>>(new List<string>[4]);
            datas[0] = new List<string>(new string[] { "a", "b", "c" });
            datas[1] = new List<string>(new string[] { "d", "e", "f" });
            datas[2] = new List<string>(new string[] { "g", "h", "i" });
            datas[3] = new List<string>(new string[] { "j", "k", "l" });
            var target =
                "\"a\",\"b\",\"c\"\n" +
                "\"d\",\"e\",\"f\"\n" +
                "\"g\",\"h\",\"i\"\n" +
                "\"j\",\"k\",\"l\"\n";

            // Act
            var result = CsvUtil.MakeCsv(datas);

            // Assert
            Assert.AreEqual(target, result);
        }

        [Test]
        public void TestMakeCsvWithFields()
        {
            // Arrange
            var fields = new List<string>( new string[] { "x", "y", "z" });
            var datas = new List<List<string>>(new List<string>[3]);
            datas[0] = new List<string>(new string[] { "a", "b", "c" });
            datas[1] = new List<string>(new string[] { "d", "e", "f" });
            datas[2] = new List<string>(new string[] { "g", "h", "i" });
            var target =
                "\"x\",\"y\",\"z\"\n" +
                "\"a\",\"b\",\"c\"\n" +
                "\"d\",\"e\",\"f\"\n" +
                "\"g\",\"h\",\"i\"\n";

            // Act
            var result = CsvUtil.MakeCsv(fields, datas);

            // Assert
            Assert.AreEqual(target, result);
        }
    }
}
