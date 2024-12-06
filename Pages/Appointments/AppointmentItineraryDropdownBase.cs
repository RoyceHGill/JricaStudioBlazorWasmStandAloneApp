using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using Microsoft.AspNetCore.Components;
using JaysLashesWebApp.Models.Dtos;
using Microsoft.JSInterop;
using JaysLashesWebApp.Models.enums;

namespace JricaStudioApp.Pages.Appointments
{
    public class AppointmentItineraryDropdownBase : ComponentBase, IDisposable
    {
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IAppointmentItemService AppointmentItemService { get; set; }
        [Inject]
        public IAppointmentService AppointmentService { get; set; }

        [Inject]
        public IUserService UserService { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorageService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Parameter]
        public Guid AppointmentId { get; set; }

        [Parameter]
        public UserDto User { get; set; }

        public AppointmentDto Appointment { get; set; }
        public IEnumerable<AppointmentProductDto>? AppointmentProductDtos { get; set; }
        public IEnumerable<AppointmentServiceDto>? AppointmentServiceDtos { get; set; }

        protected override async Task OnInitializedAsync()
        {
            
            AppointmentProductDtos = await AppointmentItemService.GetAppointmentProducts(AppointmentId);

            AppointmentServiceDtos = await AppointmentItemService.GetAppointmentServices(AppointmentId);

            AppointmentService.OnIdChanged += AppointmentIdChanged;

            Appointment = await AppointmentService.GetAppointment(AppointmentId);

            AppointmentItemService.OnServiceAdded += AppointmentServicesChanged;

            AppointmentItemService.OnProductAdded += AppointmentProdcutsChanged;

            AppointmentService.OnStatusChange += AppointmentStatusChanged;

            AppointmentService.OnStartTimeChange += AppointmentStartTimeChanged;

            base.OnInitialized();
        }

        protected async void AppointmentIdChanged(Guid id)
        {
            AppointmentId = id;
            Appointment = await AppointmentService.GetAppointment(AppointmentId);
            StateHasChanged();
        }


        protected async void AddNotification()
        {
            await JsRuntime.InvokeVoidAsync("ChangeBtnSuccessToBtnWarning", "dropdownToggleButton");
        }

        protected async void RemoveNotification()
        {
            await JsRuntime.InvokeVoidAsync("ChangeBtnWarningToBtnSuccess", "dropdownToggleButton");
        }

        protected async Task AppointmentButton_Click()
        {
            RemoveNotification();
            StateHasChanged();
        }

        private void AppointmentUserChanged(AppointmentDto appointment)
        {
            Appointment = appointment;
            StateHasChanged();
        }

        private void AppointmentServicesChanged(IEnumerable<AppointmentServiceDto> serviceDtos)
        {
            AppointmentServiceDtos = serviceDtos;
            AddNotification();
            StateHasChanged();
        }

        private void AppointmentProdcutsChanged(IEnumerable<AppointmentProductDto> productDtos)
        {
            AppointmentProductDtos = productDtos;
            AddNotification();
            StateHasChanged();
        }

        private void AppointmentStatusChanged(AppointmentStatus status)
        {
            Appointment.Status = status;
            StateHasChanged();
        }
        private void AppointmentStartTimeChanged(DateTime time)
        {
            Appointment.StartTime = time;
            StateHasChanged();
        }

        protected async Task NewAppointment_Click()
        {
            await ManageLocalStorageService.RemoveLocalAppointmentIdGuid();
            NavigationManager.NavigateTo(NavigationManager.BaseUri, true);
        }

        public void Dispose()
        {
            AppointmentItemService.OnServiceAdded -= AppointmentServicesChanged;

            AppointmentItemService.OnProductAdded -= AppointmentProdcutsChanged;

            AppointmentService.OnStatusChange -= AppointmentStatusChanged;

            AppointmentService.OnStartTimeChange -= AppointmentStartTimeChanged;

            AppointmentService.OnIdChanged -= AppointmentIdChanged;
        }
    }
}
