using JricaStudioApp.Services.Contracts;
using JaysLashesWebApp.Models.Dtos;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Products.Iterative
{
    public class RandomProductsBase : ComponentBase
    {
        [Inject]
        public IProductService ProductService { get; set; }
        [Parameter]
        public int Length { get; set; }
        public IEnumerable<ProductDto> Products { get; set; }

        [Inject]
        public IManageLocalStorageService ManageLocalStorageService { get; set; }

        public Guid AppointmentId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Products = await ProductService.GetRandomProducts(Length);
            AppointmentId = await ManageLocalStorageService.GetLocalAppointmentIdGuid();
        }
    }
}
