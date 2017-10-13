// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Membership
{
    [Table("Users")]
    public class User:Node, INode
    {
        public User()
        {
            UUID = Guid.NewGuid().ToString("N");
            UUIDType = "User";
        }

        public override bool Equals(object obj)
        {
            try
            {
                return this.UUID == ((User)obj).UUID;
            }
            catch {//
            }
            return false;
        }

        public override int GetHashCode()
        {
            return UUID.GetHashCode();
        }

        public string LicenseNumber { get; set; }
       
        public string Email { get; set; }

        [Display(Name = "Last Updated Date:")]
        public DateTime? LastUpdatedDate { get; set; }


        [Display(Name = "Anonymous:")]
        public bool Anonymous { get; set; }

        [Display(Name = "Approved:")]
        public bool Approved { get; set; }

        [Display(Name = "Banned:")]
        public bool Banned { get; set; }

        [Display(Name = "Locked Out:")]
        public bool LockedOut { get; set; }

        [Display(Name = "Is Online:")]
        public bool Online { get; set; }

        /// <summary>
        /// Verify this via AppInfo table
        /// </summary>
        public bool SiteAdmin { get; set; }

        // Returns:The date and time when the membership user was last authenticated or accessed the application.
        [Display(Name = "Last Activity Date:")]
        public DateTime? LastActivityDate { get; set; }

        [Display(Name = "Last Locked Out Date:")]
        public DateTime? LastLockedOutDate { get; set; }

        [Display(Name = "Last Lockout Date:")]
        public DateTime? LastLockoutDate { get; set; }

        [Display(Name = "Last Login Date:")]
        public DateTime? LastLoginDate { get; set; }

        [Display(Name = "Provider Name:")]
        public string ProviderName { get; set; }


        #region User Security

        public DateTime? LastPasswordChangedDate { get; set; }

        public string Password { get; set; }

        public int PasswordHashIterations { get; set; }


        public string MobilPin { get; set; }

        [Display(Name = "Password Answer:")]
        public string PasswordAnswer { get; set; }

        public string PasswordFormat { get; set; }

        [Display(Name = "Password Question:")]
        public string PasswordQuestion { get; set; }

        public string PasswordSalt { get; set; }

        [Display(Name = "Failed Password Answer Attempt Count:")]
        public int FailedPasswordAnswerAttemptCount { get; set; }

        [Display(Name = "Failed Password Answer Attempt Window Start:")]
        public int FailedPasswordAnswerAttemptWindowStart { get; set; }

        [Display(Name = "Failed Password Attempt Count:")]
        public int FailedPasswordAttemptCount { get; set; }

        [Display(Name = "Failed Password Attempt Window Start:")]
        public int FailedPasswordAttemptWindowStart { get; set; }

        public string ProviderUserKey { get; set; }

        public string UserKey { get; set; }
        #endregion
    }
}
