using Microsoft.AspNetCore.Components;
using System.Timers;

namespace JricaStudioApp.Shared
{
    public class CountDownTimerBase : ComponentBase
    {
        public TimeSpan timeRemaining;
        private System.Timers.Timer timer;
        private bool isRunning;



        protected override async Task OnInitializedAsync()
        {
            await StartCountdown();
        }
        private async Task StartCountdown()
        {
            if ( isRunning )
                return;

            timeRemaining = new TimeSpan( 0, 0, 20 ); // 1 minute countdown
            isRunning = true;

            timer = new System.Timers.Timer( 1000 ); // 1 second intervals
            timer.Elapsed += OnTimerElapsed;
            timer.Start();
        }

        private async void OnTimerElapsed( object sender, ElapsedEventArgs e )
        {
            if ( timeRemaining.TotalSeconds > 0 )
            {
                timeRemaining = timeRemaining.Subtract( TimeSpan.FromSeconds( 1 ) );
                await InvokeAsync( StateHasChanged );
            }
            else
            {
                timer.Stop();
                isRunning = false;
            }
        }

        public void Dispose()
        {
            if ( timer != null )
            {

                timer.Dispose();

            }
        }
    }
}
