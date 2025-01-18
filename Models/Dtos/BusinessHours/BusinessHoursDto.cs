using JricaStudioApp.Models.Dtos.BusinessHours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos.BusinessHours
{
    public class BusinessHoursDto : IBusinessHoursDto
    {
        public DayOfWeek Day { get; set; }
        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }
        public TimeSpan LocalTimeOffset { get; set; }
        public bool IsDisabled { get; set; }
        public int AfterHoursGraceRange { get; set; } = 2;
    }
}
