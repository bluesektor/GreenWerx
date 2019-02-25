import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { SessionService } from '../../services/session.service';
import { MessageBoxesComponent } from '../../common/messageboxes.component';
import { DataTableModule, SharedModule, DialogModule } from 'primeng/primeng';

@Component({
    templateUrl: './api.component.html',
    providers: [ SessionService]

})
export class APIComponent implements OnInit {

    processingRequest = false;
    displayDialog: boolean;
    newUser: boolean;

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(
        private _router: Router,
        private _route: ActivatedRoute,
        private _sessionService: SessionService) {

       this.msgBox = new MessageBoxesComponent();

    }

    ngOnInit() {
    }

}

