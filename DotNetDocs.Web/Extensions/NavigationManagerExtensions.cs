using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace DotNetDocs.Web.Extensions
{
    // Inspired by: https://chrissainty.com/working-with-query-strings-in-blazor/
    public static class NavigationManagerExtensions
    {
        public static bool TryGetQueryString<T>(
            this NavigationManager navManager, string key, out T value) where T : struct
        {
            var uri = navManager.ToAbsoluteUri(navManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue(key, out var valueFromQueryString))
            {
                var type = typeof(T);
                if (type.IsEnum &&
                    Enum.IsDefined(type, valueFromQueryString) &&
                    Enum.TryParse<T>(valueFromQueryString, out var valueAsEnum))
                {
                    value = valueAsEnum;
                    return true;
                }

                if (type == typeof(int) && int.TryParse(valueFromQueryString, out var valueAsInt))
                {
                    value = (T)(object)valueAsInt;
                    return true;
                }

                if (type == typeof(string))
                {
                    value = (T)(object)valueFromQueryString.ToString();
                    return true;
                }
            }

            value = default!;
            return false;
        }
    }

}
