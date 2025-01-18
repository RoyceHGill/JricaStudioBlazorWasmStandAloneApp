using JricaStudioApp.Models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos
{
    public class AppointmentIndemnityDto
    {
        public bool HasHadEyelashExtentions { get; set; } = false;
        public bool IsSampleSetComplete { get; set; } = false;
        public DateTime? SampleSetCompleted { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}
