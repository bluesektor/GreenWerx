// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Membership
{
    [Table("ApiKeys")]
    public class ApiKey :Node, INode
    {
        public ApiKey()
        {
            UUIDType = "ApiKey";
        }

        public DateTime? Created { get; set; }

        public DateTime? Expires { get; set; }

    }
}
