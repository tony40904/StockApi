using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace StockApi.Utils
{
    public class DateUtil
    {
        /// <summary>
        /// Check the format of date string.
        /// </summary>
        /// <param name="dateString">The string to be checked.</param>
        /// <returns>True if the format is valid, otherwise false.</returns>
        public static bool CheckFormat(string dateString)
        {
            if(dateString == null || dateString.Length != 8)
            {
                return false;
            }
            try
            {
                int year = Int32.Parse(dateString.Substring(0, 4));
                int month = Int32.Parse(dateString.Substring(4, 2));
                int day = Int32.Parse(dateString.Substring(6, 2));
                DateTime dateTime = new DateTime(year, month, day);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the date that is the n-th day before today.
        /// </summary>
        /// <param name="n">the number of days</param>
        /// <returns>format: YYYYMMDD</returns>
        public static string GetPreviousDate(int n)
        {
            DateTime today = DateTime.Today;
            return today.AddDays(-n).ToString("yyyyMMdd");
        }

        /// <summary>
        /// Convert date from standard into Chinese format 
        /// </summary>
        /// <param name="date">the date you want to convert.(format: YYYYMMDD)</param>
        /// <returns>format: ROC_year年MM月DD日</returns>
        public static string StandardToChineseFormat(string date)
        {
            int year = Int32.Parse(date.Substring(0, 4)) - 1911;
            string month = date.Substring(4, 2);
            string day = date.Substring(6, 2);
            return $"{year}年{month}月{day}日";
        }

        /// <summary>
        /// Convert date from Chinese into standard format 
        /// </summary>
        /// <param name="date">the date you want to convert.(format: ROC_year年MM月DD日)</param>
        /// <returns>format YYYYMMDD</returns>
        public static string ChineseToStandardFormat(string date)
        {
            string[] splited = Regex.Split(date, @"[年月日]");
            int year = Int32.Parse(splited[0]) + 1911;
            string month = splited[1];
            string day = splited[2];
            return $"{year}{month}{day}";
        }

        /// <summary>
        /// Parse the date from string format into <c>DateTime</c>.
        /// </summary>
        /// <param name="date">format: YYYYMMDD</param>
        /// <param name="isChinese"><c>date</c> is Chinese format or not</param>
        /// <returns><c>DateTime</c> object of given date.</returns>
        public static DateTime GetDate(string date, Boolean isChinese = false)
        {
            int year = 0, month = 0, day = 0;
            if (!isChinese)
            {
                year = Int32.Parse(date.Substring(0, 4));
                month = Int32.Parse(date.Substring(4, 2));
                day = Int32.Parse(date.Substring(6, 2));
            }
            else
            {
                string[] splited = Regex.Split(date, @"[年月日]");
                year = Int32.Parse(splited[0]) + 1911;
                month = Int32.Parse(splited[1]);
                day = Int32.Parse(splited[2]);
            }

            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Get all months from the start dare to the end date.
        /// </summary>
        /// <param name="start">format: YYYYMMDD</param>
        /// <param name="end">format: YYYYMMDD.Default: the date of today</param>
        /// <returns>List of months.\nFormat: YYYYMM</returns>
        public static List<string> GetMonthsInRange(string start, string end = null)
        {
            List<string> months = new List<string>();

            // get the first month
            DateTime startMonth = GetDate(start);
            startMonth = new DateTime(startMonth.Year, startMonth.Month, 1);

            DateTime endMonth;
            if (end == null)
            {
                endMonth = DateTime.Today;
            }
            else
            {
                endMonth = GetDate(end);
            }

            DateTime iterDate = startMonth;
            while (iterDate <= endMonth)
            {
                months.Add(iterDate.ToString("yyyyMM"));
                iterDate = iterDate.AddMonths(1);
            }

            return months;
        }
    }
}
