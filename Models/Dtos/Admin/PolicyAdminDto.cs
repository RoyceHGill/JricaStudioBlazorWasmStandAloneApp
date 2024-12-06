using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JricaStudioApp.Models.Dtos.Admin
{
    public class PolicyAdminDto
    {
        public Guid Id { get; set; }
        public string PolicyTitle { get; set; }
        public string PolicyArticle { get; set; }
    }
}
