// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace TreeMon.Models.General
{
    [Table("Tags")]
    public class Tag
    {
        public Tag()
        {
            this.UUID = Guid.NewGuid().ToString("N");

        }

        public string UUID { get; set; }
        public string AccountUUID { get; set; }

        public string ReferenceUUID { get; set; }
        public string Type { get; set; }
        public string  Value { get; set; }
        public int Weight { get; set; }
    }
}
