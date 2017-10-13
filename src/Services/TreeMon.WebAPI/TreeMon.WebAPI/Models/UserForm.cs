// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Collections.Generic;
using TreeMon.Models.Membership;

namespace TreeMon.Web.Models
{
    public class UserForm:User
    {
        public UserForm()
        {
            Accounts = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Accounts { get; set; }
    }
}