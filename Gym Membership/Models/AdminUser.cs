using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class AdminUser
    {


        public AdminUser()
        {

            ValidationErrors = new List<String>();
        }

        public bool HasValidationErrors
        {
            get
            {
                return ValidationErrors.Count > 0;
            }
        }

        public IList<String> ValidationErrors { get; set; }
        public int UserId { get; set; }

         [Required]
        public string Username { get; set; }


        [Required]
        public string Password { get; set; }

        public  string EncryptedPassword
        {
            get
            {
                if(String.IsNullOrWhiteSpace(Password))
                {
                    return string.Empty;

                }
                else
                {
                    return Helpers.Utils.base64Encode(Password);
                }
            }
        }

        [DisplayName("Full Name")]
        [Required]
        public string Fullname { get; set; }

        public DateTime LastLogin { get; set; }


        public AccessLevelEnum AccessLevel { get; set; }

        [DisplayName("User Access")]
        public int AccessLevelCode { get; set; }

        public bool IsValid { get; set; }

        public bool IsActive { get; set; }

        public int NumLogins { get; set; }

        public enum AccessLevelEnum
        { 
          USER =1,
            SUPERUSER,
            ADMIN

        }

        public string EmailAddress { get; set; }

        public bool IsTempPassword { get; set; }
    }
}