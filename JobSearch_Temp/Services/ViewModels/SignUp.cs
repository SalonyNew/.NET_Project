using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class Signup
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        [EmailAddress]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        public string Email { get; set; } = null!;
        
        [Required]
        public int Age { get; set; }
        [Required]
        public string Gender { get; set; } = null!;
        [Required]
        public string Role { get; set; }

        [Required]
        [DataType(DataType.Password)]

        public string Password { get; set; } = null!;
       
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [Compare("Password",ErrorMessage ="Password and ConfirmPassword does not match.")]
        public string ConfirmPassword { get; set; } = null!;
       

    }
}
