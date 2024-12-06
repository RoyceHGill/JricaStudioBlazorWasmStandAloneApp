using JricaStudioApp.Services.Contracts;
using JaysLashesWebApp.Models.Dtos;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Appointments
{
    public class AppointmentSubmissionBase : ComponentBase
    {
        [Parameter]
        public Guid Id { get; set; }
        [Inject]
        public IAppointmentService AppointmentService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public AppointmentDto Appointment { get; set; }


        protected async override Task OnInitializedAsync()
        {
            Appointment = await AppointmentService.GetAppointment(Id);
        }

        protected async Task Back_OnClick()
        {
            NavigationManager.NavigateTo($"/Finalize/{Id}");
        }

        protected async Task Home_OnClick()
        {
            NavigationManager.NavigateTo($"/");
        }

        protected async Task ContactUs_OnClick()
        {
            NavigationManager.NavigateTo("/aboutus");
        }
    }
}
