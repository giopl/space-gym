using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class ChangePasswordViewModel
    {
    

        [Required]
        [EmailAddress]
        [Display(Name = "Username")]
        public string Username { get; set; }


        public string OldPassword { get; set; }


        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool IsTempPassword { get; set; }
   
        public List<string> ValidationErrors { get; set; }

        public ChangePasswordViewModel()
        {
            ValidationErrors = new List<string>();

        }
        public string EncryptedPassword 
        {
            get
            {
                if(String.IsNullOrWhiteSpace(Password))
                {
                    return string.Empty;
                } else
                {

                    return Helpers.Utils.base64Encode(Password);
                }
            }
        }

    }
}