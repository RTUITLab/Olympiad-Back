import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { User } from '../models/User';
import { LoginViewModel } from '../models/ViewModels/LoginViewModel';
@Injectable()
export class UserStateService {

  constructor() { }

  private _logs = new BehaviorSubject<User>(undefined);
  public currentUser = this._logs.asObservable();

  public Login(model: LoginViewModel): void {
    console.error('NOT IMPLEMENT FUNCTION!');
  }
}
