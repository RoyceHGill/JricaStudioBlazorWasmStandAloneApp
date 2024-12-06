using JricaStudioApp.Services.Contracts;
using JaysLashesWebApp.Models.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace JricaStudioApp.Pages.Products.Individual
{
    public class PopularProductUpsellBase : ComponentBase
    {
        [Parameter]
        public Guid AppointmentId { get; set; }

        [Inject]
        public IProductService ProductService { get; set; }

        [Inject]
        public IAppointmentItemService AppointmentItemService { get; set; }
        [Parameter]
        public ProductDto Product { get; set; }

        public IEnumerable<AppointmentProductDto> AppointmentProductDtos { get; set; }


        protected async override Task OnInitializedAsync()
        {

            if (Product == null)
            {
                Product = await ProductService.GetShowcaseProduct();
                if (Product == null)
                {
                    Product = new();
                }
            }
            if (AppointmentId == Guid.Empty)
            {
                AppointmentProductDtos = new List<AppointmentProductDto>();
            }
            else
            {
                AppointmentProductDtos = await AppointmentItemService.GetAppointmentProducts(AppointmentId);

            }

        }

        protected async Task AddProductShowCaseToAppointment_OnClick()
        {
            try
            {
                if (AppointmentId != Guid.Empty)
                {

                    var appointmentProduct = await AppointmentItemService.PostAppointmentProduct(new AppointmentProductToAddDto()
                    {
                        AppointmentId = AppointmentId,
                        ProductId = Product.Id,
                        Quantity = 1,
                    });
                    AppointmentProductDtos = AppointmentProductDtos.Append(appointmentProduct);
                    AppointmentItemService.RaiseEventOnProductsChanged(AppointmentProductDtos);
                    StateHasChanged();
                }

            }
            catch (Exception)
            {

                throw;
            }
            

        }


    }
}
