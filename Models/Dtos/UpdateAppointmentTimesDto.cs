using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos
{
    public class UpdateAppointmentTimesDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
