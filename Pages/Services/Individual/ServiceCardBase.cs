using JricaStudioApp.Models.Dtos;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Services.Individual
{
    public class ServiceCardBase : ComponentBase
    {
        [Parameter]
        public ServiceDto Service { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected async Task NavigateToService_Click()
        {
            if (Service.Id != default)
            {
                NavigationManager.NavigateTo($"/ServiceDetails/{Service.Id}");

            }
        }
    }
}
