using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Blazing.ReCaptcha
{
    public static class JsInterop
    {
        /// <summary>
        /// Call before <see cref="RenderReCaptchaAsync{T}(IJSRuntime, T, string, string)"/>.
        /// </summary>
        public static ValueTask LoadReCaptchaAsync(
            this IJSRuntime jsRuntime) =>
            jsRuntime.InvokeVoidAsync("recaptcha.load");

        /// <summary>
        /// Call after <see cref="LoadReCaptchaAsync(IJSRuntime)"/>
        /// </summary>
        public static ValueTask<int> RenderReCaptchaAsync<T>(
            this IJSRuntime jsRuntime,
            T instance,
            string elementId,
            string siteKey) where T : class =>
            jsRuntime.InvokeAsync<int>(
                "recaptcha.render", DotNetObjectReference.Create(instance), elementId, siteKey);

        public static ValueTask<string> GetResponseAsync(
            this IJSRuntime jSRuntime,
            int recaptchaId) =>
            jSRuntime.InvokeAsync<string>(
                "recaptcha.getResponse", recaptchaId);
    }
}
