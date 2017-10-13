// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Node } from '../models/node';

export class Location extends Node {


    // dlg has been added to to the client ui.
    // make sure when adding to ui the api/update
    // function updates the added properites.
    Code: string;

    CurrencyUUID: number;

    LocationType:    string;    // dlg

    Latitude:   number;         // dlg

    Longitude:  number;         // dlg

    TimeZoneUUID: number;

    Address1: string;           // dlg

    Address2: string;           // dlg

    City: string;               // dlg

    State: string;              // dlg

    Country: string;

    Postal: string;             // dlg

    Type: number;

    Description: string;

    isDefault: boolean;

    IsBillingAddress: boolean;  // dlg

    // This is to link an account to a location.
    // Set this to the account.UUID
    AccountReference: string;

    // This is the linked account name
    AccountName: string;        // dlg
}
