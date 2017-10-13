import { FormControl } from '@angular/forms';

export class BasicValidators {
    static email(control: FormControl ) {
        const regEx = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        const valid = regEx.test(control.value);
        return valid ? null : { email: true };
    }

    static isNullOrEmpty(value: string): boolean {
      if (value === null ||
          value === '' ||
          value === undefined ||
          value === 'undefined' ) {
            return true;
        }
        return false;
    }
}
