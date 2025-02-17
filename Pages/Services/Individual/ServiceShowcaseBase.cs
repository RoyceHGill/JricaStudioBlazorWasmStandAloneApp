using Blazorise;
using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Services.Individual
{
    public class ServiceShowcaseBase : ComponentBase
    {
        [Inject]
        public IServiceService ServiceService { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorageService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public Guid ServiceId { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid UserId { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsReturningCustomer { get; set; }





        protected override async Task OnInitializedAsync()
        {
            try
            {
                AppointmentId = await ManageLocalStorageService.GetLocalAppointmentIdGuid();
                UserId = await ManageLocalStorageService.GetLocalUserIdGuid();

                var service = await ServiceService.GetPreviouslyOrderedService( UserId );

                if ( service != null )
                {
                    ServiceId = service.Id;
                    IsReturningCustomer = true;
                }
                else
                {
                    var showcaseService = await ServiceService.GetServiceShowcase();
                    if ( showcaseService != null )
                    {
                        ServiceId = showcaseService.Id;
                        IsReturningCustomer = false;
                    }
                }
            }
            catch ( Exception e )
            {
                ErrorMessage = e.Message;
            }

        }

    }
}
