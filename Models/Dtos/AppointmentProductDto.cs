using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos
{
    public class AppointmentProductDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImagePath { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }

    }
}
