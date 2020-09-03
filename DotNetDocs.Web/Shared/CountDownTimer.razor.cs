using System;
using System.Collections;
using System.Threading.Tasks;
using System.Timers;
using DotNetDocs.Services;
using Microsoft.AspNetCore.Components;

namespace DotNetDocs.Web.Shared
{
    public partial class CountDownTimer : IDisposable
    {
        static readonly TimeSpan ShowDuration = TimeSpan.FromHours(-1).Duration();

        [Parameter]
        public DateTime ShowTime { get; set; }

        [Parameter]
        public EventCallback<bool> ShowIsStarting { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; } = null!;

        [Inject]
        public DateTimeService DateTimeService { get; set; } = null!;

        protected TimeSpan TimeRemaining { get; private set; }
        protected string ImminentClass { get; private set; } = "";
        protected DateTime DateTime { get; private set; }

        bool _showStarted;
        Timer? _timer;

        public CountDownTimer()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
        }

        protected override void OnParametersSet()
        {
            _timer?.Start();

            base.OnParametersSet();
        }

        async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_timer != null)
            {
                _timer.Enabled = false;
            }

            DateTime = TimeZoneInfo.ConvertTime(DateTime.Now, DateTimeService.CentralTimeZone);
            TimeRemaining = ShowTime.Subtract(DateTime);

            bool reenableTimer = true;
            bool alreadyStarted = TimeRemaining.Ticks < 0;
            if (alreadyStarted)
            {
                // Show is over
                if (TimeRemaining.Duration() >= ShowDuration)
                {
                    reenableTimer = false;
                    StopTimerAndUnregisterHandler();
                    Navigation.NavigateTo("/", true);
                }
                else if (!_showStarted)
                {
                    ImminentClass = "";
                    await InvokeAsync(
                        async () => await ShowIsStarting.InvokeAsync(_showStarted = true));
                }
            }
            else
            {
                if (TimeRemaining <= TimeSpan.FromSeconds(30))
                {
                    // Starts in 30 seconds, show embedded Twitch stream
                    ImminentClass = "";
                    await InvokeAsync(
                        async () => await ShowIsStarting.InvokeAsync(_showStarted = true));
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

            if (_timer != null)
            {
                _timer.Enabled = reenableTimer;
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
