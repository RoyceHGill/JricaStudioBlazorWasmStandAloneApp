using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos
{
    public class ServiceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageData { get; set; }
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }
        public Guid ServiceCategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
