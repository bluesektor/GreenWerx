// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, ViewEncapsulation, OnInit, ViewChild, Output, EventEmitter  } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { MessageBoxesComponent } from './common/messageboxes.component';
import { StepsModule, MenuItem, CheckboxModule } from 'primeng/primeng';
import { AppInfo } from './models/appinfo';
import { AppService } from './services/app.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
    templateUrl: './install.component.html',
    styleUrls: [
         './install.component.css'
    ],
    encapsulation: ViewEncapsulation.None,
    providers: [AppService]

})

export class InstallComponent implements OnInit {

    step1Valid = false;
    step2Valid = false;
    step3Valid = false;
    enableNext = false;
    seedDatabase = false;
    activeIndex = 0;
    maxSteps = 2;
    providers: any[];
    showDbCredentials = false;
    processingRequest = false;
    private _appStatus: string;


    public items: MenuItem[];

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;
    private _appInfo: AppInfo = new AppInfo();

    public dynamicControlDatabaseServer: FormControl = new FormControl('', [this.ValidateData.bind(this)]);
    public dynamicControlActiveDatabase: FormControl = new FormControl('', [this.ValidateData.bind(this)]);
    public dynamicControlActiveDbUser: FormControl = new FormControl('', [this.ValidateData.bind(this)]);
    public dynamicControlActiveDbPassword: FormControl = new FormControl('', [this.ValidateData.bind(this)]);


    public dynamicControlUserName: FormControl = new FormControl('', [this.ValidateData.bind(this)]);
    public dynamicControlUserEmail: FormControl = new FormControl('', [this.ValidateData.bind(this)]);
    public dynamicControlUserPassword: FormControl = new FormControl('', [this.ValidateData.bind(this)]);
    public dynamicControlConfirmPassword: FormControl = new FormControl('', [this.ValidateData.bind(this)]);
    public dynamicControlSecurityQuestion: FormControl = new FormControl('', [this.ValidateData.bind(this)]);
    public dynamicControlUserSecurityAnswer: FormControl = new FormControl('', [this.ValidateData.bind(this)]);

    formAppSettings: FormGroup;

    constructor(fb: FormBuilder,
        private _router: Router,
        private _route: ActivatedRoute,
        private _appService: AppService
    ) {
        this.formAppSettings = fb.group({
            SiteDomain: ['', Validators.required],
            ActiveDbProvider: '',
            DatabaseServer: '',
            ActiveDatabase: '',
            ActiveDbUser: '',
            ActiveDbPassword: '',
            UserEmail: '',
            UserName: '',
            UserPassword: '',
            ConfirmPassword: '',
            SecurityQuestion: '',
            UserSecurityAnswer: ''
        });

        this._appInfo.AccountEmail = '';
        this._appInfo.AccountIsPrivate = true;
        this._appInfo.AccountName = '';
        this._appInfo.ActiveDatabase = '';
        this._appInfo.ActiveDbConnectionKey = '';
        this._appInfo.ActiveDbPassword = '';
        this._appInfo.ActiveDbProvider = '';
        this._appInfo.ActiveDbUser = '';
        this._appInfo.AppType = 'web';
        this._appInfo.ConfirmPassword = '';
        this._appInfo.DatabaseServer = 'localhost';
        this._appInfo.PasswordSalt = '';
        this._appInfo.RunInstaller = true;
        this._appInfo.SecurityQuestion = '';
        this._appInfo.SiteDomain = '';
        this._appInfo.UserEmail = '';
        this._appInfo.UserIsPrivate = true;
        this._appInfo.UserName = 'bluesektor';
        this._appInfo.UserPassword = '';
        this._appInfo.UserSecurityAnswer = '';
    }

    public ValidateData(control: FormControl): { [key: string]: boolean } {

       if (!this._appInfo) {
          return {}; }

        switch (this.activeIndex) {

            case 0:
                this.step1Valid = false;
                this.enableNext = false;

                if (this._appInfo.ActiveDbProvider === 'sqlite') {
                    return {};
                }

                if (!this._appInfo.DatabaseServer) {
                    return { required: true };
                }
                if (!this._appInfo.ActiveDatabase) {
                    return { required: true };
                }
                if (!this._appInfo.ActiveDbUser) {
                    return { required: true };
                }
                if (!this._appInfo.ActiveDbPassword) {
                    return { required: true };
                }

                this.step1Valid = true;
                this.enableNext = true;
                break;
            case 1:
                this.step2Valid = false;
                this.enableNext = false;

                if (!this._appInfo.UserName) {
                    return { required: true };
                }
                if (!this._appInfo.UserEmail) {
                    return { required: true };
                }
                if (!this._appInfo.UserPassword) {
                    return { required: true };
                }
                if (!this._appInfo.ConfirmPassword) {
                    return { required: true };
                }
                if (!this._appInfo.SecurityQuestion) {
                    return { required: true };
                }
                if (!this._appInfo.UserSecurityAnswer) {
                    return { required: true };
                }

                this.step2Valid = true;
                this.enableNext = true;
                break;

        }
        return {};
    }

    ngOnInit() {
        this._appInfo.Command = 'CreateDatabase';
        this._appInfo.ActiveDbProvider = '';
        this.items = [{ label: 'Database', }, { label: 'Admin Account' }, { label: 'Wrap up..' }];
        this.providers = Array() as any[];
        this.providers.push({ Name: 'MSSQL', Value: 'mssql' });
        this.checkInstallFlag();
    }


    cboDbProviderChange(selectedProvider) {
        this._appInfo.ActiveDbProvider = selectedProvider;

        this.showDbCredentials = false;
        switch (selectedProvider) {
            case 'mssql':
                this.showDbCredentials = true;
                break;
        }
    }

    CreateDatabase() {
        if ( this.processingRequest === true) {
            this.msgBox.ShowMessage('info', 'Already processing: ' + this._appInfo.Command, 10 );
            return;
        }
        this.processingRequest = true;
        this._appService.CreateDatabase(this._appInfo).subscribe(response => {
                   this.processingRequest = false;
                   if (response.Code !== 200) {
                       this.msgBox.ShowMessage(response.Status, response.Message, 9999999);
                       return;
                   }
                   this.msgBox.ShowMessage('info', 'Database created, saving settings..', 15);
                   this._appInfo.Command = 'SaveSettings';
                   this.SaveSettings();
               }, err => {
                   this.processingRequest = false;
                   this.msgBox.ShowResponseMessage(err.status, 9999999);
                   return false;
               });
    }

    SaveSettings() {
        if ( this.processingRequest === true) {
            this.msgBox.ShowMessage('info', 'Already processing: ' + this._appInfo.Command, 10 );
            return;
        }
        this.processingRequest = true;
        this._appService.SaveSettings(this._appInfo).subscribe(response => {
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 9999999);
                return;
            }
            this.msgBox.ShowMessage('info', 'Settings saved..', 15);
            if ( this.seedDatabase === true ) {
                this._appInfo.Command = 'SeedDatabase';
                this.SeedDatabase();
            }else { // if not seeding show the next button
                this._appInfo.Command = 'Accounts';
                this.activeIndex = 1;
                this.enableNext = false;
            }
        }, err => {
            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 9999999);
            return;
        });
    }

    SeedDatabase() {
        if ( this.processingRequest === true) {
            this.msgBox.ShowMessage('info', 'Already processing: ' + this._appInfo.Command, 10 );
            return;
        }
        this.processingRequest = true;
        this.msgBox.ShowMessage('info', 'Seeding the datbase it may take a while...', 15);
        this._appService.SeedDatabase(this._appInfo).subscribe(response => {
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 9999999);
                return;
            }
            this.msgBox.ShowMessage('info', 'Seeding datbase completed.', 15);
            this._appInfo.Command = 'Accounts';
            this.activeIndex = 1;
           this.enableNext = false;
        }, err => {
            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 9999999);
            return;
        });
    }

    CreateAccounts() {
        if ( this.processingRequest === true) {
            this.msgBox.ShowMessage('info', 'Already processing: ' + this._appInfo.Command, 10 );
            return;
        }
        this.processingRequest = true;
        this._appService.AddAccounts(this._appInfo).subscribe(response => {
            this.enableNext = false;
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 9999999);
                return;
            }
            this.msgBox.ShowMessage('info', 'Accounts created, finalize the settings.', 15);
            this._appInfo.Command = 'Finalize';
            this.activeIndex = 2;
            this.enableNext = true;
            this.Finalize();
        }, err => {
            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 9999999);
            return;
        });
    }

    Finalize() {
        if ( this.processingRequest === true) {
            this.msgBox.ShowMessage('info', 'Already processing: ' + this._appInfo.Command, 10 );
            return;
        }
        this.processingRequest = true;
        this._appService.Finalize(this._appInfo).subscribe(response => {
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 9999999);
                return;
            }
            this.msgBox.ShowMessage('info', 'Install completed, you will be redirected to the login..', 15);
            setTimeout(() => {
                this._router.navigate(['/membership/login'], { relativeTo: this._route });

            }, 3000);

        }, err => {
            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 9999999);
            return;
        });
    }

    onClickNextStep() {
        if ( this.processingRequest === true) {
            this.msgBox.ShowMessage('info', 'Already processing: ' + this._appInfo.Command, 10 );
        }
        this.processingRequest = true;
        this.msgBox.closeMessageBox();
        if (this.activeIndex === this.maxSteps) {
            return;
        }
     //   let gotoNextStep;
     //   gotoNextStep = false;
        switch (this._appInfo.Command) {
            case 'CreateDatabase': // Install Step 1
                 break;
            case 'SaveSettings':  // Install Step 1.A. call this from client right after the databaase is created
                break;
            case 'SeedDatabase': // Install Step 1.B
                break;
            case 'Accounts': // Install Step 2 Save Account info
                break;
            case 'Finalize':   // Install Step: finalize Add, set and cleanup last of the installation settings
                break;
        }
     //   if ( gotoNextStep === true ) {
      //   this.activeIndex++;
            this.setNextStatus();
      //  }
    }

    onClickBackStep() {
        if (this.activeIndex === 0) {
            return;
        }
        this.activeIndex--;

        this.setNextStatus();
    }

    setNextStatus() {
        switch (this.activeIndex) {

            case 0:
                this.enableNext = this.step1Valid;
                break;
            case 1:
                this.enableNext = this.step2Valid;
                break;
            case 2:
                this.enableNext = this.step3Valid;
                break;
        }
    }

    installApp() {
        this.processingRequest = true;
        this.msgBox.closeMessageBox();
        const res =  this._appService.installApp(this._appInfo).subscribe(response => {

            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 9999999);
                return false;
            }
            this.msgBox.ShowMessage('ok', 'Your site has finished installing, you will be redirected to the login.', 10    );
            setTimeout(() => {
                this._router.navigate(['/membership/login'], { relativeTo: this._route });
            }, 3000);

        }, err => {
            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 9999999);

          

        });

    //    //NOTE to user, do not share this account. it controls the entire
    //    //site. create a 'work' account for everyday transactions if you
    //    //require higher security.
    //    // if install fails re-enable so they can try again


    }


    checkInstallFlag() {

        const appRes = this._appService.getAppStatus();
        appRes.subscribe(response => {

                this.processingRequest = false;

                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }
                this._appStatus = response.Result;

            if ( this._appStatus === 'REQUIRES_INSTALL' ||  this._appStatus === 'INSTALLING'   ) {
                this._router.navigate(['/install'], { relativeTo: this._route });
            }
        }, err => {
            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {

                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });
    }
}
