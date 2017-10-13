// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Finance
{
    [Table("FinanceAccountTransactions")]
    public class FinanceAccountTransaction : Node, INode
    {
        public FinanceAccountTransaction() {
            this.UUIDType = "FinanceAccountTransaction";
        }
        public string   FinanceAccountUUID             { get; set; }

        public string   PayToAccountUUID                  { get; set; }// was   public string Address { get; set; }      public int AccountID { get; set; }

        public string   PayFromAccountUUID             { get; set; }// was public string AddressFrom { get; set; }

        public DateTime CreationDate                { get; set; }

        public string   CustomerIp                { get; set; } //    public string CustomerIp { get; set; }

        public DateTime? LastPaymentStatusCheck { get; set; }

        public string   OrderUUID                   { get; set; }

        public decimal Balance { get; set; }

        public decimal AmountTransferred { get; set; }//PayReceived

        public string    TransactionType             { get; set; }//paymentto, payment from see LedgerFlags

        public DateTime TransactionDate            { get; set; }

        public string   PaymentTypeUUID             { get; set; }

      

        public string   SelectedPaymentTypeSymbol { get; set; }

        public decimal SelectedPaymentTypeTotal { get; set; }

        public string   UserUUID                    { get; set; }

        public string   CustomerEmail              { get; set; }

        public string   CurrencyUUID              { get; set; }

        public string AccountEmail { get; set; }

    }
}
