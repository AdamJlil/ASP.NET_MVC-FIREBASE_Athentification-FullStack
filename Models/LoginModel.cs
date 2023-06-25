using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Full_projeject.Models
{
    public class LoginModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display (Name = "Email")]
        public string Email { get; set; }

        [Required, Display (Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}