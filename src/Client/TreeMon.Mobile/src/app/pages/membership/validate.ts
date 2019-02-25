import { Component, ViewEncapsulation, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../services';
import {   ServiceResult } from '../../models/index';
import { Events } from '@ionic/angular';
import { Storage } from '@ionic/storage';

@Component({
  selector: 'page-validate',
  templateUrl: 'validate.html',
  styleUrls: ['./validate.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ValidatePage implements OnInit {
    validationType: string;
    operation: string;
    validationCode: string;
    validating = true;
    message = '';

  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    public router: Router,
    public messages: Events,
    public storage: Storage ) {
        console.log('validate.ts constructor *******************************************');
  }

  ngOnInit() {
    this.route.params.subscribe(params => {
        this.validationCode = params['code'];
        console.log('validate.ts constructor validationCode:', this.validationCode);
    });

    this.route.params.subscribe(params => {
        this.operation = params['operation'];
        console.log('validate.ts constructor operation:', this.operation);
    });

    this.route.params.subscribe(params => {
        this.validationType = params['type'];
        console.log('validate.ts constructor validationType:', this.validationType);
    });

    if (!this.validationCode || this.validationCode.length === 0) {
        this.message =  'Validation code is wrong!';
        this.messages.publish('api:err', this.message);
        this.validating = false;
        return;
    }

    if (!this.operation || this.operation.length === 0) {
        this.message = 'operation is wrong!';
        this.messages.publish('api:err', this.message);
        this.validating = false;
        return;
    }

    if (!this.validationType || this.validationType.length === 0) {
        this.message = 'operation type is wrong!';
        this.messages.publish('api:err', this.message);
        this.validating = false;
        return;
    }

    this.userService.validateUser( this.validationType, this.operation, this.validationCode ).subscribe(response => {
        this.validating = false;
        const data = response as ServiceResult;
        this.message = data.Message;
        if (data.Code !== 200) {
            this.messages.publish('api:err', this.message);
            return false;
        }
        this.messages.publish('api:ok', this.message);
        const typeOperation = this.validationType + '_' + this.operation;

        switch (typeOperation.toLocaleLowerCase()) {
                case 'mbr_mreg': // user validated email after registering.
                    this.message =  this.message + ' <br/> Account has been activated. You will be redirected to the login.';
                    setTimeout(() => {    this.router.navigateByUrl('/login'); }, 5000);
                break;
                case 'mbr_mdel': // membership oops/remove
                    this.message =  this.message + 'Your account has been deleted.';
                    break;
                case 'mbr_pwdr': // password reset
                        // let url = '/users/changepassword/operation/' + this.operation + '/code/' + this.validationCode;
                        // setTimeout(() => { this._router.navigate([url], { relativeTo: this._route }); }, 1000);
                break;
                default:                    // Invalid code.
                this.message =  this.message + 'Invalid code.';
        }

    }, err => {
        this.validating = false;
        this.messages.publish('service:err', err);
    });

    }
}
