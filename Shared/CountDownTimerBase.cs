using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Timers;

namespace JricaStudioApp.Shared
{
    public class CountDownTimerBase : ComponentBase
    {
        public TimeSpan timeRemaining;
        private System.Timers.Timer timer;
        private bool isRunning;
        [Parameter]
        public int Seconds { get; set; } = 5;
        [Inject]
        public IJSRuntime JS { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await StartCountdown();
        }

        protected async override Task OnAfterRenderAsync( bool firstRender )
        {
            await JS.InvokeVoidAsync( "SetTimer", $"{Seconds}" );
        }
        private async Task StartCountdown()
        {
            if ( isRunning )
                return;

            timeRemaining = new TimeSpan( 0, 0, Seconds ); // set countdown timer
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
