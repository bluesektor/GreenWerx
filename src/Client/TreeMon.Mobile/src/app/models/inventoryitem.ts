// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Node } from './node';

export class InventoryItem extends Node {

    Condition: string;

    DetailView: boolean;

    Quantity: number;

    ReferenceType: string;

    ReferenceUUID: string;

    Virtual: boolean;

    LocationUUID: string;

    Published: boolean;

    CategoryUUID: string;

    GroupUUID: string;

    Custom: boolean;

    CategoryName: string;

    Discount: number;

    DiscountType: string;

    MarkUp: number;

    MarkUpType: string;

    Price: number;

    SKU: string;

    ManufacturerUUID: string;

    Expires: string;

    SerialNumber: string;

    SystemUUID: string;

    Weight: number;

    WeightUOM: string;

    UOM: string; // this is not in the server model.

    CartItemUUID: string;

    Link: string;

    LinkProperties: string;

    LocationType: string;

    Description: string;
}
