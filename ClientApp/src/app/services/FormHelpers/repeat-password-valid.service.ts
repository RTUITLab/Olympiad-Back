import { Injectable } from '@angular/core';
import { ValidatorFn, AbstractControl } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class RepeatPasswordValidService {

  constructor() { }
  RepeatPasswordValidator(): ValidatorFn {
    return (control: AbstractControl): {[key: string]: any} | null => {
      // const forbidden = nameRe.test(control.value);
      // return forbidden ? {'forbiddenName': {value: control.value}} : null;
      console.warn('TODO do RepeatPasswordValid');
      return null;
    };
  }
}
