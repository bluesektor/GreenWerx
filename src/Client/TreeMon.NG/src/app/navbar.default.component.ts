// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, ElementRef } from '@angular/core';
import { AppService } from './services/app.service';
import { SessionService } from './services/session.service';
import { Setting } from './models/setting';
import { SettingsService } from './services/settings.service';

@Component({
    selector: 'tm-navbar-default',
    templateUrl: './navbar.default.component.html',
    providers: [AppService, SessionService, SettingsService]
})

export class NavBarDefaultComponent {
    pageTitle = '';
    pageSettings: any;
    highlightedDiv: number;
    navbarLinks: any[] = [];
    userIsLoggedIn =  false;
    userIsAdmin = false;
    userDropDownExpanded = false;

    constructor(
        private _sessionService: SessionService,
        private _appSettings: AppService,
        private _settingsService: SettingsService) {

        this.loadSettings();
    }

    loadSettings() {

        this.userIsLoggedIn = this._sessionService.CurrentSession.validSession;
        this.userIsAdmin =  this._sessionService.CurrentSession.isAdmin;

        const res = this._appSettings.getDashboard('navbar_default', '' );
        res.subscribe(response => {
            if (response.Code !== 200) {return false; }
            this.pageSettings = response.Result;
            this.pageTitle = this.pageSettings.Title;
            this.navbarLinks = this.initializeTopMenu(this.pageSettings.TopMenuItems);

            this.toggleActive(0); // set first one as active. should be home page
        }, err => {

        });
    }

    initializeTopMenu(navLinks: any[]) {
        const menuItems = [];

        for (let i = 0; i < navLinks.length; i++) {
            const itm = {
                label: navLinks[i].label,
                icon: navLinks[i].icon,
                routerLink: navLinks[i].href, // [navLinks[i].href], <= use this if converting to primeng
                // url: navLinks[i].href,
                // items: this.initializeTopMenu(navLinks[i].items) //recursive add sub-menu

                // command?: (event?: any) => void;
                // url?: string;
                // routerLink?: any;
                // eventEmitter?: EventEmitter<any>;
                //  items?: MenuItem[];
                // expanded?: boolean;
                // disabled?: boolean;
                // visible?: boolean;
                // target?: string;
            };
            // if (navLinks[i].items.length == 0) {
            //    itm.items = null;
            // }

            menuItems.push(itm);
        }
        return menuItems;
    }


    toggleActive(index) {

        if (this.highlightedDiv !== index) {
            this.highlightedDiv = index;
        }
    }


    toggleUserDropDown() {
        this.userDropDownExpanded = !this.userDropDownExpanded;

        if (this.userDropDownExpanded === true) {
            // This was the only way I could get the menu to toggle (reload the object).
            // I tried observables, emitters.. nothing worked.
            this._sessionService.LoadSessionState();
        }
    }

}
