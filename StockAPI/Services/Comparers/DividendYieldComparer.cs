using System;
using System.Collections.Generic;

namespace StockApi.Services.Comparers
{
    public class DividendYieldComparer : FieldComparer
    {
        public DividendYieldComparer()
        {
        }

        public override int Compare(List<string> x, List<string> y)
        {
            double dividendYieldX = Double.Parse(x[FieldIndex]);
            double dividendYieldY = Double.Parse(y[FieldIndex]);
            return -(dividendYieldX.CompareTo(dividendYieldY));
        }
    }
}
