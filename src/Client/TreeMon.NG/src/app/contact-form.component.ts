import { Component, ViewChild, Inject } from '@angular/core';
import {  FormBuilder, FormGroup, Validators } from '@angular/forms'; //

import { MessageBoxesComponent } from './common/messageboxes.component';
import { AppService } from './services/app.service';
import { BasicValidators } from './common/basicValidators';
import { Message } from './models/message';

@Component({

    templateUrl: './contact-form.component.html',
    providers: [AppService]
})

export class ContactFormComponent {
    form: FormGroup;
    sendingMessage = false;
    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;
    SentFrom: string;
    Subject: string;
    Comment: string;
    Type = 'contact_us';

    constructor(
        @Inject(FormBuilder) fb: FormBuilder,
        private _appService: AppService

    ) {

        this.form = fb.group({
            sentFrom: ['', Validators.compose([Validators.required, BasicValidators.email])],
            subject: ['', Validators.nullValidator],
            comment: ['', Validators.required],
            type: ['contactus', Validators.nullValidator]
        });

    }

    SendContactMessage() {
        this.sendingMessage = true;
       // this.form.controls['type'].setValue('contactus');  todo test that we don't need this
        const result = this._appService.sendMessage(this.form.value);
        result.subscribe(
            response => {

                this.sendingMessage = false;

                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }

                this.form.reset();
                this.form.markAsPristine();
                this.msgBox.ShowMessage('info', 'Your email has been sent. Thank you.', 10   );
            },
            err => {
                this.sendingMessage = false;
                this.msgBox.ShowResponseMessage(err.status, 10);


        });
    }
}

