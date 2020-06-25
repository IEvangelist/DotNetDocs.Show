using System;
using System.Runtime.InteropServices;
using System.Timers;
using Microsoft.AspNetCore.Components;

namespace DotNetDocs.Web.Shared
{
    public class CountDownTimerComponent : ComponentBase, IDisposable
    {
        Timer _timer;
        readonly TimeZoneInfo _centralTimeZone;

        [Parameter]
        public DateTime ShowTime { get; set; }

        [Parameter]
        public EventCallback<bool> ShowIsStarting { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; } = null!;

        protected TimeSpan TimeRemaining { get; private set; }
        protected string ImminentClass { get; private set; } = "";

        public CountDownTimerComponent()
        {
            _centralTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(
                    RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                        ? "America/Chicago"
                        : "Central Standard Time");

            _timer = new Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
        }

        protected override void OnParametersSet()
        {
            _timer.Start();

            base.OnParametersSet();
        }

        async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, _centralTimeZone);
            TimeRemaining = ShowTime.Subtract(dateTime);

            bool alreadyStarted = TimeRemaining.Ticks < 0;
            if (alreadyStarted)
            {
                // Show is over
                if (TimeRemaining >= TimeSpan.FromMinutes(-61))
                {
                    StopTimerAndUnregisterHandler();
                    Navigation.NavigateTo("/", true);
                }
            }
            else
            {
                if (TimeRemaining <= TimeSpan.FromSeconds(30))
                {
                    // Starts in 30 seconds, show embedded Twitch stream
                    ImminentClass = "";
                    await InvokeAsync(
                        async () => await ShowIsStarting.InvokeAsync(true));
                }
                else if (TimeRemaining <= TimeSpan.FromSeconds(90))
                {
                    // Starts in 90 seconds, start blinking
                    ImminentClass = "blinking";
                    await InvokeAsync(() => StateHasChanged());
                }
                else
                {
                    // Show hasn't started, nor is it imminent, just keep ticking
                    await InvokeAsync(() => StateHasChanged());
                }
            }
        }

        public void Dispose()
        {
            if (_timer is null)
            {
                return;
            }

            StopTimerAndUnregisterHandler();

            _timer.Dispose();
            _timer = null!;
        }

        void StopTimerAndUnregisterHandler()
        {
            if (_timer is null)
            {
                return;
            }

            _timer.Stop();
            _timer.Elapsed -= OnTimerElapsed;
        }
    }
}
