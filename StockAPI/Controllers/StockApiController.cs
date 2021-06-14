using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockApi.Services;
using StockApi.Utils;

namespace StockApi.Controllers
{
    [ApiController]
    [Route("api/stock")]
    public class StockApiController : Controller
    {

        private Dictionary<string, string> _fieldNameMap;
        private IStockApiService _stockApiService = new StockApiService();

        public StockApiController(IStockApiService stockApiService = null)
        {
            if (stockApiService != null)
            {
                _stockApiService = stockApiService;
            }
            _fieldNameMap = new Dictionary<string, string>();
            _fieldNameMap.Add("PER", "本益比");
            _fieldNameMap.Add("DY", "殖利率(%)");
            _fieldNameMap.Add("PBR", "股價淨值比");
        }

        /// <summary>
        /// Retrieve the stock data by stock number for the current n days.
        /// url: api/stock/number/{stockNo}?[n=10]
        /// </summary>
        /// <param name="stockNo">The stock number you want to search.</param>
        /// <param name="n">The number of days you want to search. Default = 10</param>
        /// <returns>CSV format of data.</returns>
        [HttpGet("number/{stockNo}")]
        public async Task<string> GetStocksForCurrentNDays(string stockNo, int n = 10)
        {
            if (n <= 0)
            {
                return "N should be a positive integer.";
            }

            DateTime startTime = DateTime.Now;
            Dictionary<string, Object> result = await _stockApiService.RetrieveByStockNo(stockNo, n);

            var stat = (string)result.GetValueOrDefault("stat");
            if (!stat.Equals("OK"))
            {
                return stat;
            }

            var fields = (List<string>)result.GetValueOrDefault("fields");
            var datas = (List<List<string>>)result.GetValueOrDefault("datas");

            string csv = CsvUtil.MakeCsv(fields, datas);

            TimeSpan excutionTime = DateTime.Now.Subtract(startTime);
            Console.WriteLine($"Finished: Retrieve stock data {stockNo} for current {n} days.");
            Console.WriteLine($"Excution time: {excutionTime.ToString(@"mm\:ss\.ff")}");

            return csv;
        }

        /// <summary>
        /// Retrieve stocks of given date and sorted by given field.
        /// url: api/stock/date/{date}?[n=10][&sortBy=PER][&reverse=false]
        /// </summary>
        /// <param name="date">The date you eant to search.</param>
        /// <param name="n">The number you want to display.</param>
        /// <param name="sortBy">The field you want to sort.</param>
        /// <param name="reverse">Reverse order or not.</param>
        /// <returns>CSV format of data.</returns>
        [HttpGet("date/{date}")]
        public async Task<string> GetSortedStocks(string date, int n = 10, string sortBy = "PER", bool reverse = false)
        {
            if (n <= 0)
            {
                return "N should be a positive integer.";
            }

            DateTime startTime = DateTime.Now;

            sortBy = _fieldNameMap.GetValueOrDefault(sortBy, null);

            if (sortBy == null)
            {
                return "Invalid sortBy value";
            }

            Dictionary<string, Object> result = await _stockApiService.RetrieveByDate(date, sortBy);

            var stat = (string)result.GetValueOrDefault("stat");
            if (!stat.Equals("OK"))
            {
                return stat;
            }

            var fields = (List<string>)result.GetValueOrDefault("fields");
            var datas = (List<List<string>>)result.GetValueOrDefault("datas");

            if (reverse)
            {
                datas.Reverse();
            }

            n = Math.Min(n, datas.Count);

            TimeSpan excutionTime = DateTime.Now.Subtract(startTime);
            Console.WriteLine($"Finished: Retrieve stock data on {date}, display the {(reverse?"bottom":"top")} {n} of \"{sortBy}\"");
            Console.WriteLine($"Excution time: {excutionTime.ToString(@"mm\:ss\.ff")}");

            string csv = CsvUtil.MakeCsv(fields, datas.GetRange(0, n));

            return csv;
        }

        /// <summary>
        /// Between the given date range, find the longest interval that the given field data is increasing.
        /// url: api/stock/number/{stockNo}/interval/{field}?from=20200101&to=20210130[&distinct=true]
        /// </summary>
        /// <param name="stockNo">The stock number you want to search.</param>
        /// <param name="field">The field you want to search.</param>
        /// <param name="from">The first date of the date range.</param>
        /// <param name="to">The last date of the date range.</param>
        /// <param name="distinct">Consider distinctly increasing or not. Default: true</param>
        /// <returns>A dictionary that contains key "stat", "field", "begin", "end", "number of days", "number of trading days".</returns>
        [HttpGet("number/{stockNo}/interval/{field}")]
        public async Task<Dictionary<string, string>> GetIncreacingInterval(string stockNo, string field, string from, string to, bool distinct = true)
        {
            DateTime startTime = DateTime.Now;

            field = _fieldNameMap.GetValueOrDefault(field, null);
            if (field == null)
            {
                var err = new Dictionary<string, string>();
                err.Add("stat", "Invalid field value");
                return err;
            }

            Dictionary<string, string> result = await _stockApiService.GetIncreasingInterval(stockNo, from, to, field, distinct);

            TimeSpan excutionTime = DateTime.Now.Subtract(startTime);
            Console.WriteLine($"Finished: Obtain the longest interval of {stockNo} between {from} and {to} that \"{field}\" increasing{(distinct ? " distinctly" : "")}.");
            Console.WriteLine($"Excution time: {excutionTime.ToString(@"mm\:ss\.ff")}");

            return result;
        }

    }
}
