using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos.Admin
{
    public class ServiceAdminPageDto
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageData { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal Price { get; set; }
        public Guid ServiceCategoryId { get; set; }
    }
}
