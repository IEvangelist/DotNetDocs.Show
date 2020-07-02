namespace DotNetDocs.Extensions
{
    public static class NumberExtensions
    {
        public static (int remainder, int nearest) RoundUpToNearest(this int number, int roundTo)
        {
            int remainder = number % roundTo;
            int nearest = remainder == 0
                ? number
                : roundTo - remainder + number;

            return (remainder: remainder != 0 && remainder < roundTo ? roundTo - remainder : remainder, nearest);
        }
    }
}
