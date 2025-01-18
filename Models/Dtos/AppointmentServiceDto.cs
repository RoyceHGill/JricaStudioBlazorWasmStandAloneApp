using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos
{
    public class AppointmentServiceDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public string ServiceImagePath { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal Price { get; set; }
    }
}
