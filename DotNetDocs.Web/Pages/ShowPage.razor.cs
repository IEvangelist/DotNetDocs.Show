using System.Threading.Tasks;
using DotNetDocs.Services;
using DotNetDocs.Services.Models;
using Microsoft.AspNetCore.Components;

namespace DotNetDocs.Web.Pages
{
    public partial class ShowPage
    {
        [Inject]
        public IScheduleService? ScheduleService { get; set; }

        [Parameter]
        public string ShowId { get; set; } = null!;

        bool _isShowStarting;
        DocsShow? _show;
        bool _noShow;

        protected override async Task OnInitializedAsync()
        {
            if (ScheduleService != null && ShowId != null)
            {
                _show = await ScheduleService.GetShowAsync(ShowId);
            }

            if (_show is null)
            {
                _noShow = true;
            }
        }

        async Task OnShowIsStarting(bool isStarting)
        {
            _isShowStarting = isStarting;
            await InvokeAsync(() => StateHasChanged());
        }
    }
}
