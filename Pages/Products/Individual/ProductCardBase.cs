using JricaStudioApp.Services.Contracts;
using JricaStudioApp.Models.Dtos;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Products.Individual
{
    public class ProductCardBase : ComponentBase, IDisposable
    {
        [Parameter]
        public Guid AppointmentId { get; set; }
        [Parameter]
        public ProductDto Product { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IAppointmentItemService AppointmentItemService { get; set; }
        [Inject]
        public IAppointmentService AppointmentService { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorageService { get; set; }
        public AppointmentDto Appointment { get; set; }
        public AppointmentProductToAddDto ProductToAddDto { get; set; }

        protected async override Task OnInitializedAsync()
        {
            AppointmentService.OnIdChanged += AppointmentIdChanged;
            try
            {
                Appointment = await AppointmentService.GetAppointment(AppointmentId);
                if (Appointment == null)
                {
                    AppointmentId = await ManageLocalStorageService.GetLocalAppointmentIdGuid();
                    Appointment = await AppointmentService.GetAppointment(AppointmentId);
                }
            }
            catch (Exception)
            {

            }
            
        }

        private async void AppointmentIdChanged(Guid id)
        {
            AppointmentId = id;
            Appointment = await AppointmentService.GetAppointment(AppointmentId);
            StateHasChanged();
        }

        protected async Task AddToAppointment_Click(AppointmentProductToAddDto addDto)
        {

            try
            {
                var appointmentProduct = await AppointmentItemService.PostAppointmentProduct(addDto);
                var dtos = await AppointmentItemService.GetAppointmentProducts(AppointmentId);
                AppointmentItemService.RaiseEventOnProductsChanged(dtos);
            }
            catch (Exception)
            {
                NavigationManager.NavigateTo($"/Appointment/Edit/{AppointmentId}");
            }
        }

        protected async Task NavigateToProduct()
        {
            if (Product.Id != null)
            {
                NavigationManager.NavigateTo($"ProductDetails/{Product.Id}");
            }
        }

        public void Dispose()
        {
            AppointmentService.OnIdChanged -= AppointmentIdChanged;
        }
    }
}
