// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, ViewChild, OnInit, Input, Output , EventEmitter } from '@angular/core';
import { PickListModule } from 'primeng/primeng';
import { WebApiService } from '../services/webApi.service';
import { SessionService } from '../services/session.service';
import { Node } from '../models/node';

// Implementation example
// <picklist-component [type]='User' > </picklist-component>
@Component({
    selector: 'app-picklistcomponent',

    templateUrl: './picklist.component.html',
    providers: [SessionService, WebApiService]
})
export class PickListComponent implements OnInit {

    @Input() type: any;
    @Input() pluralize = true; // NOTE: set this to false to statically define the route.

    availableNodes: Node[];
    selectedNodes: Node[];
    loadingData = false;
    pluralType: string;

    @Output() onMoveToSource: EventEmitter<any> = new EventEmitter();
    @Output() onMoveToTarget: EventEmitter<any> = new EventEmitter();

    constructor(private _sessionService: SessionService, private _webApiService: WebApiService ) {
    }

    ngOnInit() {

        this.pluralType = this.type;

        if (this.pluralize === true) {
            this.pluralType += 's';
        }

       // this._webApiService.setBaseRoute(this.pluralType);

    }

    showAvailableNodes() {

        this.loadingData = true;
        // the invokeRequest. wont work unless we extend webapi and call super with plural type or create a new instance
        //  or set the property call function to set _apiRoute in webapi

        const result = this._webApiService.invokeRequest('GET', ''    );
        result.subscribe(
            response => {
                this.loadingData = false;
                if (response.Code !== 200) {
                    return false;
                }
                this.availableNodes = response.Result;
            },
            err => {
                this.loadingData = false;
            }
        );
    }

    showSelectedNodes(accountUUID) {
    }

    addNodes(event: any) {
    }

    removeNodes(event: any) {
    }
}
