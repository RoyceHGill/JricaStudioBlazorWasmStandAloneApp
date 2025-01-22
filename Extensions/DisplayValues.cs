using JricaStudioSharedLibrary.Dtos.Admin;

namespace JricaStudioApp.Extensions
{
    public static class DisplayValues
    {
        public static string DisplayBoolAsYesNo(this bool myBool)
        {
            if (myBool)
            {
                return "Yes";
            }
            else
            {
                return "No";
            }
        }

        public static string DisplayTotalDuration(AdminAppointmentDto? appointment)
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
        public static string DisplayTimeString(this DateTime time)
        {
            return time.ToString("hh:mm tt  dd/MM/yyyy");
        }
    }
}
