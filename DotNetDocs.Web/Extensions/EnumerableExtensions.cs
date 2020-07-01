using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetDocs.Web.Extensions
{
    public static class EnumerableExtensions
    {
        static readonly Random s_random = new Random((int)DateTime.Now.Ticks);

        public static T RandomElement<T>(IReadOnlyList<T> array) =>
            array[s_random.Next(array.Count)];

        public static IEnumerable<object> InterleaveWithAdaptor<T>(
            this IEnumerable<T> source,
            Func<T, DateTimeOffset> dateSelector,
            Func<DateTimeOffset, object> adaptSelector,
            TimeSpan expectedGap,
            int? nearestOfMultiple = null)
        {
            DateTimeOffset previousDate = default;
            TimeSpan difference = default;

            var results = new List<object>();
            foreach ((T value, int index) in
                source.OrderBy(value => dateSelector(value)).Select((value, index) => (value, index)))
            {
                if (index == 0)
                {
                    previousDate = dateSelector(value);
                }
                else
                {
                    DateTimeOffset currentDate = dateSelector(value);
                    difference = currentDate - previousDate;
                    if (difference.Duration() == expectedGap.Duration())
                    {
                        previousDate = currentDate;
                    }
                    else
                    {
                        DateTimeOffset nextDate = previousDate.Add(expectedGap);
                        while (nextDate < currentDate)
                        {
                            results.Add(adaptSelector(nextDate));
                            nextDate = nextDate.Add(expectedGap);
                        }
                        
                        previousDate = currentDate;
                    }
                }

                results.Add(value);
            }

            if (nearestOfMultiple.HasValue)
            {
                (int _, int nearest) = results.Count.RoundUpToNearest(nearestOfMultiple.Value);
                while (results.Count < nearest)
                {
                    previousDate = previousDate.Add(expectedGap);
                    results.Add(adaptSelector(previousDate));
                }
            }

            return results;
        }
    }
}
