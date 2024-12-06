using JaysLashesWebApp.Models.Dtos.Admin;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Admin.Appointments.Components
{
    public class AdminAppointmentDetailsBase : ComponentBase
    {

        [Parameter]
        public AdminAppointmentDto? Appointment { get; set; }
        [Inject]
        public NavigationManager NavigationManger { get; set; }
        public string AppointmentDetailLabelclasses { get; set; } = "col-5 text-end";
        public string AppointmentBoolDetailLabelclasses { get; set; } = "col text-end";
        public string AppointmentDetailRowclasses { get; set; } = "row m-2";


        protected string DisplayTotalDuration(AdminAppointmentDto? appointment)
        {
            double totalDuration = 0;
            if (appointment == null)
            {
                return $"{totalDuration} Minutes";
            }
            else
            {
                if (appointment.Services.Count() == 0)
                {
                    return $"{totalDuration} Minutes";
                }
                foreach (var item in appointment.Services)
                {
                    totalDuration = totalDuration + item.Duration.TotalMinutes;
                }
                return $"{totalDuration} Minutes";
            }
        }

    }
}
