using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace DotNetDocs.Web.Interop
{
    public static class Utilities
    {
        static readonly Lazy<ValueTask> s_nopTask = new Lazy<ValueTask>(() => new ValueTask());

        public static ValueTask ScrollIntoViewAsync(
            this IJSRuntime jSRuntime, string selector)
        {
            try
            {
                return jSRuntime?.InvokeVoidAsync("utilities.scrollIntoView", selector) ?? s_nopTask.Value;
            }
            catch
            {
                return s_nopTask.Value;
            }
        }

        public static ValueTask NudgeTwitterAsync(
            this IJSRuntime jSRuntime)
        {
            try
            {
                return jSRuntime?.InvokeVoidAsync("utilities.nudgeTwitter") ?? s_nopTask.Value;
            }
            catch
            {
                return s_nopTask.Value;
            }
        }

        public static ValueTask LoadTwitterImagesAsync(
            this IJSRuntime jSRuntime)
        {
            try
            {
                return jSRuntime?.InvokeVoidAsync("utilities.loadTwitterImages") ?? s_nopTask.Value;
            }
            catch
            {
                return s_nopTask.Value;
            }
        }
    }
}
