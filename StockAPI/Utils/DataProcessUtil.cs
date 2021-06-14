using System.Collections.Generic;

namespace StockApi.Utils
{
    public class DataProcessUtil
    {
        public DataProcessUtil()
        {
        }

        /// <summary>
        /// Original response from TWSE contains whole month data.
        /// This method is used to remove the data whose date is out of the given range.
        /// </summary>
        /// <param name="datas">original data</param>
        /// <param name="start">start date. Format: YYYYMMDD</param>
        /// <param name="end">end date. Format: YYYYMMDD. Default: null</param>
        public static void PruneData(List<List<string>> datas, string start = null, string end = null)
        {
            // Remove data before the start day
            if (start != null)
            {
                var startDate = DateUtil.GetDate(start);
                var removeCount = 0;
                foreach (var data in datas)
                {
                    var date = DateUtil.GetDate(data[0], isChinese: true);
                    if (startDate > date)
                    {
                        removeCount++;
                    }
                    else
                    {
                        break;
                    }
                }
                datas.RemoveRange(0, removeCount);
            }

            // Remove the data after the end day.
            if (end != null)
            {
                var endDate = DateUtil.GetDate(end);
                var removeCount = 0;
                for (int i = datas.Count - 1; i >= 0; i--)
                {
                    var date = DateUtil.GetDate(datas[i][0], isChinese: true);
                    if (endDate < date)
                    {
                        removeCount++;
                    }
                    else
                    {
                        break;
                    }
                }
                datas.RemoveRange(datas.Count - removeCount, removeCount);
            }
        }

        /// <summary>
        /// This method is used to transform fields of data.
        /// </summary>
        /// <param name="datas">Original datas from TWSE.</param>
        /// <param name="originalFields">Original fields from TWSE.</param>
        /// <param name="newFields">The fields that datas will transform to.</param>
        public static void transformFields(List<List<string>> datas, List<string> originalFields, List<string> newFields)
        {
            // Store original data in new array.
            List<List<string>> originalData = new List<List<string>>(datas);
            for (var i = 0; i < datas.Count; i++)
            {
                datas[i] = new List<string>(new string[newFields.Count]);
            }
            var index = 0;
            foreach (var field in newFields)
            {
                int originalIndex = originalFields.IndexOf(field);
                if (originalIndex >= 0)
                {
                    for (var i = 0; i < datas.Count; i++)
                    {
                        datas[i][index] = originalData[i][originalIndex];
                    }
                }
                index++;
            }
        }

        /// <summary>
        /// Find the longest interval that has increasing data of the given field.
        /// </summary>
        /// <param name="datas">The datas you want to process.</param>
        /// <param name="fields">The fields belong to <c>datas</c>.</param>
        /// <param name="targetField">The target field's name</param>
        /// <returns>An integer array of length 2.
        /// The first and second element are the begin and end index respectively.</returns>
        public static int[] GetLongestIncreasingInterval(List<List<string>> datas, List<string> fields, string targetField, bool distinct = true)
        {
            int[] interval = new int[2];
            int targetFieldIndex = fields.IndexOf(targetField);
            if (targetFieldIndex < 0)
            {
                return interval;
            }

            var curLeft = 0;
            var curRight = 0;
            var curValue = datas[0][targetFieldIndex] == "-"? double.MinValue : double.Parse(datas[0][targetFieldIndex]);
            while (curRight < datas.Count - 1)
            {
                double nextValue = datas[curRight + 1][targetFieldIndex] == "-" ? double.MinValue : double.Parse(datas[curRight + 1][targetFieldIndex]);
                if (curValue > nextValue || (distinct && curValue == nextValue))
                {
                    if (curRight - curLeft > interval[1] - interval[0])
                    {
                        interval[1] = curRight;
                        interval[0] = curLeft;
                    }
                    curLeft = curRight + 1;
                }
                curValue = nextValue;
                curRight++;
            }

            if (curRight - curLeft > interval[1] - interval[0])
            {
                interval[1] = curRight;
                interval[0] = curLeft;
            }

            return interval;
        }

    }
}
