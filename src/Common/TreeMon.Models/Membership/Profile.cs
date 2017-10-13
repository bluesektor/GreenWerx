// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Membership
{
    [Table("ProfileLogs")]
    public  class Profile:Node
    {
        public Profile()
        {
            UUIDType = "Profile";

        }
        public DateTime? DOB { get; set; }

        //estimated,actual
        public string DobType { get; set; }

        public string Gender { get; set; }

        [StringLength(32)]
        public string UserUUID { get; set; }

        public float Height { get; set; }


        public string HeightUOM { get; set; }


        public float Weight { get; set; }

        public string WeightUOM { get; set; }

        public float BodyFat { get; set; }

    }


    public class Employee : Node
    {
        public Employee()
        {
            UUIDType = "Employee";
        }


        public string FirstName { get; set; }

        public string SurName { get; set; }
        public DateTime? BirthDate { get; set; }

        public DateTime? HireDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? LeaveDate { get; set; }


        //references the user table.
        [StringLength(32)]
        public long UserUUID { get; set; }



    }
}
