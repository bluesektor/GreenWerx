// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.ComponentModel.DataAnnotations;

namespace TreeMon.Web.Models
{
    public class TosApiKey
    {
        public bool WarningUnderstand { get; set; }

        public bool TOSAgree { get; set; }

        [StringLength(32)]
        public string UserUUID { get; set; }

        public int Id { get; set; }
    }
}