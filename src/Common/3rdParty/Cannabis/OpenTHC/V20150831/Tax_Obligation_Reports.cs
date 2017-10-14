﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public class Tax_Obligation_Reports
     {
        public long Id { get; set; } 
        public long OrgiId { get; set; } 
        public long Location { get; set; } 
        public int  Month                { get; set; } 
        public int  Year                 { get; set; } 
        public DateTime   Time_Start           { get; set; } 
        public DateTime   Time_End             { get; set; } 
        public decimal Gross_Sales          { get; set; } 
        public decimal     Excise_Tax       { get; set; } 
        public decimal Amount_Due       { get; set; } 
        public bool Locked       { get; set; } 
        public int Deleted { get; set; } 
        public DateTime    Unlock_Time              { get; set; } 
        public DateTime        Submit_Time          { get; set; } 
        public DateTime          Re_Submit_Time       { get; set; } 
        public long                TransactionId    { get; set; } 
        public long TransactionId_Original { get; set; }
    }

}