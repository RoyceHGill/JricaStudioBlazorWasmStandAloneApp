using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos.Admin
{
    public class BlockOutDatesAdminDto
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }            
    }
}
