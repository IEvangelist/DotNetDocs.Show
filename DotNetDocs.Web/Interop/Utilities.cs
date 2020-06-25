using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace DotNetDocs.Web.Interop
{
    public static class Utilities
    {
        static readonly Lazy<ValueTask> s_nopTask = new Lazy<ValueTask>(() => new ValueTask());

        public static ValueTask ScrollIntoViewAsync(
            this IJSRuntime jSRuntime, string selector) =>
            jSRuntime?.InvokeVoidAsync("utilities.scrollIntoView", selector) ?? s_nopTask.Value;

        public static ValueTask NudgeTwitterAsync(
            this IJSRuntime jSRuntime) =>
            jSRuntime?.InvokeVoidAsync("utilities.nudgeTwitter") ?? s_nopTask.Value;
    }
}
