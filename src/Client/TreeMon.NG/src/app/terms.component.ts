import { Component, ViewChild } from '@angular/core';
import { AppService } from './services/app.service';
import { MessageBoxesComponent } from './common/messageboxes.component';

@Component({
    template: `
 <app-messageboxes></app-messageboxes>
<h2>{{appName }} Terms Of Service</h2>

<article>
<div  *ngIf="!loading" [innerHTML]="termsOfService"></div>
<div *ngIf="loading" style="width:100%; text-align: center;"> <i class="fa fa-spinner fa-spin fa-2x"></i> </div>
</article>
`,
    providers: [AppService]
})

export class TermsComponent {

    loading = false;
    pageSettings: any;
    appName: string;
    termsOfService: string;
    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(private _appService: AppService) {

        this.appName = '';
        const settingResult = this._appService.getDashboard('TermsOfService', '');
        this.loading = true;

        settingResult.subscribe(
            response => {
                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }
                const dashBoard = JSON.parse(response.Result);
                this.appName = dashBoard.Value;
                this.loadTermsOfService();
            },
            err => {
                this.loading = false;
                this.msgBox.ShowResponseMessage(err.status, 10);



            }
        );
    }

    loadTermsOfService() {
        this.loading = true;
        const replace = '[{  "Command" : "Replace" , "Arguments" : [{ "key": "[DOMAIN]" , "value": "' + this.appName + '" } ] } ]';

        const result = this._appService.getDashboard('TermsOfService', replace);
        result.subscribe(
            response => {
                this.loading = false;

                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }
                this.termsOfService = response.Result.Content[1].Value;
                this.loading = false;
            },
            err => {
                 this.loading = false;
                 this.msgBox.ShowResponseMessage(err.status, 10);



            }
        );
    }
}
