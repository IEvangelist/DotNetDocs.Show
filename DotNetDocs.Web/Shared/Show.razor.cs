using System.Threading.Tasks;
using AutoMapper;
using DotNetDocs.Services;
using DotNetDocs.Services.Models;
using DotNetDocs.Web.PageModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DotNetDocs.Web.Shared
{
    public class ShowComponent : ComponentBase
    {
        [Inject]
        public IMapper? Mapper { get; set; }

        [Inject]
        public IScheduleService? ScheduleService { get; set; }

        [Inject]
        public NavigationManager? Navigation { get; set; }

        [Parameter]
        public string? ShowId { get; set; }

        protected bool IsDisabled { get; set; }
        protected ShowModel Show { get; set; } = null!;

        DocsShow? _show;

        protected override async Task OnInitializedAsync()
        {
            if (ScheduleService != null && !string.IsNullOrWhiteSpace(ShowId))
            {
                _show = await ScheduleService.GetShowAsync(ShowId);
                Show = Mapper?.Map<ShowModel>(_show)!;
            }
        }

        protected async ValueTask SubmitShowUpdatesAsync(EditContext context)
        {
            IsDisabled = true;

            if (context.IsModified() && ScheduleService != null)
            {
                _show = Mapper?.Map<DocsShow>(Show);
                if (!(_show is null))
                {
                    await ScheduleService.UpdateShowAsync(_show);
                }
            }

            NavigateBack();
        }

        protected void NavigateBack() => Navigation?.NavigateTo("admin");
    }
}
