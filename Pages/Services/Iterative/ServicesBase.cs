using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Services
{
    public class ServicesBase : ComponentBase
    {
        [Inject]
        public IServiceService ServiceService { get; set; }
        public IEnumerable<ServiceDto> Services { get; set; }

        public int ItemsPerRow { get; set; } = 4;
        protected override async Task OnInitializedAsync()
        {
            Services = await ServiceService.GetServices();
        }
    }
}
