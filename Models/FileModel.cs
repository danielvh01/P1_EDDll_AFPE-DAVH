using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace P1_EDDll_AFPE_DAVH.Models
{
    public class FileModel
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
