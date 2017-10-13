// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Node } from '../models/node';

export class Product extends Node {

    CategoryUUID: string;

    DepartmentUUID: string;

    Description: string;

    /// Discount for the product as a percentage.
     /// When updating these records:
     /// If you specify Discount without specifying price, the price is adjusted to accommodate the new Discount value,
     /// and the UnitPrice is held constant.
     /// If you specify both Discount and Quantity, you must also specify either TotalPrice or UnitPrice so
     // the system knows which one to automatically adjust.
    Discount: number;

    Expires: Date;

    ImagePath: string;

    // Number to apply the MarkUpType
    MarkUp: number;

    // percent, numeric, multiplier, function/formula (would have to figure this out first).
    MarkUpType: string;

    // This is the price on display for the customer
    //
    Price: number;
    SKU: string;
    StrainUUID: string;
    ManufacturerUUID: string;
    ManufacturerUUIDType: string;

    // Set to true if the product is virtual
    Virtual: boolean;

    Weight: number;

    UOMUUID: string;

    WeightUOM: string;

    // This is client side only.
    CategoryName: string;

    Link: string;

    LinkProperties: string;
}
