using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DotNetDocs.Services;
using DotNetDocs.Web.PageModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;

namespace DotNetDocs.Web.Shared
{
    public partial class RequestShowPage
    {
        [Inject]
        public NavigationManager? Navigation { get; set; }

        [Inject]
        public DateTimeService? DateTimeService { get; set; }

        [Inject]
        public LogicAppService? RequestShowService { get; set; }

        [Inject]
        public ILogger<RequestShowPage>? Logger { get; set; }

        [Parameter]
        public string ShowDate { get; set; } = null!;

        protected bool IsRequested { get; set; }
        protected bool IsReCaptchaValid { get; set; }
        protected bool IsFormInvalid { get; set; }
        protected RequestModel RequestShow { get; set; } = null!;

        EditContext? _editContext;

        protected override void OnInitialized()
        {
            if (DateTimeService != null && !string.IsNullOrWhiteSpace(ShowDate))
            {
                int[]? parts = ShowDate.Split("-").Select(str => int.Parse(str, NumberStyles.HexNumber)).ToArray();
                (int month, int day, int year) = (parts[0], parts[1], parts[2]);
                RequestShow = new RequestModel
                {
                    ShowDate = new DateTimeOffset(
                        year, month, day, 11, 0, 0,
                        DateTimeService.GetCentralTimeZoneOffset(
                            new DateTime(year, month, day)))
                };

                _editContext = new EditContext(RequestShow);
                _editContext.OnFieldChanged += OnModelChanged;
            }
        }

        void OnModelChanged(object? sender, FieldChangedEventArgs e)
        {
            IsFormInvalid = !_editContext?.Validate() ?? true;
            StateHasChanged();
        }

        async ValueTask OnEvaluated((bool IsValid, string[] errors) tuple)
        {
            await InvokeAsync(() =>
            {
                (bool isValid, string[] errors) = tuple;
                IsReCaptchaValid = isValid;

                if (!IsReCaptchaValid)
                {
                    foreach (string? error in errors)
                    {
                        Logger?.LogWarning(error);
                    }
                }
            });
        }

        async ValueTask OnExpired() => await InvokeAsync(() => IsReCaptchaValid = false);

        protected async ValueTask SubmitUpdatesAsync(EditContext context)
        {
            if (RequestShowService != null)
            {
                IsRequested = await RequestShowService.RequestShowAsync(
                    RequestShow.ShowDate,
                    RequestShow.TentativeTitle,
                    RequestShow.Idea,
                    RequestShow.FirstName,
                    RequestShow.LastName,
                    RequestShow.Email,
                    RequestShow.TwitterHandle);
            }
        }

        protected void NavigateBack()
        {
            if (Navigation == null)
            {
                return;
            }

            Navigation.NavigateTo("/");
        }

        public void Dispose() => _editContext!.OnFieldChanged -= OnModelChanged;
    }
}
