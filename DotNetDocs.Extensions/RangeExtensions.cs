using System;

namespace DotNetDocs.Extensions
{
    public static class RangeExtensions
    {
        public static bool IsInRange(this Range range, int value) =>
            value >= range.Start.Value && value <= range.End.Value;
    }
}
