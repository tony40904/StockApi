using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockApi.Models;
using StockApi.Utils;
using StockApi.Services.Comparers;

namespace StockApi.Services
{
    public class StockApiService : IStockApiService
    {
        public List<string> _standardFields = new List<string>(new string[] { "日期", "殖利率(%)", "股利年度", "本益比", "股價淨值比", "財報年/季" });

        /// <summary>
        /// Retrieve the stock data by stock number for the current n days.
        /// </summary>
        /// <param name="stockNo">the stock number</param>
        /// <param name="n">the number of days</param>
        /// <returns>A list of datas</returns>
        public async Task<Dictionary<string, Object>> RetrieveByStockNo(string stockNo, int n)
        {
            string start = DateUtil.GetPreviousDate(n);
            string end = DateTime.Today.ToString("yyyyMMdd");
            return await RetrieveByStockNo(stockNo, start, end);
        }

        /// <summary>
        /// Retrieve the stock data by stock number in the date range.
        /// </summary>
        /// <param name="stockNo">The stock number you want to search.</param>
        /// <param name="start">The first date of the range</param>
        /// <param name="end">the last date of the range (default: null)</param>
        /// <returns></returns>
        public async Task<Dictionary<string, Object>> RetrieveByStockNo(string stockNo, string start, string end)
        {
            Dictionary<string, Object> result = new Dictionary<string, object>();

            if (!DateUtil.CheckFormat(start) || !DateUtil.CheckFormat(end))
            {
                result.Add("stat", "Error: Date format is wrong");
                return result;
            }

            List<string> months = DateUtil.GetMonthsInRange(start, end);
            var datas = new List<List<string>>();
            foreach (var yearMonth in months)
            {
                StockResult stockResult = await RetrieveDataUtil.RetrieveStockByNoAndMonth(stockNo, yearMonth);
                if (!stockResult.Stat.Equals("OK"))
                {
                    break;
                }
                List<List<string>> curDatas = stockResult.Data;
                List<string> curFields = stockResult.Fields;

                // The data before 2017/04/11 don't have fields "股利年度" and "財報年/季".
                if (!curFields.Count.Equals(_standardFields.Count))
                {
                    DataProcessUtil.transformFields(curDatas, curFields, _standardFields);
                }

                datas.AddRange(curDatas);
            }

            DataProcessUtil.PruneData(datas, start, end);

            if (datas.Count == 0)
            {
                result.Add("stat", "No data");
                return result;
            }

            result.Add("stat", "OK");
            result.Add("fields", _standardFields);
            result.Add("datas", datas);

            return result;
        }

        /// <summary>
        /// Retrieve stocks of given date and sorted by given field.
        /// </summary>
        /// <param name="date">The date you want to search.</param>
        /// <param name="comparingField">The field used for sorting.</param>
        /// <returns>The key is an array of fields and the value is a list of stock datas.</returns>
        public async Task<Dictionary<string, Object>> RetrieveByDate(string date, string comparingField)
        {
            Dictionary<string, Object> result = new Dictionary<string, object>();

            if (!DateUtil.CheckFormat(date))
            {
                result.Add("stat", "Error: Date format is wrong");
                return result;
            }

            StockResult stockResult = await RetrieveDataUtil.RetrieveStockByDate(date);

            // If there is no data of the day, return an empty Pair.
            if (!stockResult.Stat.Equals("OK"))
            {
                result.Add("stat","No data");
                return result;
            }

            var fields = new List<string>(stockResult.Fields);
            int comparingFieldIndex = fields.IndexOf(comparingField);

            var datas = new List<List<string>>(stockResult.Data);

            // Sort the data by comparingField
            var comparerFactory = new ComparerFactory();
            FieldComparer comparer = comparerFactory.CreateComparer(comparingField);
            comparer.FieldIndex = comparingFieldIndex;
            datas.Sort(comparer);

            // Remove the data that not be sorted, only reserve the first 2 fields ("證券代號", "證券名稱") and comparingField
            var newFields = new List<string>(new string[] { "證券代號", "證券名稱", comparingField });
            DataProcessUtil.transformFields(datas, fields, newFields);

            result.Add("stat", "OK");
            result.Add("fields", newFields);
            result.Add("datas", datas);

            return result;
        }

        /// <summary>
        /// Between the given date range, find the longest interval that the given field data is increasing.
        /// </summary>
        /// <param name="stockNo">The stock number you want to search.</param>
        /// <param name="start">The first date of the date range.</param>
        /// <param name="end">The last date of the date range.</param>
        /// <param name="targetField">The field you want to search.</param>
        /// <param name="distinct">Consider distinctly increasing or not. Default: true</param>
        /// <returns>A dictionary that contains key "stat", "field", "begin", "end", "number of days", "number of trading days".</returns>
        public async Task<Dictionary<string, string>> GetIncreasingInterval(string stockNo, string start, string end, string targetField, bool distinct = true)
        {
            var result = new Dictionary<string, string>();

            if (!DateUtil.CheckFormat(start) || !DateUtil.CheckFormat(end))
            {
                result.Add("stat", "Error: Date format is wrong");
                return result;
            }

            Dictionary<string, Object> stockData = await RetrieveByStockNo(stockNo, start, end);

            var stat = (string)stockData.GetValueOrDefault("stat");
            if (!stat.Equals("OK"))
            {
                result.Add("stat", stat);
                return result;
            }

            var datas = (List<List<string>>)stockData.GetValueOrDefault("datas");
            var fields = (List<string>)stockData.GetValueOrDefault("fields");

            // Find the longest increasing interval
            int[] interval = DataProcessUtil.GetLongestIncreasingInterval(datas, fields, targetField, distinct);
            int beginIndex = interval[0];
            int endIndex = interval[1];

            if (endIndex > beginIndex)
            {
                string beginDateString = datas[beginIndex][0];
                string endDateString = datas[endIndex][0];
                DateTime beginDate = DateUtil.GetDate(beginDateString, isChinese: true);
                DateTime endDate = DateUtil.GetDate(endDateString, isChinese: true);

                result.Add("stat", "OK");
                result.Add("field", targetField);
                result.Add("begin", beginDateString);
                result.Add("end", endDateString);
                result.Add("number of days", endDate.Subtract(beginDate).TotalDays.ToString());
                result.Add("number of trading days", (endIndex - beginIndex).ToString());
            }
            else
            {
                result.Add("stat", "No data");
            }

            return result;
        }
    }
}
