using System;
using System.Threading.Tasks;
using AutoMapper;
using DotNetDocs.Services;
using DotNetDocs.Services.Models;
using DotNetDocs.Extensions;
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
        public LogicAppService? LogicAppService { get; set; }

        [Inject]
        public DateTimeService? DateTimeService { get; set; }

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
            if (ScheduleService != null && DateTimeService != null && !string.IsNullOrWhiteSpace(ShowId))
            {
                if (IsCreatingNewShow)
                {
                    var nxtThrsdy = DateTime.Today.AddDays(1).GetNextWeekday(DayOfWeek.Thursday);
                    var offset = DateTimeService.GetCentralTimeZoneOffset(nxtThrsdy);

                    Show = Mapper?.Map<ShowModel>(new DocsShow
                    {
                        Date = new DateTimeOffset(
                            nxtThrsdy.Year,
                            nxtThrsdy.Month,
                            nxtThrsdy.Day,
                            11, 0, 0,
                            offset)
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
                if (LogicAppService != null &&
                    _show.IsPublished &&
                    _show.IsInFuture &&
                    _show.IsScheduled &&
                    !_show.IsCalendarInviteSent)
                {
                    _show.IsCalendarInviteSent = await LogicAppService.CreateShowCalendarInviteAsync(_show);
                }

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
