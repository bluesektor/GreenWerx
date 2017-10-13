// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.ComponentModel.DataAnnotations;

namespace TreeMon.Models.Membership
{
    public class UserRegister
    {
        [Required]
        [Display(Name = "User name")]
        public string Name { get; set; }
            
        public string Email { get; set; }
        public bool UserIsPrivate { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }

        public string AccountUUID { get; set; }

        //website, mobile.app
        public string ClientType { get; set; }

    }
}
