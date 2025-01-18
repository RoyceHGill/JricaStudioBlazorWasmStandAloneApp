using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos
{
    public class UserWaiverPatchDto
    {
        public Guid Id { get; set; }
        public bool IsAccepted { get; set; }
    }
}
