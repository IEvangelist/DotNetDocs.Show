using System.Threading.Tasks;
using DotNetDocs.Services;
using DotNetDocs.Web.PageModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;

namespace DotNetDocs.Web.Pages
{
    public partial class SubmitIdeaPage
    {
        [Inject]
        public NavigationManager? Navigation { get; set; }

        [Inject]
        public LogicAppService? ShowIdeaService { get; set; }

        [Inject]
        public ILogger<SubmitIdeaPage>? Logger { get; set; }

        [Parameter]
        public string ShowDate { get; set; } = null!;

        protected bool IsRequested { get; set; }
        protected bool IsReCaptchaValid { get; set; }
        protected bool IsFormInvalid { get; set; }
        protected ShowIdeaModel ShowIdea { get; set; } = null!;

        EditContext? _editContext;

        protected override void OnInitialized()
        {
            if (ShowIdeaService != null)
            {
                ShowIdea = new ShowIdeaModel();

                _editContext = new EditContext(ShowIdea);
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

        protected async ValueTask SubmitUpdatesAsync(EditContext context)
        {
            if (ShowIdeaService != null)
            {
                if (!ShowIdea.TwitterHandle.StartsWith("@"))
                {
                    ShowIdea.TwitterHandle = $"@{ShowIdea.TwitterHandle}";
                }

                IsRequested = await ShowIdeaService.ProposeShowIdeaAsync(
                    ShowIdea.Idea,
                    ShowIdea.FirstName ?? "omitted",
                    ShowIdea.LastName ?? "excluded",
                    ShowIdea.Email,
                    ShowIdea.TwitterHandle ?? "non-existant");
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
