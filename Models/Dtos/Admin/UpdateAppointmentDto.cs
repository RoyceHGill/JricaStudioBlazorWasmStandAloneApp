using JricaStudioApp.Models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos.Admin
{
    public class UpdateAppointmentDto
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool HasHadEyelashExtentions { get; set; }
        public bool IsSampleSetComplete { get; set; }
        public bool IsDepositPaid { get; set; }
        public DateTime? SampleSetCompleted { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}
