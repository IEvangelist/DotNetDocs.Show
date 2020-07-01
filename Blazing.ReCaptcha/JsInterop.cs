using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Blazing.ReCaptcha
{
    public class JsInterop
    {
        public static ValueTask LoadAsync(
            IJSRuntime jsRuntime) =>
            jsRuntime.InvokeVoidAsync("recaptcha.load");

        public static ValueTask<int> RenderReCaptchaAsync<T>(
            IJSRuntime jsRuntime,
            T instance,
            string elementId,
            string siteKey) where T : class =>
            jsRuntime.InvokeAsync<int>(
                "recaptcha.render", DotNetObjectReference.Create(instance), elementId, siteKey);

        public static ValueTask<string> GetResponseAsync(
            IJSRuntime jSRuntime,
            int recaptchaId) =>
            jSRuntime.InvokeAsync<string>(
                "recaptcha.getResponse", recaptchaId);
    }
}
