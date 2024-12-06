using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos.Admin
{
    public class AdminProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime Created { get; set; }
        public decimal Price { get; set; }
    }
}
