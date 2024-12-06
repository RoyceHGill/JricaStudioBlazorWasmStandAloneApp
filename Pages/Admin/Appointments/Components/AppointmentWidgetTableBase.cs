using JaysLashesWebApp.Models.Dtos.Admin;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Admin.Appointments.Components
{
    public class AppointmentWidgetTableBase : ComponentBase
    {
        [Parameter]
        public List<AdminAppointmentWidgetDto> Appointments { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected async Task GoToAppointment_Click(Guid appointmentId)
        {
            NavigationManager.NavigateTo($"admin/appointments/edit/{appointmentId}");
        }
    }
}
