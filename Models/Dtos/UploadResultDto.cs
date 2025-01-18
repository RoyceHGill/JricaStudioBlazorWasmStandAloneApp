using JricaStudioApp.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos
{
    public class UploadResultDto
    {
        public Guid Id { get; set; }
        public string Filename { get; set; }
        public string StoredFileName { get; set; }
        public string ContentType { get; set; }

    }
}
