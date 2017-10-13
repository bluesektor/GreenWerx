// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.ComponentModel.DataAnnotations.Schema;
using TreeMon.Models.Store;

namespace TreeMon.Web.Models
{
    [Table("Products")]
    public class ProductForm:Product
    {
        [NotMapped]
        public string Captcha { get; set; }
    }
}