// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { CanDeactivate } from '@angular/router';
import { FormGroup } from '@angular/forms';

export interface FormComponent {
    form: FormGroup;
}

export class PreventUnsavedChangesGuard implements CanDeactivate<FormComponent> {
    canDeactivate(component: FormComponent) {
        if (component.form.dirty) {
            return confirm('You have unsaved changes. Are you sure you want to navigate away?');
        }

        return true;
    }
}
