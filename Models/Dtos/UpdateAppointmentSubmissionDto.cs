using JricaStudioApp.Models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos
{
    public class UpdateAppointmentSubmissionDto
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public IEnumerable<AppointmentServiceDto>? Services { get; set; }
        public IEnumerable<AppointmentProductDto>? Products { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateOnly DOB { get; set; }
        public bool IsWavierAcknowledged { get; set; }
        public bool WearsContanctLenses { get; set; }
        public bool HasHadEyeProblems4Weeks { get; set; }
        public bool HasAllergies { get; set; }
        public bool HasSensitiveSkin { get; set; }
        public bool HasHadEyelashExtentions { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}