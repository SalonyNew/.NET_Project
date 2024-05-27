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
        public Guid JobPostId { get; set; }

        [Required(ErrorMessage = "Job title is required.")]
        public string JobTitle { get; set; } = null!;

        [Required(ErrorMessage = "Job description is required.")]
        public string? JobDescription { get; set; }

        [Required(ErrorMessage = "Qualification required is required.")]
        public string? QualificationRequired { get; set; }

        public DateTime Deadline { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; } = null!;

        [Required(ErrorMessage = "Industry is required.")]
        public string Industry { get; set; } = null!;

        [Required(ErrorMessage = "Type is required.")]
        public string Type { get; set; } = null!;
    }
}
