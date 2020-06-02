using System.Collections.Generic;
using System.Linq;

namespace DotNetDocs.Web.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<TValue>> Chunk<TValue>(
            this IEnumerable<TValue> values,
            int chunkSize) =>
            values.Select((value, index) => (Value: value, GroupIndex: index / chunkSize))
                  .GroupBy(x => x.GroupIndex)
                  .Select(g => g.Select(x => x.Value));
    }
}
