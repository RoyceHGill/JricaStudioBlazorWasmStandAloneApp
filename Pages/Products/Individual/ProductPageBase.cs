using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using Microsoft.AspNetCore.Components;


namespace JricaStudioApp.Pages.Products.Individual
{
    public class ProductPageBase : ComponentBase
    {
        [Parameter]
        public Guid Id { get; set; }

        [Inject]
        public IProductService ProductService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IManageLocalStorageService ManageLocalStorageService { get; set; }

        public Guid AppointmentId { get; set; }


        public string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                AppointmentId = await ManageLocalStorageService.GetLocalAppointmentIdGuid();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }



        
    }
}
