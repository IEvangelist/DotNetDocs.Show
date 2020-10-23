using static System.Text.Json.JsonSerializer;

namespace Blazing.ReCaptcha.Extensions
{
    static class StringExtensions
    {
        internal static T FromJson<T>(this string json) =>
            string.IsNullOrWhiteSpace(json)
            ? default
            : Deserialize<T>(json);
    }
}
