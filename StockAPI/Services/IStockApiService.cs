using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApi.Services
{
    public interface IStockApiService
    {
        public Task<Dictionary<string, Object>> RetrieveByStockNo(string stockNo, int n);
        public Task<Dictionary<string, Object>> RetrieveByStockNo(string stockNo, string start, string end);
        public Task<Dictionary<string, Object>> RetrieveByDate(string date, string comparingField);
        public Task<Dictionary<string, string>> GetIncreasingInterval(string stockNo, string start, string end, string targetField, bool distinct = true);
    }

}