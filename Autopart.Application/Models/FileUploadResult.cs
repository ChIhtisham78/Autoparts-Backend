using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class FileUploadResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string FileName { get; set; }
    }
}
