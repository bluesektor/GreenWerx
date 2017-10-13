// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
namespace TreeMon.Models.Flags
{
    public class StoreFlag
    {
        #region Nested Types

        public struct OrderStatus
        {

            public const string Cancelled = "CANCELLED";
            public const string Deleted = "DELETED";
            public const string Recieved = "RECIEVED";

            //item shipped
            //cancelled
            //refunded
            //returned
            //

          
        }

        //public struct PayStatus
        //{

        //    public const string Paid = "PAID";
        //    public const string OverPaymentReceived = "OVERPAYREC";
        //    public const string OverPaymentSent = "OVERPAYSENT";
        //    public const string PaymentPartialRecieved = "PAYPARTREC";
        //    public const string PendingExpense = "PENDINGE";
        //    public const string PendingIncome = "PENDINGI"; //pending income is used for waitning for confirmation...e.g. bitcoint  waiting for X number of confirmations.
        //    public const string Refund = "REFUND";
        //    public const string ScheduledExpense = "SCHEDULEDE";

        //    //These should be used for transactions that occur on a timely basis.
        //    //
        //    public const string ScheduledIncome = "SCHEDULEDI";
        //    public const string SyncFromNetwork = "SYNCFROMNETWORK";
        //    public const string Unpaid = "UNPAID";
        //    public const string WriteOff = "WRITEOFF";

        //}

        #endregion Nested Types
    }
}
