using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTrackTHC.V4
{
    
        //This contains employee information as previously submitted. It is UBI specific 
        //(as opposed to license specific) and can be queried with all records or only active 
        //ones.
    //
    class Employee
    {
        public string Employee_Name { get; set; } //variable length text field
        public string Employee_Id { get; set; } //unique variable length text field
        public int  Birth_Month  { get; set; }//two character integer
        public int Birth_Day { get; set; } //two character integer
        public int Birth_Year { get; set; } //four character integer
        public int  Hire_Month { get; set; } //two character integer
        public int  Hire_Day { get; set; } //two character integer 
        public int  Hire_Year  { get; set; }  //four character integer

        //Optional, integer, this is the first 
        //transactionid value received from 
        //creation of this employee. This can 
        //also be used to identify and update 
        //an existing record
        public int Transactionid_Original { get; set; }

        //references the user table. This was added by me.
        public long User_Id { get; set; }
         
    }
}
