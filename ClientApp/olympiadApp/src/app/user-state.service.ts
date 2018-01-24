import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { User } from '../models/User';
@Injectable()
export class UserStateService {

  constructor() { }

  private _logs = new BehaviorSubject<User>(undefined);
  public currentUser = this._logs.asObservable();

}
