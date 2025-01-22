using Blazorise;
using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Services.Individual
{
    public class ServicePageBase : ComponentBase
    {
        [Parameter]
        public Guid Id { get; set; }

        [Inject]
        public IServiceService ServiceService { get; set; }

        public ServiceDto Service { get; set; }
        public string ErrorMessage { get; set; }


        protected override async Task OnInitializedAsync()
        {
            try
            {
               
                Service = await ServiceService.GetService(Id);
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
