using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos.Admin
{
    public class AppointmentServiceAdminAddDto
    {
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal Price { get; set; }


    }
}
