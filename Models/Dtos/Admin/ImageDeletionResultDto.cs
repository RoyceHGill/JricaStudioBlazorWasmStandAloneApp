using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos.Admin
{
    public class ImageDeletionResultDto
    {
        public int RecordsAffected { get; set; }
        public UploadResultDto DeletedUpload { get; set; }
        public string ErrorMessage { get; set; }
    }
}
