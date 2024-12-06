using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos
{
    public class UpdateAppointmentIndemnityDto
    {
        public Guid UserId { get; set; }
        public bool HasHadEyelashExtentions { get; set; } = false;
        public bool IsSampleSetComplete { get; set; } = false;
        public DateTime? SampleSetCompleted { get; set; }
    }
}
