﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class ApplicationViewModel
    {
        public Guid ApplicationId { get; set; }

        public string? Name { get; set; }
        public string? PhoneNo { get; set; }
        public string? Email { get; set; }
        public string? Education { get; set; }
        public string? Resume { get; set; }
        public DateTime ApplicationDate { get; set; }

       
    }
}