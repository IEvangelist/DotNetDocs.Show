﻿using System;
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
        AddPersonOption _option;
        bool _showModal = false;

        protected override async Task OnInitializedAsync()
        {
            if (ScheduleService != null && !string.IsNullOrWhiteSpace(ShowId))
            {
                _show = await ScheduleService.GetShowAsync(ShowId);
                if (_show != null)
                {
                    if (Enum.TryParse(PersonEmail, out _option))
                    {
                        _person = new Person();
                        Person = Mapper?.Map<PersonModel>(_person)!;
                    }
                    else
                    {
                        _person = _show.Guests.Concat(_show.Hosts).FirstOrDefault(p => p.Email.Contains(PersonEmail!, StringComparison.OrdinalIgnoreCase));
                        Person = Mapper?.Map<PersonModel>(_person)!;
                    }

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

        async ValueTask PerformDelete()
        {
            if (ScheduleService != null && _show != null && _person != null)
            {
                static bool FindPerson(Person person, Person other) =>
                    person.Email == other.Email &&
                    person.FirstName == other.FirstName &&
                    person.LastName == other.LastName;

                _show.Hosts = _show.Hosts.Except(_show.Hosts.Where(person => FindPerson(person, _person)));
                _show.Guests = _show.Guests.Except(_show.Guests.Where(person => FindPerson(person, _person)));

                await ScheduleService.UpdateShowAsync(_show);

                // Update cache
                var shows = await ScheduleService.GetAllAsync(DateTime.Now.Date.AddDays(-(20 * 7)));
                Cache.Set(CacheKeys.ShowSchedule, shows);
            }

            NavigateBack();
        }

        protected async ValueTask SubmitUpdatesAsync(EditContext context)
        {
            if (ScheduleService != null && _show != null)
            {
                if (TwitterService != null &&
                    Person.TwitterHandle != null &&
                    (Person.ImageUrl is null || Person.ImageUrl is { Length: 0 }))
                {
                    string? imageUrl = await TwitterService.GetUserProfileImageAsync(Person.TwitterHandle);
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

                IEnumerable<Person> AddToCollection(IEnumerable<Person> people)
                {
                    var peopleList = people.ToList();
                    if (peopleList != null)
                    {
                        peopleList.Add(Mapper?.Map(Person, new Person())!);
                    }
                    return peopleList!;
                }

                if (_option == default)
                {
                    UpdateCollection(_show.Hosts);
                    UpdateCollection(_show.Guests);
                }
                else
                {
                    _ = _option switch
                    {
                        AddPersonOption.Host => _show.Hosts = AddToCollection(_show.Hosts),
                        AddPersonOption.Guest => _show.Guests = AddToCollection(_show.Guests),
                        _ => throw new Exception("WTF")
                    };
                }

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

        void OnConfirmDelete() => _showModal = true;

        void Cancel() => _showModal = false;

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
