using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Blazing.ReCaptcha.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace Blazing.ReCaptcha
{
    public sealed partial class ReCaptcha
    {
        [Inject]
        public IJSRuntime JavaScript { get; set; }

        [Inject]
        public IOptionsMonitor<ReCaptchaOptions> Options { get; set; }

        [Inject]
        public IHttpClientFactory ClientFactory { get; set; }

        [Parameter]
        public EventCallback<(bool IsValid, string[] Errors)> Evaluated { get; set; }

        string ReCaptchaElementId => "blazor-recaptcha";
        int _recaptchaId;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JsInterop.LoadAsync(JavaScript);
                _recaptchaId = await JsInterop.RenderReCaptchaAsync(
                    JavaScript,
                    this,
                    ReCaptchaElementId,
                    Options.CurrentValue.SiteKey);
            }
        }

        public ValueTask<string> GetResponseAsync() =>
            JsInterop.GetResponseAsync(JavaScript, _recaptchaId);

        [JSInvokable]
        public async Task OnEvaluated(string recaptchaResponse)
        {
            if (Evaluated.HasDelegate)
            {
                using (var content = new FormUrlEncodedContent(new Dictionary<string, string>
                                     {
                                         ["secret"] = Options.CurrentValue.SecretKey,
                                         ["response"] = recaptchaResponse
                                     }))
                {
                    HttpClient httpClient = ClientFactory.CreateClient();
                    HttpResponseMessage response =
                        await httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
                    if (response != null)
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    string json = await response.Content.ReadAsStringAsync();
                    var verification = JsonSerializer.Deserialize<ReCaptchaVerificationResponse>(json);
                    if (verification?.Success ?? false)
                    {
                        await Evaluated.InvokeAsync(
                            (IsValid: true, Errors: new string[0]));
                    }
                    else
                    {
                        await Evaluated.InvokeAsync(
                            (IsValid: false, Errors: verification.ErrorCodes.Select(err => err.Replace('-', ' ')).ToArray()));
                    }
                }
            }
        }

        [JSInvokable]
        public async Task OnExpired()
        {
            if (Evaluated.HasDelegate)
            {
                await Evaluated.InvokeAsync(
                    (IsValid: false, Errors: new string[] { "reCAPTCHA has expired, and would need to be reevaluated." }));
            }
        }
    }
}
