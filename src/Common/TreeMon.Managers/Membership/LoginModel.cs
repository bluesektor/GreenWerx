// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.ComponentModel.DataAnnotations;

namespace TreeMon.Managers.Membership
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName{get; set;}

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password{get; set;}

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public string ClientType { get; set; }
    }
}
