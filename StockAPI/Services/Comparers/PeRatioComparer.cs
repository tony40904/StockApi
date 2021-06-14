using System;
using System.Collections.Generic;

namespace StockApi.Services.Comparers
{
    public class PeRatioComparer : FieldComparer
    {
        public PeRatioComparer()
        {
        }

        public override int Compare(List<string> x, List<string> y)
        {
            if (x.Equals(y)) {
                return 0;
            }
            double peRatioX;
            double peRatioY;
            if (x[FieldIndex].Equals("-"))
            {
                peRatioX = Int32.MinValue;
            }
            else
            {
                peRatioX = Double.Parse(x[FieldIndex]);
            }
            if (y[FieldIndex].Equals("-"))
            {
                peRatioY = Int32.MinValue;
            }
            else
            {
                peRatioY = Double.Parse(y[FieldIndex]);
            }
            return -(peRatioX.CompareTo(peRatioY));
        }
    }
}
