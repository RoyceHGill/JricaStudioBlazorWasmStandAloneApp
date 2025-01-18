using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos
{
    public class AppointmentProductQuantityUpdateDto
    {
        public Guid AppointmentProductId { get; set; }
        public int Quantity { get; set; }
    }
}
