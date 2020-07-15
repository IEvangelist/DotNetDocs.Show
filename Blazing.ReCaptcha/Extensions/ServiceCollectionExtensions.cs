using Blazing.ReCaptcha.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blazing.ReCaptcha.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazorReCaptcha(
            this IServiceCollection services,
            IConfiguration configuration) =>
            services.AddHttpClient()
                    .Configure<ReCaptchaOptions>(
                configuration.GetSection(nameof(ReCaptchaOptions)));
    }
}
