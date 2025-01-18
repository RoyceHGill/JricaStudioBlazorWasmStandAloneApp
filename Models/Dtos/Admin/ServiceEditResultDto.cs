using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos.Admin
{
    public class ServiceEditResultDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public UploadResultDto UploadResult { get; set; }
    }
}
