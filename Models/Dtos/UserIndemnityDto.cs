using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos
{
    public class UserIndemnityDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateOnly DOB { get; set; }
        public bool WearsContanctLenses { get; set; } = false;
        public bool HasHadEyeProblems4Weeks { get; set; } = false;
        public bool HasAllergies { get; set; } = false;
        public bool HasSensitiveSkin { get; set; } = false;
    }
}
