// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, OnInit, ViewChild , Output, EventEmitter, ElementRef} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AccordionModule, CheckboxModule, PickListModule } from 'primeng/primeng';
import { ConfirmDialogModule, ConfirmationService, GrowlModule, AutoCompleteModule } from 'primeng/primeng';
import { MessageBoxesComponent } from '../../common/messageboxes.component';
import { GraphsComponent } from '../../common/graphs.component';
import { BasicValidators } from '../../common/basicValidators';
import { AppService } from '../../services/app.service';
import { SessionService } from '../../services/session.service';
import { ReportService } from '../../services/report.service';
import { ApiKey } from '../../models/ApiKey';
import { Filter } from '../../models/filter';
import { Screen } from '../../models/screen';
import { DataPoint } from '../../models/datapoint';
import { List } from 'linqts';

@Component({
    templateUrl: './keys.component.html',
    providers: [ReportService, ConfirmationService, SessionService,   AppService]
})
export class KeysComponent implements OnInit {

    form: FormGroup;
    title: string;
    newKey = false;
    selectedApiKey = new ApiKey();
    savingData = false;
    apiKeys: ApiKey[] = [];
    loadingData = false;
    deletingData = false;
    displayDialog = false;
    newApiKey = false;
    showUsage = false;
    dataSet: DataPoint[] = [];
    @ViewChild(GraphsComponent) graphs: GraphsComponent;


    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor( fb: FormBuilder,
        private _router: Router,
        private _route: ActivatedRoute,
        private _appService: AppService,
        private _sessionService: SessionService,
        private _reportService: ReportService ) {



        if (this._sessionService.CurrentSession.validSession) {

            this.form = fb.group({
                name: ['', Validators.required],
                Email: ['', BasicValidators.email],
                PasswordQuestion: ['', Validators.required],
                PasswordAnswer: ['', Validators.required],
            });
        } else {
            this.form = fb.group({
                name: ['', Validators.required],
                Email: ['', BasicValidators.email],
                password: ['', Validators.compose([
                    Validators.required
                ])],
                confirmPassword: ['', Validators.required],
                PasswordQuestion: ['', Validators.required],
                PasswordAnswer: ['', Validators.required],

            });
        }
    }

    ngOnInit() {
        if (!this._sessionService.CurrentSession.validSession) {
            this._router.navigate(['/membership/login'], { relativeTo: this._route });
            return;
        }

        const filter = new Filter();
        filter.PageResults = false;

        const screenDate = new Screen();
        screenDate.Command = 'BETWEEN';
        screenDate.Field = 'DateCreated';
        screenDate.Operator = 'INCLUSIVE';
        const today = new Date();
        today.setMonth(0);
        today.setHours(0, 0, 0, 0);
        // Start date on left of pipe, end date on right of pipe. no pipe and end date = now
        screenDate.Value  = today.toJSON().slice(0, 10).replace(/-/g, '/').toString();
        screenDate.Value  += '|' + today.toJSON().slice(0, 10).replace(/-/g, '/').toString();
        screenDate.Order = 1;
        filter.Screens.push(screenDate);

        const screen = new Screen();
        screen.Command = 'SearchBy';
        screen.Field = 'SyncType';
        screen.Operator = 'Contains';
        screen.Value = 'APIKey';
        screen.Order = 0;
        filter.Screens.push(screen);

        this._reportService.getDataset('AccessLog', 'SyncType', filter).subscribe(response => {
            this.loadingData = false;
            this.showUsage = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            if (response.Result.length === 0) {
                this.msgBox.ShowMessage('info', 'Key usage results returned.', 10);
                return false;
            }
            this.showUsage = true;

            this.dataSet = response.Result;

            const list = new  List<DataPoint>();
            this.dataSet.forEach(x => {
                if ( x.ValueType === 'AccessLog') {
                    x.Value = JSON.parse(x.Value);
                    list.Add(x);
                }
            });
            const graphData = this._reportService.compile(list,  'sum', 'name');

            this.graphs.graph(graphData);

        }, err => {
            this.msgBox.ShowResponseMessage(err.status, 10);
            this.loadingData = false;
            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });

    }

    showDialogToAdd() {
        this.newApiKey = true;
        this.selectedApiKey = null;
        this.selectedApiKey = new ApiKey();
        this.displayDialog = true;
    }

    cancel() {
       this.displayDialog = false;
    }

    onRowSelect(event) {
        this.newApiKey = false;
        console.log('onRowSelect data:', event.data);
        this.selectedApiKey = this.cloneApiKey(event.data);
       // this.displayDialog = true;
    }

    onEdit(event) {
        this.newApiKey = false;
        console.log('onEdit data:', event);
        this.selectedApiKey = this.cloneApiKey(event);
        this.displayDialog = true;
    }

    cloneApiKey(c: ApiKey): ApiKey {
        const apiKey = new ApiKey();
        for (const prop in c) {
            if (prop != null) {
            apiKey[prop] = c[prop];
        }
        }
        return apiKey;
    }

    findSelectedIndex(apiKey): number {
      for (let i = 0; i < this.apiKeys.length; i++) {

            if (this.apiKeys[i].UUID === apiKey.UUID) {
                return i;
            }
        }
        return -1;
    }


    loadApiKeys(  page?: number, pageSize?: number) {

        const filter = new Filter();
        filter.PageResults = true;
        filter.StartIndex = page;
        filter.PageSize = pageSize;

        const res = this._appService.getApiKeys(filter);
        res.subscribe(response => {
            this.loadingData = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.apiKeys = response.Result;
        }, err => {
            this.msgBox.ShowResponseMessage(err.status, 10);
            this.loadingData = false;
            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });
    }

    lazyLoadKeysList(event) {
        this.loadApiKeys(  event.first, event.rows);

    }

    SaveKey() {
        this.loadingData = true;
        this.msgBox.closeMessageBox();

        let res;

        if (this.newApiKey === true) {
            this.selectedApiKey.UUID = '';
            res = this._appService.addApiKey(this.selectedApiKey);

        } else {
            res = this._appService.updateApiKey(this.selectedApiKey);
        }

        res.subscribe(response => {

            this.loadingData = false;
            this.displayDialog = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            if (this.newApiKey) {
                this.msgBox.ShowMessage('info', 'Key added.', 10);
                this.selectedApiKey.UUID = response.Result.UUID;
                this.newApiKey = false;
                this.apiKeys.push(this.selectedApiKey);

            } else {
                this.msgBox.ShowMessage('info', 'Key updated.', 10);
                this.apiKeys[this.findSelectedIndex(this.selectedApiKey)] = this.selectedApiKey;
            }
            this.loadApiKeys(  1, 25);  // not updating the list so reload for now.
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

    onClickDeleteApiKey() {
        if (confirm('Are you sure you want to delete ' + this.selectedApiKey.Name + '?')) {
            console.log('deleting key:', this.selectedApiKey.UUID);
            this.deleteKey(this.selectedApiKey.UUID);
        }
    }

    deleteKey(keyUUID) {

        this.msgBox.closeMessageBox();
        this.deletingData = true;
        const res = this._appService.deleteApiKey(keyUUID);

        res.subscribe(response => {

            this.deletingData = false;
            this.displayDialog = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.msgBox.ShowMessage('info', 'Key deleted.', 10);
            const index = this.findSelectedIndex(this.selectedApiKey);
            // Here, with the splice method, we remove 1 object
            // at the given index.
            this.apiKeys.splice(index, 1);
            this.loadApiKeys(1, 25);

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


    toggleActive($event) {
        this.selectedApiKey.Active = !this.selectedApiKey.Active;
        console.log('this.selectedApiKey.Active:', this.selectedApiKey.Active);
        $event.stopPropagation();
    }

    onShowUsage(apiKey) {
        console.log('onShowUseage:', apiKey);
        this.showUsage  = false;
        const filter = new Filter();
        filter.PageResults = false;

        const screen = new Screen();
        screen.Command = 'SearchBy';
        screen.Field = 'SyncType';
        screen.Operator = 'Contains';
        screen.Value = 'APIKey';
        screen.Order = 0;
        filter.Screens.push(screen);

        const screen2 = new Screen();
        screen2.Command = 'SearchBy';
        screen2.Field = 'UserName';
       // screen.Operator = 'Contains';
        screen2.Value  = apiKey.Key;
        screen2.Order = 1;

        filter.Screens.push(screen2);

        const screenDate = new Screen();
        screenDate.Command = 'BETWEEN';
        screenDate.Field = 'DateCreated';
        screenDate.Operator = 'INCLUSIVE';
        const today = new Date();
        today.setMonth(0);
        today.setHours(0, 0, 0, 0);
        // Start date on left of pipe, end date on right of pipe. no pipe and end date = now
        screenDate.Value  = today.toJSON().slice(0, 10).replace(/-/g, '/').toString();
        screenDate.Value  += '|' + today.toJSON().slice(0, 10).replace(/-/g, '/').toString();
        screenDate.Order = 2;
        filter.Screens.push(screenDate);

        this._reportService.getDataset('AccessLog', 'SyncType', filter).subscribe(response => {
            this.loadingData = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            if (response.Result.length === 0) {
                this.msgBox.ShowMessage('info', 'Key usage results returned.', 10);
                return false;
            }
            this.showUsage = true;

            this.dataSet = response.Result;

            const list = new  List<DataPoint>();
            this.dataSet.forEach(x => {
                if ( x.ValueType === 'AccessLog') {
                    x.Value = JSON.parse(x.Value);
                    list.Add(x);
                }
            });
            const graphData = this._reportService.compile(list,  'sum', 'name');
            this.graphs.graph(graphData);

        }, err => {
            this.msgBox.ShowResponseMessage(err.status, 10);
            this.loadingData = false;
            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });

        // this.compileReport('count', 'fieldName');//todo, add this to graphs?
        //  this.graph.ShowReport(this.Dataset);


        // this.graphs.
        // api/Reports/{category}/Dataset/{field}
        // api/Reports/AccessLog/Dataset/{field}
        //  SyncType = "APIKey",

        // see build all solution, DatasetManagerTests Javascript GetFilters function.
        // the screens are still be the same as whats implemented on the server, it's just
        // using the old javascript way instead of classes like typescript.
        // ------------

        /*
        const filter = new Filter();
        const screen = new Screen();
        screen.Field = 'CategoryType';
        //screen.Operator = 'CONTAINS';
        screen.Command = 'SearchBy';
        screen.Value = 'Product';
        filter.Screens.push(screen);

        ==================================
          const filter = new Filter();
            filter.PageResults = true;
            filter.StartIndex = page;
            filter.PageSize = pageSize;
            const screen = new Screen();
            screen.Field = 'TransactionDate';
            screen.Command = 'ORDERBY';
            screen.Value = '';
            filter.Screens.push(screen);
        ==================================
         const screen = new Screen();
            screen.Field = 'NAME';
            screen.Command = 'DISTINCTBY';
            screen.Value = '';
            filter.Screens.push(screen);
                ==================================
                        ==================================
        */
    }
}

