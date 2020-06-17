using System;
using System.Timers;
using Microsoft.AspNetCore.Components;

namespace DotNetDocs.Web.Shared
{
    public class CountDownTimerComponent : ComponentBase, IDisposable
    {
        Timer _timer;

        [Parameter]
        public DateTime ShowTime { get; set; }

        protected TimeSpan TimeRemaining { get; private set; }

        public CountDownTimerComponent()
        {
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
            TimeRemaining = ShowTime.Subtract(DateTime.Now);

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
