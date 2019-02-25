// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, ViewChild, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageBoxesComponent } from '../../common/messageboxes.component';
import { BasicValidators } from '../../common/basicValidators';
import { SessionService } from '../../services/session.service';
import { AccordionModule } from 'primeng/primeng';
import { CheckboxModule } from 'primeng/primeng';
import { PickListModule } from 'primeng/primeng';
import { ConfirmDialogModule, ConfirmationService, GrowlModule } from 'primeng/primeng';

import { AccountService } from '../../services/account.service';
import { Account } from '../../models/account';


@Component({
    templateUrl: './accounts.component.html',
    styles: [`
.ui-picklist-source-controls {
  display: none !important;
}

.ui-picklist-target-controls {
  display: none !important;
}

.ui-picklist-listwrapper {
  width: 45% !important;
}
  `],
    providers: [AccountService, ConfirmationService, SessionService]
})
export class AccountsComponent implements OnInit {

    loadingData = false;
    deletingData = false;
    selectedTab = 0;

    // ===--- Top Menu Bar ---===
    newAccount = false;
    accounts: any[];

    // this is the current account the user is logged into (default account)
    activeAccount: any;
    settingActiveAccount = false;
    msgs: any[] = [];

    // ===--- Account Detail Tab (0) ---===
    accountDetail = new Account();
    formAccountDetail: FormGroup;

    // ===--- Account Users Tab (1) ---===
    accountNonMembers: Node[];
    accountMembers: Node[];

    @Output() onMoveToSource: EventEmitter<any> = new EventEmitter();
    @Output() onMoveToTarget: EventEmitter<any> = new EventEmitter();

    // ===--- Account Permissions Tab (2) ---===
    availablePermissions: Node[];
    selectedPermissions: Node[];

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(fb: FormBuilder,
        private _accountService: AccountService,
        private _confirmationService: ConfirmationService,
        private _sessionService: SessionService,
        private _router: Router,
        private _route: ActivatedRoute) {

        this.formAccountDetail = fb.group({
            Name: ['', Validators.required],
            Email: ['', BasicValidators.email],
            Private: '',
            Active: '',
            SortOrder: 0
        });

    }

    // ===--- General Events ---===

    ngOnInit() {
        this.loadingData = true;

        if (!this._sessionService.CurrentSession.validSession) {
            return;
        }

        this.loadAccountDropDown();
    }

    onTabShow(e) {
        console.log('tab index:', e.index);
        switch (e.index) {
            case 0:
                this.selectedTab = 0;
                this.showDetails(this.accountDetail.UUID);
                break;
            case 1:
                this.selectedTab = 1;
                this.showAccountUsers(this.accountDetail.UUID);
                this.showNonAccountUsers(this.accountDetail.UUID);
                break;
            case 2:
                this.selectedTab = 2;
                this.showPermissions(this.accountDetail.UUID);
                break;
        }
    }

    // ===--- Top Menu Bar ---===

    loadAccountDropDown() {
        const res = this._accountService.getAccounts();

        res.subscribe(response => {
            this.loadingData = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.accounts = response.Result;

            for (const account of response.Result) {
                if (account.UUID === this._sessionService.CurrentSession.userAccountUUID) {
                    this.activeAccount = account;

                    this.accountDetail.UUID = account.UUID;
                    this.showDetails(this.accountDetail.UUID);
                    break;
                }
            }
        }, err => {
            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });
    }

    cboAccountsChange(selectedAccountUUID) {

        this.accountDetail.UUID = selectedAccountUUID;

        this.onTabShow({ 'index': this.selectedTab });
    }

    onClickSetActiveAccount(event) {

        this.settingActiveAccount = true;

        const res = this._accountService.setActiveAccount(this.accountDetail.UUID);

        res.subscribe(response => {

            this.settingActiveAccount = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.activeAccount = Object.assign({}, this.accountDetail);
            this._sessionService.CurrentSession.userAccountUUID = this.accountDetail.UUID.toString();
            this._sessionService.SaveSessionState();
            this.msgBox.ShowMessage('info', 'Default account updated.', 10);

        }, err => {
            this.settingActiveAccount = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });

    }

    deleteAccount(accountUUID) {

        this.deletingData = true;
        const res = this._accountService.deleteAccount(accountUUID);

        res.subscribe(response => {

            this.deletingData = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.msgBox.ShowMessage('info', 'Account deleted.', 10);
            this.loadAccountDropDown();
        }, err => {
            this.deletingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });

    }

    onClickAddNewAccount(event) {
        this.accountDetail = new Account();
        this.newAccount = true;
        // this.formAccountDetail.controls.
    }

    onClickDeleteAccountDetail(event) {

        this._confirmationService.confirm({
            message: 'Do you want to delete this ACCOUNT?',
            header: 'Delete Confirmation',
            icon: 'fa fa-trash',
            accept: () => {
                this.msgs = [];
                this.msgs.push({ severity: 'info', summary: 'Confirmed', detail: 'Account deleted' });
                this.deleteAccount(this.accountDetail.UUID);
                this.accountDetail = new Account();
            }
        });
    }


    // ===--- Account Detail Tab (0) ---===

    saveAccountDetail() {
        this.loadingData = true;

        let res;

        if (this.newAccount) {
            res = this._accountService.addAccount(this.accountDetail);

        } else {
            res = this._accountService.updateAccount(this.accountDetail);
        }

        res.subscribe(response => {

            this.loadingData = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            // if account is created add the current user to it so it will load in the cbo box.
            if (this.newAccount) {

                this.msgBox.ShowMessage('info', 'Account added.', 10);
                this.accountDetail.UUID = response.Result.UUID;
                this._accountService.addUserToAccount(this.accountDetail.UUID,
                    this._sessionService.CurrentSession.userUUID).subscribe( sessionResponse => {

                        if (sessionResponse.Code !== 200) {
                            this.msgBox.ShowMessage(sessionResponse.Status, sessionResponse.Message, 10);
                            return false;
                        }
                        this.newAccount = false;
                        this.loadAccountDropDown();
                    });
            } else {
                this.msgBox.ShowMessage('info', 'Account updated.', 10);
            }
        }, err => {
            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });

    }

    showDetails(accountUUID) {

        const res = this._accountService.getAccount(accountUUID);

        res.subscribe(response => {

            this.loadingData = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.accountDetail = response.Result;

        }, err => {
            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });

    }


    // ===--- Account Users Tab (1) ---===

    showNonAccountUsers(accountUUID) {

        this.loadingData = true;

        const resNonMembers = this._accountService.getNonMembers(accountUUID);

        resNonMembers.subscribe(response => {

            this.loadingData = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.accountNonMembers = response.Result;

        }, err => {
            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });
    }

    showAccountUsers(accountUUID) {

        // Load account members..
        const resMembers = this._accountService.getMembers(accountUUID);

        resMembers.subscribe(response => {

            this.loadingData = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.accountMembers = response.Result;

        }, err => {
            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }
        });
    }

    addUsers(event: any) {

        this.loadingData = true;

        const res = this._accountService.addUsersToAccount(this.accountDetail.UUID, event.items);

        res.subscribe(response => {

            this.loadingData = false;

            if (response.Code !== 200) {
                this.loadingData = false;
                this.showAccountUsers(this.accountDetail.UUID);
                this.showNonAccountUsers(this.accountDetail.UUID);
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.msgBox.ShowMessage('info', 'Users added.', 10);

        }, err => {

            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });
    }

    removeUsers(event: any) {

        this.loadingData = true;


        const res = this._accountService.removeUsersFromAccount(this.accountDetail.UUID, event.items);

        res.subscribe(response => {

            this.loadingData = false;

            if (response.Code !== 200) {
                this.showAccountUsers(this.accountDetail.UUID);
                this.showNonAccountUsers(this.accountDetail.UUID);
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.msgBox.ShowMessage('info', 'Users removed.', 10);
        }, err => {
            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });

    }

    // ===--- Account Permissions Tab (2) ---===

    showAvailablePermissions(accountUUID) {

    }

    showSelectedPermissions(accountUUID) {

    }

    addPermissions(event: any) {

    }

    removePermissions(event: any) {

    }

    showPermissions(accountUUID) {
        this.msgBox.closeMessageBox();
        this.loadingData = false;
    }
}
