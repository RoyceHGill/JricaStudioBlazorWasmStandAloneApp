using JricaStudioApp.Services.Contracts;
using JricaStudioApp.Models.Dtos;
using JricaStudioApp.Models.Dtos.Admin;
using Microsoft.AspNetCore.Components;
using Models.Dtos.Admin;

namespace JricaStudioApp.Pages.Admin.Services.Components
{
    public class ServiceShowcaseAdministrationBase : ComponentBase
    {
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public IServiceService ServiceService { get; set; }
        public ServiceDto ShowcaseService { get; set; }
        public IEnumerable<ServiceAdminPageDto> Services { get; set; }
        public UpdateServiceShowcaseDto UpdateDto { get; set; }
        public AdminUserLoginDto Admin { get; set; }

        public string ErrorMessage { get; set; }
        public string ConfirmationMessage { get; set; }


        protected override async Task OnInitializedAsync()
        {
            Admin = await AdminService.AuthenticateUser();

            Services = await  ServiceService.GetAdminServices();

            ShowcaseService = await ServiceService.GetServiceShowcase();

            if (ShowcaseService != null)
            {
                UpdateDto = new UpdateServiceShowcaseDto
                {
                    ServiceId = ShowcaseService.Id
                };
            }
            else
            {
                UpdateDto = new UpdateServiceShowcaseDto();
            }
        }

        protected async Task UpdateServiceShowcase_Click()
        {
            try
            {
                var service = await ServiceService.UpdateServiceShowcase(UpdateDto);

                if (service == null)
                {
                    ApplyErrorMessage("The service showcase was not updated.");
                    return;
                }
                ShowcaseService = service;
                ApplyConfirmationMessage("The service showcase was updated.");
            }
            catch (Exception e)
            {
                ApplyErrorMessage(e.Message);
            }
        }

        protected void ApplyErrorMessage(string message)
        {
            ConfirmationMessage = string.Empty;
            ErrorMessage = message;
        }

        protected void ApplyConfirmationMessage(string message)
        {
            ConfirmationMessage = message;
            ErrorMessage = string.Empty;
        }

        protected ServiceDto PassService(ServiceDto dto)
        {
            if (dto != null)
            {
                return dto;
            }
            return default;
        }



    }
}
