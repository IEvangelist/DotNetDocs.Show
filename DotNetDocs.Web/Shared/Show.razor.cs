using System;
using System.Threading.Tasks;
using AutoMapper;
using DotNetDocs.Services;
using DotNetDocs.Services.Models;
using DotNetDocs.Web.Extensions;
using DotNetDocs.Web.PageModels;
using DotNetDocs.Web.Workers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetDocs.Web.Shared
{
    public class ShowComponent : ComponentBase
    {
        [Inject]
        public IMapper? Mapper { get; set; }

        [Inject]
        public IScheduleService? ScheduleService { get; set; }

        [Inject]
        public IMemoryCache? Cache { get; set; }

        [Inject]
        public NavigationManager? Navigation { get; set; }

        [Parameter]
        public string? ShowId { get; set; }

        protected string SelectedShowId { get; set; } = null!;
        protected int SelectedVideoId { get; set; }
        protected bool IsFormInvalid { get; set; }
        protected ShowModel Show { get; set; } = null!;

        EditContext? _editContext;
        DocsShow? _show;

        bool IsCreatingNewShow => ShowId == "create";

        protected override async Task OnInitializedAsync()
        {
            if (ScheduleService != null && !string.IsNullOrWhiteSpace(ShowId))
            {
                if (IsCreatingNewShow)
                {
                    var nxtThrsdy = DateTime.Today.AddDays(1).GetNextWeekday(DayOfWeek.Thursday);
                    Show = Mapper?.Map<ShowModel>(new DocsShow
                    {
                        Date = DateTimeOffset.Parse(
                            $"{nxtThrsdy.Year}-{nxtThrsdy.Month:00}-{nxtThrsdy.Day:00}T11:00:00-05:00")
                    })!;
                }
                else
                {
                    _show = await ScheduleService.GetShowAsync(ShowId);
                    Show = Mapper?.Map<ShowModel>(_show)!;
                }
                
                _editContext = new EditContext(Show);
                _editContext.OnFieldChanged += OnModelChanged;
            }
        }

        void OnModelChanged(object? sender, FieldChangedEventArgs e)
        {
            IsFormInvalid = !_editContext?.Validate() ?? true;
            StateHasChanged();
        }

        protected async ValueTask SubmitShowUpdatesAsync(EditContext context)
        {
            if (ScheduleService != null && (_show = Mapper?.Map<DocsShow>(Show)) != null)
            {
                if (IsCreatingNewShow)
                {
                    await ScheduleService.CreateShowAsync(_show);
                }
                else
                {
                    await ScheduleService.UpdateShowAsync(_show);
                }

                // Update cache
                var shows = await ScheduleService.GetAllAsync(DateTime.Now.Date.AddDays(-(20 * 7)));
                Cache.Set(CacheKeys.ShowSchedule, shows);
            }

            NavigateBack();
        }

        protected void OnSelectShowThumbnail()
        {
            SelectedShowId = ShowId!;
            SelectedVideoId = Show.VideoId!.Value;

            StateHasChanged();
        }

        protected void OnThumbnailChanged(string thumbnailUrl)
        {
            Show.ShowImage = thumbnailUrl;

            StateHasChanged();
        }

        protected void OnEditPerson(string email) =>
            Navigation?.NavigateTo($"admin/show/{ShowId}/person/{email}");

        protected void NavigateBack() => Navigation?.NavigateTo("admin");

        public void Dispose() => _editContext!.OnFieldChanged -= OnModelChanged;
    }
}
