using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazing.ReCaptcha.Extensions;
using Blazing.ReCaptcha.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

/// Inspired by: https://github.com/sample-by-jsakamoto/Blazor-UseGoogleReCAPTCHA
namespace Blazing.ReCaptcha
{
    public sealed partial class ReCaptcha : IDisposable
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
        ReCaptchaOptions _options;
        IDisposable _changeToken;
        int _recaptchaId;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _options = Options.CurrentValue;
                _changeToken = Options.OnChange(options => _options = options);

                await JavaScript.LoadReCaptchaAsync();
                _recaptchaId = await JavaScript.RenderReCaptchaAsync(
                    this,
                    ReCaptchaElementId,
                    _options.SiteKey);
            }
        }

        public ValueTask<string> GetResponseAsync() =>
            JavaScript.GetResponseAsync(_recaptchaId);

        [JSInvokable]
        public async Task OnEvaluated(string recaptchaResponse)
        {
            if (Evaluated.HasDelegate)
            {
                using (var content =
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["secret"] = _options.SecretKey,
                        ["response"] = recaptchaResponse
                    }))
                {
                    using (HttpClient httpClient = ClientFactory.CreateClient())
                    {
                        HttpResponseMessage response =
                            await httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
                        if (response != null)
                        {
                            response.EnsureSuccessStatusCode();
                        }
                        string json = await response.Content.ReadAsStringAsync();
                        var verification = json.FromJson<ReCaptchaVerificationResponse>();
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

        public void Dispose() => _changeToken?.Dispose();
    }
}
