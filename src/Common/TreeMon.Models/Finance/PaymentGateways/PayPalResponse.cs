// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
namespace TreeMon.Models.Finance.PaymentGateways
{
    public class PayPalResponse
    {

        public decimal mc_gross { get; set; }
        public decimal tax{ get; set; } 
       public decimal payment_fee{ get; set; }
       public decimal handling_amount{ get; set; }
                public decimal payment_gross{ get; set; } 
       public decimal shipping { get; set; }
        public decimal mc_fee{ get; set; }
        public string payment_status { get; set; } //completed
        public int quantity { get; set; }

        public string  protection_eligibility{ get; set; }
        public string address_status{ get; set; }
        public string payer_id{ get; set; } 
        public string address_street{ get; set; } 
        public string payment_date{ get; set; }//20 % 3A12 % 3A59 + Jan + 13 % 2C + 2009 + PST 
       
        public string charset{ get; set; } 
        public string address_zip{ get; set; }  
        public string first_name{ get; set; }  
        public string address_country_code{ get; set; }
        public string address_name{ get; set; } 
        public float notify_version{ get; set; }
        public string custom{ get; set; }//this is set to the cartUUID, the Order has a cartUUID field. Pull the order and update all the tables. see StoreManager.ProcessPayment() to figure out all the tables to update
        public string payer_status{ get; set; }
        public string address_country{ get; set; }
        public string address_city{ get; set; }
        
        public string verify_sign{ get; set; } 
        public string payer_email { get; set; }
        public string txn_id{ get; set; } 
        public string payment_type{ get; set; }  
        public string last_name{ get; set; }  
        public string address_state{ get; set; }  
        public string receiver_email{ get; set; } 
        public string receiver_id{ get; set; } 
        public string txn_type{ get; set; } 
        public string item_name{ get; set; }
        public string mc_currency{ get; set; } 
        public string item_number { get; set; }
        public string residence_country{ get; set; }  
        public string test_ipn{ get; set; } 
        public string transaction_subject{ get; set; }
   
                  
    }

   
}
