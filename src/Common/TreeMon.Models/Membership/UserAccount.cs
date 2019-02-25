// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TreeMon.Models.Helpers;

namespace TreeMon.Models.Membership
{
    /// <summary>
    /// This is the business level user object.
    /// When the user registers, they will become a member of
    /// this account 
    /// </summary>
    [Table("Accounts")]
    public class Account:INode
    {
        public Account()
        {
            this.UUID = Guid.NewGuid().ToString("N");
            this.UUIDType = "Account";
            this.Id = 0;
            this.ParentId = 0;
            this.UUParentID = string.Empty;
            this.UUParentIDType = string.Empty;
            this.Name = string.Empty;
            this.Status = string.Empty;
            this.AccountUUID = string.Empty;
            this.Active = true;
            this.Deleted = false;
            this.Private = true;
            this.SortOrder = 0;
            this.CreatedBy = string.Empty;
            this.DateCreated = DateTime.MinValue;
            this.RoleWeight = 1;
            this.RoleOperation = "=";
        }

        //TODO temporary for mobile unitl I can implement with categories
        public string Category { get; set; }

        public string Description { get; set; }

        public string GUUID { get; set; }

        public string GuuidType { get; set; }

        public int RoleWeight { get; set; }

        public string RoleOperation { get; set; }

        // [SQLite.PrimaryKey,SQLite.AutoIncrement]]
        [Key]
        public int Id { get; set; }

        public string AccountUUID { get; set; }

        public int? ParentId { get; set; }

        public string Image { get; set; }

        [StringLength(32)]
        public string UUID { get; set; }
        /// <summary>
        /// Defines the type of SettingUUID used (guid, hash.<algo> )..
        /// </summary>
        [StringLength(32)]
        public string UUIDType { get; set; }

        [StringLength(32)]
        public string UUParentID { get; set; }
        [StringLength(32)]
        public string UUParentIDType { get; set; }
     
        public string Name { get; set; }

        public string Status { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool Active { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool Deleted { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool Private { get; set; }

        public int SortOrder { get; set; }

        public string LocationUUID { get; set; }

        /// <summary>
        /// EventLocation, Location
        /// </summary>
        public string LocationType { get; set; }

        public string BillingAddress { get; set; }

        public string BillingCity { get; set; }

        public string BillingState { get; set; }

        public string BillingPostalCode { get; set; }

        public string BillingCountry { get; set; }

        public string Phone { get; set; }

        public string AccountSource { get; set; }
        
        public string Email { get; set; }


        [StringLength(32)]
        public string OwnerUUID { get; set; }
      
        public string WebSite { get; set; }

        // The Data Universal Numbering System(D-U-N-S) number is a unique, 
        //nine-digit number assigned to every business location in the Dun & Bradstreet 
        //database that has a unique, separate, and distinct operation.D-U-N-S numbers are used by 
        //industries and organizations around the world as a global standard for business 
        //identification and tracking. Maximum size is 9 characters.
        public string DunsNumber { get; set; }

        //The six-digit North American Industry Classification System(NAICS) code is the standard 
        //used by business and government to classify business establishments into industries, 
        //according to their economic activity for the purpose of collecting, analyzing, and publishing 
         //statistical data related to the U.S.business economy.Maximum size is 8 characters.
        public string NaicsCode { get; set; }

        //Standard Industrial Classification code of the company’s main business categorization, 
        //for example, 57340 for Electronics.Maximum of 20 characters.
        public string Sic { get; set; }
        
        /// <summary>
        /// Types could be another business/account, a person..
        /// </summary>
        public string OwnerType { get; set; }


        public string CreatedBy { get; set; }

        public DateTime DateCreated { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool? Breeder { get; set; }

    }
}
