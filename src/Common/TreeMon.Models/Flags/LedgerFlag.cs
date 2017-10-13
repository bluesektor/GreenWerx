// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

//todo see if we need to implement these commonly used codes:
//* 26 = Invoice Cancelled
//* 33 = Currency Exchange Incorrect
//* 50 = Late Charge
//* 51 = Interest Penalty Charge
//* 52 = Credit for Previous Overpayment
//* 60 = No Open Item on File
//* 76 = Cash Discount
//* 80 = Overpayment
//* 81 = Credit as Agreed
//* 86 = Duplicate Payment
//* CS = Adjustment
//* L3 = Penalty
//* L5 = Interest Due
//* LP = Late Payment
//* PT = Payment
//* ZZ = Mutually Defined

namespace TreeMon.Models.Flags
{
    public class LedgerFlag
    {
        public struct LedgerCategories
        {
            public const string External = "external"; //external account
            public const string Internal = "internal"; //internal user account
            public const string Pool = "pool"; //
        }

        public struct TransactionTypes
        {
            public const string Credit = "CREDIT";
            public const string Debit = "DEBIT";
            public const string Refund = "REFUND";
            public const string Transfer = "TRANSFER";
            public const string WriteOff = "WRITEOFF";//set status unpaid

        }

        public struct LedgerTypes
        {
            public const string Escrow = "escrow"; //
            public const string Member = "member"; //user account
            public const string Pool = "pool"; //
        }

        public struct Status
        {
            public const string Completed = "COMPLETED";
         
            public const string Expense = "EXP"; //was debit
            public const string Income = "INC"; //was income

            public const string Pending = "PENDING"; //for log status containing both transaction id's

            //public const string = "UNPAID";
            //public const string = "PAID";
            //public const string = "ESCROW";
            //public const string = "COLLECTION";
            //public const string = "ANTE"; //is a credit type
            public const string PendingCredit = "PENDINGC";
            public const string PendingDebit = "PENDINGD";
            public const string PendingExpense = "PENDINGE"; //
            public const string PendingIncome = "PENDINGI";
          
            public const string SyncFromNetwork = "SYNCFROMNETWORK"; // this is used to flag the account so it'll get the balance from the network
          


            public const string Paid = "PAID";
            public const string OverPaymentReceived = "OVERPAYREC";
            public const string OverPaymentSent = "OVERPAYSENT";
            public const string PaymentPartialRecieved = "PAYPARTREC";
            //public const string PendingExpense = "PENDINGE";
            //public const string PendingIncome = "PENDINGI"; //pending income is used for waitning for confirmation...e.g. bitcoint  waiting for X number of confirmations.
            //public const string Refund = "REFUND";
            public const string ScheduledExpense = "SCHEDULEDE";

            //These should be used for transactions that occur on a timely basis.
            //
            public const string ScheduledIncome = "SCHEDULEDI";
            //public const string SyncFromNetwork = "SYNCFROMNETWORK";
            public const string Unpaid = "UNPAID";


            // example, creating the admin account with a balance of 0, and set this flag.
            // then have the finace server pull the btc address and get the balance from
            // the bitcoin servers, then have the finance server update the balance in the
            // ledger, add a ledger transaction and  reset the status flag in the ledger.


        }

        //owner = logged in user comencing the transaction on own account (account owner).
        public struct TransferTypeToken
        {
            public const string EscrowToOwner = "oeou"; //internal escrow account to owner/internal user account to  (xfer from users account to escrow/pool account)
            public const string ExternalToOwner = "euou"; //external user account to internal user account
            public const string MemberToPoolEscrow = "mtpe"; //transferring from member account to a pool escrow account
            public const string OwnerToEscrow = "ouoe"; //owner/internal user account to internal escrow account (xfer from users account to escrow/pool account)
            public const string OwnerToExternal = "oueu"; //owner/internal user account to a external user(different user) account (including pool(s), escrow etc.).
            public const string OwnerToOwner = "ouou"; //owner/internal user account to owner/internal user account (xfer from users account to same users other account)
        }

      
    }
}
