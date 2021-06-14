using System;
using System.Collections.Generic;

namespace StockApi.Services.Comparers
{
    public class PbRatioComparer : FieldComparer
    {
        public PbRatioComparer()
        {
        }

        public override int Compare(List<string> x, List<string> y)
        {
            double PbRatioX = Double.Parse(x[FieldIndex]);
            double PbRatioY = Double.Parse(y[FieldIndex]);
            return -(PbRatioX.CompareTo(PbRatioY));
        }
    }
}
