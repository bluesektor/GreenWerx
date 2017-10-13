// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import {
    Component
} from '@angular/core';

import { BasicValidators } from './basicValidators';

@Component({
    selector: 'app-messageboxes',
    templateUrl: './messageboxes.component.html'
})


export class MessageBoxesComponent  {

     showMessageBox = false;
     message: string;
     displayStyle: string;
     messageType: string;

    ShowMessage(msgType: string, message: string, displayTimeSeconds: number, style?: string) {
        if ( msgType && msgType !== null) {
            this.messageType = msgType.toLowerCase();
        }

        this.message = message;

        this.displayStyle = style;

        if (msgType === '' || message === '') {
            this.showMessageBox = false;
        } else {
            this.showMessageBox = true;
        }

        setTimeout(() => {
            this.showMessageBox = false;
        },  displayTimeSeconds * 1000);
    }

    ShowResponseMessage(code: number, displayTime, msg?: string) {
        const msgType = 'error';
        let text = msg;

        if (BasicValidators.isNullOrEmpty(msg) === true) {
            text = '';
        }

        switch (code) {
            case 401:
                text += ' Session expired.';
                break;
            case 429:
                text += 'Too many requests being sent.';
                break;
        }

        this.ShowMessage(msgType, text, displayTime);

    }


    closeMessageBox() {
        this.showMessageBox = false;
        this.message = '';
    }
}
