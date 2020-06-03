using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetDocs.Services;
using DotNetDocs.Services.Models;
using Microsoft.AspNetCore.Components;

namespace DotNetDocs.Web.Shared
{
    public class ShowsComponent : ComponentBase
    {
        [Inject]
        public IScheduleService? ScheduleService { get; set; }

        public IEnumerable<DocsShow> Shows { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            if (ScheduleService != null)
            {
                Shows = await ScheduleService.GetAllAsync(DateTime.Now.AddYears(-10));
            }
        }
    }
}
