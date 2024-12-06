using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos.Admin
{
    public class AdminUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateOnly DOB { get; set; }
        public bool WearsContanctLenses { get; set; }
        public bool HasHadEyeProblems4Weeks { get; set; }
        public bool HasAllergies { get; set; }
        public bool HasSensitiveSkin { get; set; }
        public bool IsWaiverAcknowledged { get; set; }

    }
}
