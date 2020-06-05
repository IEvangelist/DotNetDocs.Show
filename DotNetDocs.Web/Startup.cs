using System.Linq;
using System.Net.Mime;
using DotNetDocs.Services.Extensions;
using DotNetDocs.Web.Extensions;
using DotNetDocs.Web.Workers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Azure.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetDocs.Web
{
    public class Startup
    {
        readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => _configuration.Bind("AzureAd", options));

            services.AddControllersWithViews(options =>
                options.Filters.Add(
                    new AuthorizeFilter(new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build())));

            services.AddMemoryCache();
            services.AddServerSideBlazor();

            services.AddSignalR()
                .AddAzureSignalR(options =>
                    options.ServerStickyMode = ServerStickyMode.Required);

            services.AddProtectedBrowserStorage();
            services.AddResponseCompression(options =>
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { MediaTypeNames.Application.Octet }));
            services.AddDotNetDocsShowServices(_configuration);
            services.AddHostedService<ScheduleWorker>();
            services.AddAutoMapper();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
