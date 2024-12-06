using JricaStudioApp.Services.Contracts;
using JaysLashesWebApp.Models.Dtos;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Services
{
    public class RandomServicesBase : ComponentBase
    {
        [Inject]
        public IServiceService ServiceService { get; set; }
        [Parameter]
        public int Length { get; set; }

        public IEnumerable<ServiceDto> Services { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Services = await ServiceService.GetRandomServices(Length);
        }
    }
}
