// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Node } from './node';
import { InventoryItem } from './inventory';
import { User } from './user';
import { Address } from './address';
import { PriceRule } from './pricerule';

export class ShoppingCart extends Node {

    AffiliateUUID: string;

    CurrencyUUID: string;

    subTotal = 0;

    total = 0;

    shippingCost = 0;

    taxes = 0;

    discount = 0;

    shippingSameAsBiling = false;

    customer: User = new User();

    BillingAddress: Address = new Address();

    ShippingAddress: Address = new Address();

    coupon: PriceRule = new PriceRule();

    CartItems: InventoryItem[] = [];

    UserUUID = '';

    PaymentGateway = '';

    IsPaymentSandbox = false;

    // who got paid
    PaidTo = '';

    // email, accountUUID...
    // i.e. papal uses email to identify who's getting paid
    PaidType = '';

    FinanceAccountUUID = '';

    // public string ShippingMethodUUID { get; set; }
}
