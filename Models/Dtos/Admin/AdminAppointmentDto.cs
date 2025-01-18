using JricaStudioApp.Models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos.Admin
{
    public class AdminAppointmentDto
    {
        public Guid Id { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public AdminUserDto User { get; set; }
        public bool HasHadEyelashExtentions { get; set; }
        public bool IsSampleSetComplete { get; set; }
        public bool IsDepositPaid { get; set; }
        public DateTime? SampleSetCompleted { get; set; }
        public IEnumerable<AppointmentServiceDto>? Services { get; set; }
        public IEnumerable<AppointmentProductDto>? Products { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}
