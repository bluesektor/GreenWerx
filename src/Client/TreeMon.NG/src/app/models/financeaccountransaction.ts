// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Node } from '../models/node';

export class FinanceAccountTransaction extends Node {

    FinanceAccountUUID: string;

    PayToAccountUUID: string;

    PayFromAccountUUID: string;

    CreationDate: Date;

    CustomerIp: string;

    LastPaymentStatusCheck: Date;

    OrderUUID: string;

     // Amount Transferred
    Amount: number;

    TransactionType: string;

    TransactionDate: Date;

    PaymentTypeUUID: string;

    Balance: number;

    SelectedPaymentTypeSymbol: string;

    SelectedPaymentTypeTotal: number;

    UserUUID: string;

    CustomerEmail: string;

    CurrencyUUID: string;
}
