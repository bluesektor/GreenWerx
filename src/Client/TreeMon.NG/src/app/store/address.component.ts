// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, OnInit, ViewChild, Input, Output, Inject, EventEmitter } from '@angular/core';
import { FormsModule, FormBuilder, FormGroup, Validators, FormControl, ReactiveFormsModule } from '@angular/forms';

import { SessionService } from '../services/session.service';
import { MessageBoxesComponent } from '../common/messageboxes.component';
import { AppService } from '../services/app.service';
import { Address } from '../models/address';
import { BasicValidators } from '../common/basicValidators';


@Component({
    selector: 'tm-address', // <tm-address [address]=shoppingCart?.Address></tm-address>
    templateUrl: './address.component.html',
    providers: [SessionService, AppService]

})
export class AddressComponent implements OnInit {

    @Input() required = false;
    @Input() title;
    @Input() address: Address = new Address();
    @Output() update = new EventEmitter<any>();


    validAddressContactName = false;
    validAddressStreetAddress = false;
    validAddressCity = false;
    validAddressState = false;
    validAddressCountry = false;
    validAddressPostalCode = false;

    formAddress: FormGroup;
    addressFormGroup: FormGroup = new FormGroup({});
    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(
        private _appService: AppService,
        private _sessionService: SessionService,
        @Inject(FormBuilder) fb: FormBuilder
        ) {
        this.msgBox = new MessageBoxesComponent();
        this.addressFormGroup.addControl('AddressContactName', new FormControl('', [this.ValidateEx.bind(this, [ 'AddressContactName'])]));
        this.addressFormGroup.addControl('AddressStreetAddress1', new FormControl('', [this.ValidateEx.bind(this, ['AddressStreetAddress1'] )]));
        this.addressFormGroup.addControl('AddressCountry', new FormControl('', [this.ValidateEx.bind(this, [ 'AddressCountry'])]));
        this.addressFormGroup.addControl('AddressCity', new FormControl('', [this.ValidateEx.bind(this, ['AddressCity'])]));
        this.addressFormGroup.addControl('AddressState', new FormControl('', [this.ValidateEx.bind(this, [ 'AddressState'])]));
        this.addressFormGroup.addControl('AddressPostalCode', new FormControl('', [this.ValidateEx.bind(this, ['AddressPostalCode'])]));
        this.formAddress = fb.group({
            AddressGroup: this.addressFormGroup

        });

    }

    ngOnInit() {
        this.address.isValid = this.isValidAddress();
        this.update.emit({
            addressIsValid: this.address.isValid
        });
    }

    public ValidateEx(args: any[]): { [key: string]: boolean } {

        if (!args || args === null || args.length === 0 || this.address == null) {

            return {};
        }
        let res = {};

        const fieldToValidate = args[0];

        switch (fieldToValidate) {
            case 'AddressContactName':
                this.validAddressContactName = true;
                if (BasicValidators.isNullOrEmpty(this.address.ContactName)) {
                    this.validAddressContactName = false;
                    res = { required: true };
                }
                break;
            case 'AddressStreetAddress1':
                this.validAddressStreetAddress = true;
                if (BasicValidators.isNullOrEmpty(this.address.StreetAddress1)) {
                    this.validAddressStreetAddress = false;
                    res = { required: true };
                }
                break;
            case 'AddressCity':
                this.validAddressCity = true;
                if (BasicValidators.isNullOrEmpty(this.address.City)) {
                    this.validAddressCity = false;
                    res = { required: true };
                }
                break;
            case 'AddressState':
                this.validAddressState = true;
                if (BasicValidators.isNullOrEmpty(this.address.State)) {
                    this.validAddressState = false;
                    res = { required: true };
                }
                break;
            case 'AddressCountry':
                this.validAddressCountry = true;
                if (BasicValidators.isNullOrEmpty(this.address.Country)) {
                    this.validAddressCountry = false;
                    res = { required: true };
                }
                break;
            case 'AddressPostalCode':
                this.validAddressPostalCode = true;
                if (BasicValidators.isNullOrEmpty(this.address.PostalCode)) {
                    this.validAddressPostalCode = false;
                    res = { required: true };
                }
                break;

        }
        this.address.isValid = this.isValidAddress();
        this.update.emit({
            addressIsValid: this.address.isValid
        });

        return res;
    }

    isValidAddress(): boolean {
        if (this.validAddressContactName &&
            this.validAddressStreetAddress &&
            this.validAddressCity &&
            this.validAddressState &&
            this.validAddressCountry &&
            this.validAddressPostalCode) {
            return true;
        }
        return false;
    }
}
