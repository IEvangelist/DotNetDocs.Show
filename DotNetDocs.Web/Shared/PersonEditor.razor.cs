using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DotNetDocs.Services;
using DotNetDocs.Services.Models;
using DotNetDocs.Web.PageModels;
using DotNetDocs.Web.Workers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetDocs.Web.Shared
{
    public partial class PersonEditor
    {
        [Inject]
        public IMapper? Mapper { get; set; }

        [Inject]
        public IScheduleService? ScheduleService { get; set; }

        [Inject]
        public TwitterService? TwitterService { get; set; }

        [Inject]
        public IMemoryCache? Cache { get; set; }

        [Inject]
        public NavigationManager? Navigation { get; set; }

        [Parameter]
        public string? ShowId { get; set; }

        [Parameter]
        public string? PersonEmail { get; set; }

        protected bool IsFormInvalid { get; set; }
        protected PersonModel Person { get; set; } = null!;

        EditContext? _editContext;
        DocsShow? _show;
        Person? _person;

        protected override async Task OnInitializedAsync()
        {
            if (ScheduleService != null && !string.IsNullOrWhiteSpace(ShowId))
            {
                _show = await ScheduleService.GetShowAsync(ShowId);
                if (_show != null)
                {
                    _person = _show.Guests.Concat(_show.Hosts).FirstOrDefault(p => p.Email == PersonEmail);
                    Person = Mapper?.Map<PersonModel>(_person)!;
                    _editContext = new EditContext(Person);
                    _editContext.OnFieldChanged += OnModelChanged;
                }
            }
        }

        void OnModelChanged(object? sender, FieldChangedEventArgs e)
        {
            IsFormInvalid = !_editContext?.Validate() ?? true;
            StateHasChanged();
        }

        protected async ValueTask SubmitUpdatesAsync(EditContext context)
        {
            if (ScheduleService != null && _show != null)
            {
                if (TwitterService != null &&
                    Person.TwitterHandle != null &&
                    (Person.ImageUrl is null || Person.ImageUrl is { Length: 0 }))
                {
                    string? imageUrl = TwitterService.GetUserProfileImage(Person.TwitterHandle);
                    if (!string.IsNullOrWhiteSpace(imageUrl))
                    {
                        Person.ImageUrl = imageUrl;
                    }
                }

                if (Person.ImageUrl != null)
                {
                    Person.ImageUrl = Person.ImageUrl.Replace("_normal", "_bigger");
                }

                void UpdateCollection(IEnumerable<Person> people)
                {
                    Person? person = people.FirstOrDefault(p => p.Email == PersonEmail);
                    if (person != null)
                    {
                        Mapper?.Map(Person, person);
                    }
                }

                UpdateCollection(_show.Hosts);
                UpdateCollection(_show.Guests);

                if (!(_show is null))
                {
                    await ScheduleService.UpdateShowAsync(_show);

                    // Update cache
                    var shows = await ScheduleService.GetAllAsync(DateTime.Now.Date.AddDays(-(20 * 7)));
                    Cache.Set(CacheKeys.ShowSchedule, shows);
                }
            }

            NavigateBack();
        }

        protected void NavigateBack()
        {
            if (Navigation == null)
            {
                return;
            }

            var uri = new Uri(Navigation.Uri);

            // /admin/show/{showId}/person/{personEmail}
            // We want /admin/show/{showId}
            IEnumerable<string>? showSegements = uri.Segments.Take(uri.Segments.Length - 2);

            Navigation.NavigateTo(string.Join("", showSegements));
        }

        public void Dispose() => _editContext!.OnFieldChanged -= OnModelChanged;
    }
}
