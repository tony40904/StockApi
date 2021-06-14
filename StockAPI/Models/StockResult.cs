using System.Collections.Generic;

namespace StockApi.Models
{
    public class StockResult
    {
        public StockResult()
        {
        }

        public string Stat { get; set; }
        public string Date { get; set; }
        public string Title { get; set; }
        public List<string> Fields { get; set; }
        public List<List<string>> Data { get; set; }
    }
}
