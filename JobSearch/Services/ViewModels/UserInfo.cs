using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class UserInfo
    {
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Education { get; set; }

        public string? WorkExperience { get; set; }

        public string? Skills { get; set; }

        public string? Resume { get; set; } 

        

        


    }
}
