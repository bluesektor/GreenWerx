// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
namespace TreeMon.Web.Models
{
    public class ChangePassword
    {
        public string Email { get; set; }

        /// <summary>
        /// True if the user initiated a password reset.
        /// Old password will be blank so don't check it.
        /// </summary>
        public bool ResetPassword { get; set; }

        /// <summary>
        /// When reseting via email, a confirmation code is set in the user
        /// table ProviderUserKey field. The ProviderName must also match int the user flags.
        /// </summary>
        public string ConfirmationCode { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm")]
        //[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword
        {
            get; set;
        }

        //StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "New password")]
        public string NewPassword
        {
            get; set;
        }

        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "Current password")]
        public string OldPassword
        {
            get; set;
        }

        
    }
}