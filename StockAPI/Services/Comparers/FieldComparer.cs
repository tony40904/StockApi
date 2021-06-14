using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace StockApi.Services.Comparers
{
    public abstract class FieldComparer : IComparer<List<string>>
    {
        public int FieldIndex { get; set; }

        public abstract int Compare([AllowNull] List<string> x, [AllowNull] List<string> y);
    }
}
