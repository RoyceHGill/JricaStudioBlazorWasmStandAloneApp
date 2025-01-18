using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos.Admin
{
    public class AdminServiceToAddDto<T>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal Price { get; set; }
        public Guid ServiceCategoryId { get; set; }
        public T ImageFile { get; set; }
        
    }
}
