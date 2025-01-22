using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Products.Iterative
{
    public class ProductsCardsBase : ComponentBase
    {
        [Parameter]
        public IEnumerable<ProductDto> Products { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorageService { get; set; }
        public Guid AppointmentId { get; set; }


        protected override async Task OnInitializedAsync()
        {
            AppointmentId = await ManageLocalStorageService.GetLocalAppointmentIdGuid();

            base.OnInitialized();
        }
    }
}
