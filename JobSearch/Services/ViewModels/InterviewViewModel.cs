using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class InterviewViewModel
    {

        public Guid InterviewId { get; set; }
        public Guid ApplicationId { get; set; }
        public string? ApplicantName { get; set; }
        public DateTime InterviewDate { get; set; }
        public TimeOnly Time { get; set; }
        public string? Location { get; set; }
       

        
       


    }
}
