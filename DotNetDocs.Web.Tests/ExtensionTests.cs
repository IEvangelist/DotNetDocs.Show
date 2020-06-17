using Xunit;
using DotNetDocs.Web.Extensions;

namespace DotNetDocs.Web.Tests
{
    public class ExtensionTests
    {
        [
            Theory,
            InlineData(2, 7, 1, false),
            InlineData(2, 7, 2, true),
            InlineData(2, 7, 3, true),
            InlineData(2, 7, 4, true),
            InlineData(2, 7, 5, true),
            InlineData(2, 7, 6, true),
            InlineData(2, 7, 7, true),
            InlineData(2, 7, 8, false)
        ]
        public void IsInRangeTest(int start, int end, int number, bool expected) =>
            Assert.Equal(expected, (start..end).IsInRange(number));
    }
}
