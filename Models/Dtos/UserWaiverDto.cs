using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos
{
    public class UserWaiverDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsWavierAcknowledged { get; set; }
    }
}
