// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { FormControl, FormGroup } from '@angular/forms';

export class PasswordValidators {

    static complexPassword(control: FormControl) {
        const minLength = 5;

        if (!control) {
            return null;
        }

        // We bypass this validation rule if the field is empty, assuming
        // it is valid. At this point, the required validator will kick in
        // and asks the user to type a value. With this technique, we'll
        // display only a single validation message at a time, rather than:
        // * This field is required.
        // * Password doesn't match complexity rules.
        //
        if (control.value === '' || control.value == null) {
            return null;
        }

        // Note that I'm returning an object, instead of:
            // return { complexPassword: true }
            // This will give the client additional data about the
            // validation and why it failed.
        if (control.value.length < minLength) {
            return { complexPassword: {  minLength: minLength  }  };
        }
        return null;
    }

    static passwordsShouldMatch(group: FormGroup) {
        const ctlPassword = group.get('password');
        if (ctlPassword == null) {
            return null;
        }

        const newPassword = ctlPassword.value;

        const ctlConfirmPassword = group.get('confirmPassword');
        if (ctlConfirmPassword == null) {  return null; }

        const confirmPassword = ctlConfirmPassword.value;

        // If either of these fields is empty, the validation
        // will be bypassed. We expect the required validator to be
        // applied first.
        if (newPassword == null || newPassword === '' || confirmPassword == null || confirmPassword === '') {
            return null;
        }

        if (newPassword !== confirmPassword) {
            return { passwordsShouldMatch: true };
        }

        return null;
    }
}
