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

        protected TimeSpan TimeRemaining { get; private set; }

        public CountDownTimerComponent()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
            }
            else
            {
                _centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            }

            _timer = new Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
        }

        protected override void OnParametersSet()
        {
            _timer.Start();

            base.OnParametersSet();
        }

        void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, _centralTimeZone);
            TimeRemaining = ShowTime.Subtract(dateTime);

            InvokeAsync(() => StateHasChanged());
        }

        public void Dispose()
        {
            if (_timer is null)
            {
                return;
            }

            _timer.Stop();
            _timer.Elapsed -= OnTimerElapsed;
            _timer.Dispose();
            _timer = null!;
        }
    }
}
