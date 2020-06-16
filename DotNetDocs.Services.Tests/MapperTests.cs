using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DotNetDocs.Services.Models;
using DotNetDocs.Web.Extensions;
using DotNetDocs.Web.PageModels;
using NodaTime;
using NodaTime.Text;
using Xunit;

namespace DotNetDocs.Services.Tests
{
    public class MapperTests
    {
        readonly IMapper _mapper;

        public MapperTests() =>
            _mapper = new MapperConfiguration(
                config => config.AddProfile(new MappingProfile())).CreateMapper();

        [Fact]
        public void PersonMapsCorrectlyToPersonModel()
        {
            var person = new Person
            {
                Email = "david.pine@microsoft.com",
                FirstName = "David",
                LastName = "Pine",
                TwitterHandle = "@davidpine7",
                IsBlueBadge = true
            };

            PersonModel personModel = _mapper.Map<PersonModel>(person);

            AssertPersonEqualsPersonModel(person, personModel);
        }

        void AssertPersonEqualsPersonModel(Person person, PersonModel personModel)
        {
            Assert.Equal(person.Email, personModel.Email);
            Assert.Equal(person.FirstName, personModel.FirstName);
            Assert.Equal(person.LastName, personModel.LastName);
            Assert.Equal(person.TwitterHandle, personModel.TwitterHandle);
            Assert.Equal(person.IsBlueBadge, personModel.IsBlueBadge);
            Assert.Equal(person.MicrosoftMvpId, personModel.MicrosoftMvpId);
        }

        [Fact]
        public void PersonModelMapsCorrectlyToPerson()
        {
            var personModel = new PersonModel
            {
                Email = "david.pine@microsoft.com",
                FirstName = "David",
                LastName = "Pine",
                TwitterHandle = "@davidpine7",
                IsBlueBadge = true
            };

            Person person = _mapper.Map<Person>(personModel);
            AssertPersonModelEqualsPerson(personModel, person);
        }

        void AssertPersonModelEqualsPerson(PersonModel personModel, Person person)
        {
            Assert.Equal(personModel.Email, person.Email);
            Assert.Equal(personModel.FirstName, person.FirstName);
            Assert.Equal(personModel.LastName, person.LastName);
            Assert.Equal(personModel.TwitterHandle, person.TwitterHandle);
            Assert.Equal(personModel.IsBlueBadge, person.IsBlueBadge);
            Assert.Equal(personModel.MicrosoftMvpId, person.MicrosoftMvpId);
        }

        [Fact]
        public void DocsShowMapsCorrectlyToShowModel()
        {
            var docsShow = new DocsShow
            {
                Date = DateTimeOffset.Parse("2020-07-02T11:00:00-05:00"),
                Title = "The amazing .NET docs show",
                Url = "https://www.twitch.tv/thedotnetdocs",
                ShowImage = "https://bitrebels.com/wp-content/uploads/2018/06/programming-languages-learn-header-image.jpg"
            };

            ShowModel showModel = _mapper.Map<ShowModel>(docsShow);

            Assert.Equal(docsShow.Id, showModel.Id);
            Assert.Equal(docsShow.Title, showModel.Title);
            Assert.Equal(docsShow.Url, showModel.Url);
            Assert.Equal(docsShow.ShowImage, showModel.ShowImage);
            Assert.Equal(docsShow.VideoId, showModel.VideoId);
            Assert.Equal(docsShow.Tags, showModel.Tags);

            AssertPeopleAreEqual(docsShow.Hosts, showModel.Hosts);
            AssertPeopleAreEqual(docsShow.Guests, showModel.Guests);
        }

        void AssertPeopleAreEqual(IEnumerable<Person> people, IEnumerable<PersonModel> personModels)
        {
            foreach ((Person p, PersonModel pm) in
                people.OrderBy(p => p.TwitterHandle).Zip(personModels.OrderBy(pm => pm.TwitterHandle)))
            {
                AssertPersonEqualsPersonModel(p, pm);
            }
        }

        [Fact]
        public void ShowModelMapsCorrectlyToDocsShow()
        {
            var showModel = new ShowModel
            {
                Title = "The amazing .NET docs show",
                Url = "https://www.twitch.tv/thedotnetdocs/7",
                ShowImage = "https://bitrebels.com/wp-content/uploads/2018/06/programming-languages-learn-header-image.jpg",
                VideoId = 7,
                Tags = new[] { "WinForms", "WPF" },
                Hosts = new[] { new PersonModel { FirstName = "Dave", LastName = "Pine" } },
                Guests = new[] { new PersonModel { FirstName = "Chino", LastName = "Moreno" } }
            };

            DocsShow docsShow = _mapper.Map<DocsShow>(showModel);

            Assert.Equal(showModel.Id, docsShow.Id);
            Assert.Equal(showModel.Title, docsShow.Title);
            Assert.Equal(showModel.Url, docsShow.Url);
            Assert.Equal(showModel.ShowImage, docsShow.ShowImage);
            Assert.Equal(showModel.VideoId, docsShow.VideoId);
            Assert.Equal(showModel.Tags, docsShow.Tags);

            AssertPeopleAreEqual(showModel.Hosts, docsShow.Hosts);
            AssertPeopleAreEqual(showModel.Guests, docsShow.Guests);
        }

        void AssertPeopleAreEqual(IEnumerable<PersonModel> personModels, IEnumerable<Person> people)
        {
            foreach ((PersonModel pm, Person p) in
                personModels.OrderBy(pm => pm.TwitterHandle).Zip(people.OrderBy(p => p.TwitterHandle)))
            {
                AssertPersonModelEqualsPerson(pm, p);
            }
        }

        [Fact]
        public void DateTimeOffsetConvertsCorrectlyToZonedDateTime()
        {
            var date = DateTimeOffset.Parse("2020-07-02T11:00:00-05:00");
            var zdt = _mapper.Map<ZonedDateTime>(date);

            Assert.Equal(date, zdt.ToDateTimeOffset());
        }

        [Fact]
        public void ZonedDateTimeConvertsCorrectlyToDateTimeOffset()
        {
            var zdt =
                ZonedDateTimePattern.CreateWithCurrentCulture("G", DateTimeZoneProviders.Tzdb)
                                    .Parse("2020-07-02T11:00:00 UTC-05 (-05)");
            Assert.True(zdt.Success);
            var date = _mapper.Map<DateTimeOffset>(zdt.Value);

            Assert.Equal(zdt.Value, ZonedDateTime.FromDateTimeOffset(date));
        }
    }
}
