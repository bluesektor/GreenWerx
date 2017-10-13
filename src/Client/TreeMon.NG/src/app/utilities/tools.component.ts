// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, OnInit, ViewChild, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { SessionService } from '../services/session.service';
import { Setting } from '../models/setting';
import { AdminService } from '../services/admin.service';
import { AppService } from '../services/app.service';
import { MessageBoxesComponent } from '../common/messageboxes.component';
import { DataTableModule, SharedModule, DialogModule, FileUploadModule } from 'primeng/primeng';
import { ConfirmDialogModule, ConfirmationService, GrowlModule } from 'primeng/primeng';

@Component({
    templateUrl: './tools.component.html',
    providers: [AdminService, SessionService, AppService]

})

export class ToolsComponent implements OnInit {

    processingRequest = false;
    defaultDatabase = '';
    selectedDbBackup = '';
    importFiles: string[];
    selectedImportFile = '';
    databaseBackups: string[];
    msgs: any[] = [];
    testResult = '';

    tableNames: string[] = [];

     // =====--Table Maint.--=====
     selectedTable = '';
     scanResultCount: number;
     selectedDuplicate: any;
     searchResults: Node[] = [];

      // ===--- Encryption ---===
    cipherSource = '';
    cipherResult = '';

    // ===--- File upload and import ---===
    dataTypes: string[] = [];
    dataType: string;
    baseUrl: string;
    uploadedFiles: any[] = [];

    validate = true;
    validateGlobally = false;
    scanResult: any[];

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(
        private _router: Router,
        private _route: ActivatedRoute,
        private _toolsService: AdminService,
        private _appService: AppService,
        private _confirmationService: ConfirmationService,
        private _sessionService: SessionService) {

        this.msgBox = new MessageBoxesComponent();

    }

    ngOnInit() {
        this.dataType = '';
        this.loadDashboard();
        this.baseUrl = this._appService.BaseUrl();

        this._appService.tableNames().subscribe(response => {
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.tableNames = response.Result;
        }, err => {
            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }
        });

        this._appService.dataTypes().subscribe(response => {
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.dataTypes = response.Result;
        }, err => {
            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }
        });
    }

    loadDashboard() {
        this.processingRequest = true;
        this.msgBox.closeMessageBox();
        const res = this._toolsService.getToolsDashboard();

        res.subscribe(response => {

            this.processingRequest = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.databaseBackups = response.Result.Backups;
            this.defaultDatabase = response.Result.DefaultDatabase;
            this.importFiles = response.Result.ImportFiles; //  (ngModelChange)="cboImportFileChange($event)"

        }, err => {

            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }
        });
    }

    btnBackupDatabaseClick(event) {
        this.processingRequest = true;
        this.msgBox.closeMessageBox();

        const res = this._toolsService.backupDatabase();

        res.subscribe(response => {

            this.processingRequest = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.msgBox.ShowMessage('info', response.Message, 10);
            this.loadDashboard();
        }, err => {

            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }
        });
    }

    btnRestoreDatabaseClick(event) {
        this.processingRequest = true;
        this.msgBox.closeMessageBox();
        this._confirmationService.confirm({
            message: 'Do you want to restore this database?',
            header: 'Delete Confirmation',
            icon: 'fa fa-trash',
            accept: () => {
                this.msgs = [];
                this.msgs.push({ severity: 'info', summary: 'Confirmed', detail: 'Databse restored' });

                if (this.selectedDbBackup === '' ) {
                    this.msgBox.ShowMessage('error', 'You must select one file to restore..', 10);
                    return false;
                }
                // var countDown = $selectedFiles.length;
                let dataArray = '';
                // // Create a json array to upload to service so we can bulk insert.
                // $selectedFiles.each(function () {
                //     var lstFiles = $(this).data('record');
                dataArray += JSON.stringify({ FileName: this.selectedDbBackup});
                //     countDown--;
                //     if (countDown > 0) { dataArray += ","; }
                // });


                const res = this._toolsService.restoreDatabase('[' + dataArray + ']');
                res.subscribe(response => {

                    this.processingRequest = false;

                    if (response.Code !== 200) {
                        this.msgBox.ShowMessage(response.Status, response.Message, 10);
                        return false;
                    }

                    this.msgBox.ShowMessage('info', 'Database restored.', 10);


                }, err => {
                    this.processingRequest = false;
                    this.msgBox.ShowResponseMessage(err.status, 10);

                    if (err.status === 401) {
                        this._sessionService.ClearSessionState();
                        setTimeout(() => {
                            this._router.navigate(['/membership/login'], { relativeTo: this._route });
                        }, 3000);
                    }
                });
            }
        });
    }

    onDbBackupRowSelect(file) {
        this.selectedDbBackup = file;
    }


    btnImportFilesClick(event) {
        this.processingRequest = true;
        const res = this._toolsService.import(this.selectedImportFile.replace('.json', '') );

        res.subscribe(response => {

            this.processingRequest = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.msgBox.ShowMessage('info', 'File imported.', 10);


        }, err => {
            this.processingRequest = false;
            if (err.status === 429) {
                this._sessionService.ClearSessionState();
                this.msgBox.ShowMessage('error', 'Too many requests being sent.', 10);
                return;
            }
            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                this.msgBox.ShowMessage('error', err.status + ' Session expired.', 10    );

                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            } else {

                this.msgBox.ShowMessage('error', err.status + ' Failed to connect. Check your connection or try again later.', 10    );
            }
        });
        this.processingRequest = false;
    }

    btnEncryptClick(event) {
        this.msgBox.closeMessageBox();
        this.cipherText(true);
    }

    btnDecryptClick(event) {
        this.msgBox.closeMessageBox();
        this.cipherText(false);
    }

    cipherText(encrypt: boolean) {
        this.processingRequest = true;

        const res = this._toolsService.cipherText(this.cipherSource, encrypt);

        res.subscribe(response => {

            this.processingRequest = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.cipherResult = response.Result;


        }, err => {
            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }
        });
    }


    btnTestCodeClick(event) {
        this.processingRequest = true;
        this.testResult = '';

           // //   test ipn service
        // this._appService.testIPN('ipnfu').subscribe(response => {
        //     if (response.Code != 200) {
        //         this.msgBox.ShowMessage(response.Status, response.Message,10);
        //         return false;
        //     }
        //     var res = response.Result;
        //     // todo get app setting if pos system don't show this message
        //     this.msgBox.ShowMessage('info',
        //         res, 35);
        // });


        // ====== Other test code
        const res = this._toolsService.testCode();
        res.subscribe(response => {
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.testResult = response.Message;
        }, err => {
            this.processingRequest = false;
        this.msgBox.ShowResponseMessage(err.status, 10);

        if (err.status === 401) {
            this._sessionService.ClearSessionState();
            setTimeout(() => {
                this._router.navigate(['/membership/login'], { relativeTo: this._route });
            }, 3000);
        }
        });
    }

    onBeforeSendFile(event) {

        event.xhr.setRequestHeader('Authorization', 'Bearer ' + this._sessionService.CurrentSession.authToken);
    }

    onFileUpload( event ) {
        let currFile;
        for (const file of event.files) {
            this.uploadedFiles.push(file);
            currFile = file;
        }
        let result = JSON.parse( event.xhr.response);
        this.msgBox.ShowMessage(result.Status, result.Message, 15);
    }

    chkValidateClick(event) {
        console.log('chkValidateClick', event);
    }

    chkValidateGloballyClick(event) {
        console.log('chkValidateGloballyClick', event);
    }

    cboTableNamesChanged(event) {
        this.selectedTable = event;
    }

    cboDataTypeChanged(event) {
       this.dataType  = event;
    }


    btnScanTableNamesClick(event) {
        this.scanResult = [];
        this.searchResults = [];

        this._appService.scanForDuplicates(this.selectedTable).subscribe(response => {
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.scanResult = response.Result;
            this.scanResultCount = this.scanResult.length;
        }, err => {
            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }
        });
    }

    onRowSelectDuplicate(data) {
        this.searchResults = [];
        const uuids = [];
        for (let i = 0; i < data.group.length; i++) {
            uuids.push(data.group[i].UUID);
        }
        this._appService.searchTables(this.selectedTable, uuids).subscribe(response => {
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.searchResults = response.Result;
        }, err => {
            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }
        });

    }

    btnDeleteRecordClick(event, recordUUID) {

        this._appService.deleteItem(this.selectedTable, recordUUID).subscribe(response => {
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.msgBox.ShowMessage('info', 'Record deleted', 15);
        }, err => {
            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }
        });
    }
}

