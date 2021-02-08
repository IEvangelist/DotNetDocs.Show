using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetDocs.Extensions
{
    public static class EnumerableExtensions
    {
        static readonly Random s_random = new Random((int)DateTime.Now.Ticks);

        public static IEnumerable<T> Random<T>(this IEnumerable<T> source, int count) =>
            source?.OrderBy(_ => s_random.Next())?.Take(count) ?? Array.Empty<T>();

        public static T RandomElement<T>(IReadOnlyList<T> array) =>
            array[s_random.Next(array.Count)];

        public static IEnumerable<T> InterleaveWithAdaptor<T>(
            this IEnumerable<T> source,
            Func<T, DateTimeOffset> dateSelector,
            Func<DateTimeOffset, T> adaptSelector,
            TimeSpan expectedGap,
            int? nearestOfMultiple = null,
            Func<DateTimeOffset, bool>? filterPredicate = null)
        {
            DateTimeOffset previousDate = default;
            TimeSpan difference = default;

            var results = new List<T>();
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
                    difference = currentDate.Date - previousDate.Date;
                    if (difference.Duration() == expectedGap.Duration())
                    {
                        previousDate = currentDate;
                    }
                    else
                    {
                        DateTimeOffset nextDate = previousDate.Add(expectedGap);
                        while (nextDate.Date < currentDate.Date)
                        {
                            if (filterPredicate?.Invoke(nextDate) ?? true)
                            {
                                results.Add(adaptSelector(nextDate));
                            }
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
                    if (filterPredicate?.Invoke(previousDate) ?? true)
                    {
                        results.Add(adaptSelector(previousDate));
                    }
                }
            }

            return results;
        }
    }
}
