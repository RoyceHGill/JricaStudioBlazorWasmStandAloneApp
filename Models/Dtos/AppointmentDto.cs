using JricaStudioApp.Models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public IEnumerable<AppointmentServiceDto>? Services { get; set; }
        public IEnumerable<AppointmentProductDto>? Products { get; set; }
        public AppointmentStatus Status { get; set; }
        public Guid UserId { get; set; }
    }
}
