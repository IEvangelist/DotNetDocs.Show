using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetDocs.Services;
using DotNetDocs.Services.Models;
using Microsoft.AspNetCore.Components;

namespace DotNetDocs.Web.Shared
{
    public partial class Shows
    {
        [Inject]
        public NavigationManager Navigation { get; set; } = null!;

        [Inject]
        public IScheduleService? ScheduleService { get; set; }

        public IEnumerable<DocsShow> Episodes { get; set; } = null!;

        DocsShow? _show;
        bool _showModal = false;

        protected override async Task OnInitializedAsync()
        {
            if (ScheduleService != null)
            {
                Episodes = await ScheduleService.GetAllAsync(DateTime.Now.AddYears(-10));
            }
        }

        void Cancel() => _showModal = false;

        async Task PerformDelete()
        {
            if (ScheduleService != null && _show != null)
            {
                await ScheduleService.DeleteShowAsync(_show.Id);
                Episodes = Episodes.Where(show => show != _show);
            }

            _showModal = false;
        }

        void OnConfirmDelete(DocsShow show)
        {
            _show = show;
            _showModal = true;
        }

        void NavigateToShowDetails(string showId) =>
            Navigation.NavigateTo($"/admin/show/{showId}");
    }
}
