import { Component, ViewChild, Inject, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { UserService } from '../../services/user.service';
import { SessionService } from '../../services/session.service';

import { MessageBoxesComponent } from '../../common/messageboxes.component';
import { CookieService } from 'angular2-cookie/services/cookies.service';

@Component({

    templateUrl: './users-validate.component.html',
    providers: [CookieService, UserService, SessionService]
})

export class UsersValidateComponent implements OnInit {
    validationType: string;
    operation: string;
    validationCode: string;
    validating = true;


    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(
        private _userService: UserService,
        private _router: Router,
        private _route: ActivatedRoute,
        private _cookieService: CookieService,
        private _sessionService: SessionService
    ) {     }

    ngOnInit() {
        this._route.params.subscribe(params => {
            this.validationCode = params['code'];
        });

        this._route.params.subscribe(params => {
            this.operation = params['operation'];
        });

        this._route.params.subscribe(params => {
            this.validationType = params['type'];
        });

        if (!this.validationCode || this.validationCode.length === 0) {

            this.msgBox.ShowMessage('error', 'Validation code is wrong!', 10);
            this.validating = false;
            return;
        }

        if (!this.operation || this.operation.length === 0) {
            this.msgBox.ShowMessage('error', 'operation is wrong!', 10);
            this.validating = false;
            return;
        }

        if (!this.validationType || this.validationType.length === 0) {
            this.msgBox.ShowMessage('error', 'operation type is wrong!', 10);
            this.validating = false;
            return;
        }

        this._userService.validateUser( this.validationType, this.operation, this.validationCode ).subscribe(response => {
            this.validating = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 20);
                return false;
            }
            this.msgBox.ShowMessage(response.Status, response.Message, 10);

            let typeOperation = this.validationType + '_' + this.operation;

            switch (typeOperation.toLocaleLowerCase()) {
                    case 'mbr_mreg': // user validated email after registering.
                        this.msgBox.ShowMessage('info', 'Account has been activated. You will be redirected to the login.', 10);
                        setTimeout(() => { this._router.navigate(['/membership/login'], { relativeTo: this._route }); }, 5000);
                    break;
                    case 'mbr_mdel': // membership oops/remove
                        this.msgBox.ShowMessage('info', 'Your account has been deleted.', 10);
                        break;
                    case 'mbr_pwdr': // password reset
                        let url = '/users/changepassword/operation/' + this.operation + '/code/' + this.validationCode;
                        setTimeout(() => { this._router.navigate([url], { relativeTo: this._route }); }, 1000);
                    break;
                    default:
                        // Invalid code.
                        this.msgBox.ShowMessage('info', 'Invalid code.', 60);
                }

        }, err => {
            this.validating = false;
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
