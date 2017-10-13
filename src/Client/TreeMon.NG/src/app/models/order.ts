// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Node } from '../models/node';

export class Order extends Node {

    constructor() {
        super();
    }

    AddedBy: string;

    AffiliateUUID: number;

    BillingLocationUUID: string;

    CurrencyUUID: string;

    CustomerEmail: string;

    Discount: number;

    FinancAccountUUID: string;

    ReconciledToAffiliate: boolean;

    ShippingCost: number;

    ShippingDate: Date;

    ShippingLocationUUID: string;

    ShippingMethodUUID: string;

    ShippingSameAsBiling: boolean;

    SubTotal: number;

    Taxes: number;

    Total: number;

    TrackingUUID: string;

    TransactionID: string;

    UserUUID: string;

    CartUUID: string;

    PayStatus: string;

}
