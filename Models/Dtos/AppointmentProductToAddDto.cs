using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos
{
    public class AppointmentProductToAddDto
    {
        public Guid AppointmentId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

    }
}
