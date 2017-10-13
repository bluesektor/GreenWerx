// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.ComponentModel.DataAnnotations.Schema;
using TreeMon.Models.Plant;

namespace TreeMon.Web.Models
{
    [Table("Strains")]
    public class StrainForm:Strain
    {
        [NotMapped]
        public string Captcha { get; set; }

        [NotMapped]
        public string BreederName { get; set; }

        [NotMapped]
        public string VarietyName { get; set; }
        
    }
}