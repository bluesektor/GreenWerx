// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Screen } from './screen';

export class Filter {

    constructor() {
        this.PageResults = true;
        this.StartIndex = 0;
        this.PageSize = 25;
        this.Screens = [];
    }

    PageResults = true;

    StartIndex = 0;

    PageSize = 25;

    // These are initial sorts, additional sorting can be
    // added to the screens.
    SortBy = '';

    SortDirection = '';

    Screens: Screen[] = [];

    TimeZone = '';

    IncludePrivate = false;

    IncludeDeleted = false;
}
