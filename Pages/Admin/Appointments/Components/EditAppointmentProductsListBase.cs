using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace JricaStudioApp.Pages.Admin.Appointments.Components
{
    public class EditAppointmentProductsListBase : ComponentBase
    {
        [Parameter]
        public IEnumerable<AppointmentProductDto> Products { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public IAppointmentItemService AppointmentItemService { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorageService { get; set; }

        public int ProductCount { get; set; }
        public decimal TotalPrice { get; set; }

        protected async Task DeleteAppointmentProduct_Click(Guid id)
        {
            var newProductList = await AppointmentItemService.DeleteAppointmentProduct(id);
        }

        protected async Task UpdateAppoitmentProductQuantity_Click(Guid id, int qty)
        {
            try
            {
                if (qty > 0)
                {
                    var updateDto = new AppointmentProductQuantityUpdateDto()
                    {
                        AppointmentProductId = id,
                        Quantity = qty
                    };

                    var response = await AppointmentItemService.PatchUpdateAppoitmentProductQuantity(id, updateDto);
                }
                else
                {
                    await DeleteAppointmentProduct_Click(id);
                    return;
                }
                var newProducts = await AppointmentItemService.GetAppointmentProducts(await ManageLocalStorageService.GetLocalAppointmentIdGuid());


                UpdateTotals();

                MakeInvisible(id.ToString());
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void UpdateTotals()
        {
            ProductCount = CalculateQuantities(Products);
            TotalPrice = CalculateTotalPrice(Products);
        }

        protected decimal CalculateTotalPrice(IEnumerable<AppointmentProductDto> products)
        {
            return products.Sum(p => p.Price * p.Quantity);
        }


        protected int CalculateQuantities(IEnumerable<AppointmentProductDto> products)
        {
            return products.Sum(p => p.Quantity);
        }

        protected async void MakeVisible(string id)
        {
            await JS.InvokeVoidAsync("SetVisible", id);
        }

        protected async void MakeInvisible(string id)
        {
            await JS.InvokeVoidAsync("SetHidden", id);
        }
    }
}
