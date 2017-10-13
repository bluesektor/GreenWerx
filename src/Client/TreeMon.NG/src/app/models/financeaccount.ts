// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Node } from '../models/node';

export class FinanceAccount extends Node {

    AccountNumber: string;

    AssetClass: string;

    Balance: number;

    CurrencyUUID: string;

    CurrencyName: string;

    IsTest: boolean;

    ServiceAddress: string;

    SourceClass: string;

    SourceUUID: string;

    UsedBy: string;

    UsedByClass: string;

    LocationType: string;

    ClientCode: string;
}
