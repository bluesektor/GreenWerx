// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NavBarAdminComponent } from './navbar.admin.component';
import { NavBarDefaultComponent } from './navbar.default.component';
import { MessageBoxesComponent } from './common/messageboxes.component';
import { SessionService } from './services/session.service';
import { Message } from './models/message';
import { AppService } from './services/app.service';
import { Filter } from './models/filter';
import { Screen } from './models/screen';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    providers: [AppService, SessionService]
})
export class AppComponent implements OnInit {

    pageSettings: any;

    collapseDiv = false;
    minHeight = 230;

    processingRequest = false;
    currentYear: number;
    domainName: string;
    private _appStatus: string;
    showNavbar = true;

    userIsLoggedIn = false;
    userIsAdmin = false;

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    @HostListener('window:resize', ['$event'])
    onResize(event) {
        let topOffset = 50;
        const width = (event.target.innerWidth > 0) ? event.target.innerWidth : event.target.screen.width;

        if (width < 768) {
            this.collapseDiv = true;
            topOffset = 100; // 2-row-menu
        } else {
            this.collapseDiv = false;
        }

        let height = ((event.target.innerHeight > 0) ? event.targetinnerHeight : event.target.screen.height) - 1;
        height = height - topOffset;
        if (height < 1) { height = 1; }
        if (height > topOffset) {
            this.minHeight = height;
        }
    }

    constructor(private _appService: AppService,
        private _sessionService: SessionService,
        private _router: Router,
        private _route: ActivatedRoute) { }

    ngOnInit() {
        this.currentYear = new Date().getFullYear();
        this.loadApp();
        this.userIsLoggedIn = this._sessionService.CurrentSession.validSession;
        this.userIsAdmin = this._sessionService.CurrentSession.isAdmin;
    }

    loadApp() {
        this.processingRequest = true;

        this._appService.getAppStatus().subscribe(response => {
            this.processingRequest = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this._appStatus = response.Result;

            if (this._appStatus === 'REQUIRES_INSTALL' || this._appStatus === 'INSTALLING') {
                this.showNavbar = false;
                this._router.navigate(['/install'], { relativeTo: this._route });
                return;
            }

            this.processingRequest = true;
            const filter = new Filter();
            filter.PageResults = true;
            filter.StartIndex = 1;
            filter.PageSize = 100;
            const screen = new Screen();
            screen.Command = 'SearchBy';
            screen.Field = 'Name';
            screen.Value = 'SiteDomain'; // for now we just need domain name.
            filter.Screens.push(screen);
            this._appService.getPublicSettings(filter).subscribe(settingResponse => {
                this.processingRequest = false;
                if (settingResponse.Code !== 200) {
                    this.msgBox.ShowMessage(settingResponse.settingResponse, response.Message, 10);
                    return false;
                }
                if (settingResponse.Result.length > 0) {
                    this.domainName = settingResponse.Result[0].Value;
                }
            });

        }, err => {
            this.processingRequest = false;
            // Show everything but the session time out.
            if (err.status !== 401) {
                this.msgBox.ShowResponseMessage(err.status, 10);
            }
        });
    }
}
