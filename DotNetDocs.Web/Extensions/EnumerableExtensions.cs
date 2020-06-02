using System;
using System.Collections.Generic;

namespace DotNetDocs.Web.Extensions
{
    static class EnumerableExtensions
    {
        static readonly Random s_random = new Random((int)DateTime.Now.Ticks);

        internal static T RandomElement<T>(IReadOnlyList<T> array) =>
            array[s_random.Next(array.Count)];
    }
}
