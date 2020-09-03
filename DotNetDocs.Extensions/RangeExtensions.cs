using System;

namespace DotNetDocs.Extensions
{
    public static class RangeExtensions
    {
        public static bool IsInRange(this int value, Range range) =>
            range.Contains(value);

        public static bool Contains(this Range range, int value) =>
            value >= range.Start.Value && value <= range.End.Value;
    }
}
