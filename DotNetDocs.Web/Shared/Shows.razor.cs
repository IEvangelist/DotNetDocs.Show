using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetDocs.Services;
using DotNetDocs.Services.Models;
using Microsoft.AspNetCore.Components;

namespace DotNetDocs.Web.Shared
{
    public class ShowsComponent : ComponentBase
    {
        [Inject]
        public NavigationManager Navigation { get; set; } = null!;

        [Inject]
        public IScheduleService? ScheduleService { get; set; }

        public IEnumerable<DocsShow> Shows { get; set; } = null!;

        protected DocsShow? _show;
        protected bool _showModal = false;

        protected override async Task OnInitializedAsync()
        {
            if (ScheduleService != null)
            {
                Shows = await ScheduleService.GetAllAsync(DateTime.Now.AddYears(-10));
            }
        }

        protected void Cancel() => _showModal = false;

        protected async Task PerformDelete()
        {
            if (ScheduleService != null && _show != null)
            {
                await ScheduleService.DeleteShowAsync(_show.Id);
                Shows = Shows.Where(show => show != _show);
            }

            _showModal = false;
        }

        protected void OnConfirmDelete(DocsShow show)
        {
            _show = show;
            _showModal = true;
        }

        protected void NavigateToShowDetails(string showId) =>
            Navigation.NavigateTo($"/admin/show/{showId}");
    }
}
