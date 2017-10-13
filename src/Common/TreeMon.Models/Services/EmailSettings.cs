// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
namespace TreeMon.Models.Services
{
    public  class EmailSettings
    {
        public int RoleWeight { get; set; }

        public string MailHost { get; set; }

        public int MailPort { get; set; }

        public string HostUser { get; set; }

        public string HostPassword { get; set; }

        public bool UseSSL { get; set; }

        public string SiteEmail { get; set; }

        public string SiteDomain { get; set; }

        /// <summary>
        /// Should be the AppKey
        /// </summary>
        public string EncryptionKey { get; set; }

    }
}
