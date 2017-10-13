// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Node } from '../models/node';

export class Address extends Node {

    constructor() {
        super();
        this.isValid = false;
    }

    FirstName: string;

    LastName: string;

    ContactName: string;

    StreetAddress1: string;

    StreetAddress2: string;

    City: string;

    State: string; // Province

    Country: string;

    PostalCode: string; // Zip

    PhoneNumber: string;

    IsBillingAddress: boolean;

    Type: string; // home, business..

    isValid: boolean;

}
