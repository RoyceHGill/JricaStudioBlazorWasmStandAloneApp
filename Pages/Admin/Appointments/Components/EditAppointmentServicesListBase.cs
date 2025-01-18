using JricaStudioApp.Services.Contracts;
using JricaStudioApp.Models.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace JricaStudioApp.Pages.Admin.Appointments.Components
{
    public class EditAppointmentServicesListBase : ComponentBase
    {
        [Parameter]
        public IEnumerable<AppointmentServiceDto> Services { get; set; }
        [Inject]
        public IAppointmentItemService AppointmentItemService { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorageService { get; set; }

        public decimal TotalServicesPrice { get; set; }
        public TimeSpan Duration { get; set; }
        public int ServicesCount { get; set; }




        protected void CalculateTotals()
        {
            CalculateTotalDuration();
            GetTotalServicesCount();
            CalculateTotalServicePrice();
        }

        protected void CalculateTotalServicePrice()
        {
            TotalServicesPrice = Services.Sum(p => p.Price);
        }

        protected void CalculateTotalDuration()
        {
            var totalDuration = TimeSpan.FromMinutes(Services.Sum(s => s.Duration.TotalMinutes));
            if (totalDuration == TimeSpan.Zero)
            {
                totalDuration = TimeSpan.FromMinutes(15);
            }
            Duration = totalDuration;
        }

        protected void GetTotalServicesCount()
        {
            ServicesCount = Services.Count();
        }

        protected async void MakeVisible(string id)
        {
            await JsRuntime.InvokeVoidAsync("SetVisible", id);
        }

        protected async void MakeInvisible(string id)
        {
            await JsRuntime.InvokeVoidAsync("SetHidden", id);
        }


        protected async Task DeleteAppointmentService_Click(Guid id)
        {
            var appointmentService = await AppointmentItemService.DeleteAppointmentService(id);

            Services = await AppointmentItemService.GetAppointmentServices(await ManageLocalStorageService.GetLocalAppointmentIdGuid());

            CalculateTotals();
        }
    }

}
