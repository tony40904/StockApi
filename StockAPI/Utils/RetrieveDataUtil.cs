using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StockApi.Models;

namespace StockApi.Utils
{
    public class RetrieveDataUtil
    {
        public RetrieveDataUtil()
        {
        }

        /// <summary>
        /// Frequently sending requests will cause our ip baned by TWSE.
        /// To avoid frequently sending requests, we should wait for a moment before sending a anew request.
        /// </summary>
        private static int _waitingTime = 0;
        private static bool _isSlow = false;
        private static DateTime _lastRequest = DateTime.Now;
        private static double _lastInterval = double.MaxValue;
        private static int _highFreqCount = 0;
        private static int _highFreqPeriod = 200;
        private static int _highFreqLimit = 16;
        private static double _lowFreqTime = 0;
        private static int _coolDownTime = 60000;
        private static int _lowFreqPeriod = 6000;

        /// <summary>
        /// Dynamic update the waiting time.
        /// Switch between 2 modes.
        /// 1. high frequency: Sending a request per <C>_highFreqPeriod</C> ms for at most <c>_highFreqLimit</c> times
        /// 2. low frequency: Sending a request per <c>_lowFreqPeriod</c> ms for at least <c>_coolDownTime</c> ms.
        /// </summary>
        private static void UpdateWaitingTime()
        {
            _lastInterval = DateTime.Now.Subtract(_lastRequest).TotalMilliseconds;
            _lastRequest = DateTime.Now;

            if (_lastInterval < _lowFreqPeriod)
            {
                _lowFreqTime = 0;
            }
            else
            {
                _lowFreqTime += _lastInterval;
            }

            if (_lowFreqTime > _coolDownTime)
            {
                _isSlow = false;
                _lowFreqTime = 0;
                _highFreqCount = 0;
            }
            

            if (_isSlow)
            {
                _waitingTime = _lowFreqPeriod;
                Console.Write($"Low frequency time: {_lowFreqTime:F0} ms. ");
            }
            else
            {
                _highFreqCount++;
                _waitingTime = _highFreqPeriod;
                Console.Write($"High frequency count: {_highFreqCount:D2}. ");
                
            }

            if (_highFreqCount >= _highFreqLimit)
            {
                _highFreqCount = 0;
                _isSlow = true;
            }
            Console.WriteLine($"Waiting: {_waitingTime} ms. interval: {_lastInterval:F0} ms");

        }

        /// <summary>
        /// Retrieve the whole month stock data of the given stock number.
        /// </summary>
        /// <param name="stockNo">the stock number you want to search</param>
        /// <param name="yearMonth">format: YYYYMM</param>
        /// <returns>the whole month data</returns>
        public static async Task<StockResult> RetrieveStockByNoAndMonth(string stockNo, string yearMonth)
        {
            var url = $"https://www.twse.com.tw/exchangeReport/BWIBBU?response=json&date={yearMonth}01&stockNo={stockNo}";
            Console.Write($"Month: {yearMonth}. ");

            string stockJson = await GetResponseByUrl(url);
            /*  {
             *      "stat":"",
             *      "date":"",
             *      "title":"",
             *      "fields":[],
             *      "data":[],
             *      "notes":""
             *  }
             */

            return JsonConvert.DeserializeObject<StockResult>(stockJson);
        }

        // date format: YYYYMMDD
        /// <summary>
        /// Retrieve all the stock data by the given date.
        /// </summary>
        /// <param name="date">format: YYYYMMDD</param>
        /// <returns>all the stock data of the given date</returns>
        public static async Task<StockResult> RetrieveStockByDate(string date)
        {
            var url = $"https://www.twse.com.tw/exchangeReport/BWIBBU_d?response=json&date={date}";

            string stockJson = await GetResponseByUrl(url);
            /*  {
             *      "stat":"",
             *      "date":"",
             *      "title":"",
             *      "fields":[],
             *      "data":[],
             *      "selectType":"",
             *      "notes":""
             *  }
             */

            return JsonConvert.DeserializeObject<StockResult>(stockJson);
        }

        /// <summary>
        /// send a http request by the url, resutrns the response as string 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static async Task<string> GetResponseByUrl(string url)
        {
            Thread.Sleep(_waitingTime);
            // Waiting for a moment to avoid frequently request.
            UpdateWaitingTime();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Credentials = CredentialCache.DefaultCredentials;

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string result = readStream.ReadToEnd();

            response.Close();
            readStream.Close();
            return result;
        }
    }
}
