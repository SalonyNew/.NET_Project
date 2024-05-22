using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class JobInfo
    {
        
        [Required]
        public string JobTitle { get; set; } = null!;
        [Required]
        public string? JobDescription { get; set; }
        [Required]
        public string? QualificationRequired { get; set; }

        public DateTime Deadline { get; set; }
        [Required]
        public string Location { get; set; } = null!;

        public string Industry { get; set; } = null!;
        [Required]
        public string Type { get; set; } = null!;

    }
}
