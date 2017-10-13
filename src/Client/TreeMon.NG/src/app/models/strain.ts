// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Node } from '../models/node';

export class Strain extends Node {

        AutoFlowering:       boolean;

        GrowthRate:       string;

        FlowerColor:       string;

        Generation:       string;

        // Chronological  order
        ChronOrder:       number;

        Signature:       string;

        BreederUUID:       string; // pulled from accounts table

        Height:       number;

        HeightUOM:       string;

        // How many days from planting to harvest
        HarvestTime:       number;

        CategoryUUID:       string; // this is the Variety.. indica, sativa..

        IndicaPercent:       number;

        SativaPercent:       number;

        Lineage: string;

        SeedStability:       string;
}
