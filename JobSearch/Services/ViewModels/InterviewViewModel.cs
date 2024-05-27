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

        public DateTime InterviewDate { get; set; }
        public TimeOnly Time { get; set; }


        public string? Location {get; set; }

       
    }
}
