import { Injectable } from '@angular/core';
import { ValidatorFn, AbstractControl, FormGroup } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class RepeatPasswordValidService {

  constructor() { }
  RepeatPasswordValidator(control: AbstractControl): { invalid: boolean } {
    if (control.get('Password').value !== control.get('RepeatPassword').value) {
      control.get('RepeatPassword').setErrors({notUnique: true});
      return {invalid: true};
    }
}
}
