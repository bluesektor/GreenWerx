﻿// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Node } from './node';

export class PriceRule extends Node {

    Code: string;

    DateUsed: Date;

    // student, military, coupon, promo..
    // shipping, delivery
    ReferenceType: string;

    // how much the adjustment is (discount, tax etc.)
    Result: number;
    LocationUUID: string;

    // This is the "discount" multiplier
    Operand: number;

    Operator: string;

    Expires: Date;

    MaxUseCount: number;

    // set this for type like minimum delivery/shipping charge
    Minimum: number;

    // for maximum discount
    Maximum: number;

    Mandatory: boolean;
}
