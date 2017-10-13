// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Store
{
    [Table("Products")]
   public class Product:Item
    {
        public Product()
        {
            UUIDType = "Product";
        }

        [StringLength(32)]
        public string StrainUUID { get; set; }


        public string Link { get; set; }

        public string LinkProperties { get; set; }

    }
}
