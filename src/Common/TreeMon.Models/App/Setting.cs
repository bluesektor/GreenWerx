// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.App
{
    [Table("Settings")]
    public class Setting : Node
    {
       
        public Setting()
        {
            UUIDType = "Setting";
        }
        public string Value { get; set; }

        /// <summary>
        /// Type for the value (int, string, span etc..).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// web,forms,mobile
        /// </summary>
        public string AppType { get; set; }

        public string SettingClass { get; set; }
    }
}
