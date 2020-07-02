using System.Collections.Generic;

namespace DotNetDocs.Services.Models
{
    public class SegmentedShows
    {
        public IEnumerable<DocsShow>? PastShows { get; }

        public DocsShow? NextShow { get; }

        public IEnumerable<DocsShow>? FutureShows { get; }

        public SegmentedShows(
            IEnumerable<DocsShow>? pastShows,
            DocsShow? nextShow,
            IEnumerable<DocsShow>? futureShows) =>
            (PastShows, NextShow, FutureShows) = (pastShows, nextShow, futureShows);
    }
}
