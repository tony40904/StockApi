using System;
using System.Collections.Generic;
using System.Text;

namespace StockApi.Utils
{
    public class CsvUtil
    {
        /// <summary>
        /// Make CSV format string according to the datas.
        /// </summary>
        /// <param name="datas">The resource of CSV. The data at i-th row, j-th column is <c>datas[i][j]</c>.</param>
        /// <returns>The string of CSV.</returns>
        public static string MakeCsv(List<List<string>> datas)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var data in datas)
            {
                stringBuilder.Append("\"");
                stringBuilder.AppendJoin("\",\"", data);
                stringBuilder.Append("\"");
                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Make CSV format string according to the fields and datas.
        /// </summary>
        /// <param name="fields">The fields of <c>datas</c>.</param>
        /// <param name="datas">The resource of CSV. The data at i-th row, j-th column is <c>datas[i][j]</c>.</param>
        /// <returns>The string of CSV whose first row is the <c>fields</c></returns>
        public static string MakeCsv(List<string> fields, List<List<string>> datas)
        {
            datas.Insert(0, fields);
            return MakeCsv(datas);
        }
    }
}
