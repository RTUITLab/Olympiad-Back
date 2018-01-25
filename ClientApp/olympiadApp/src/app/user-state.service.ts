import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { User } from '../models/User';
import { LoginViewModel } from '../models/ViewModels/LoginViewModel';
import { RegisterViewModel } from '../models/ViewModels/RegisterViewModel';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { Subscriber } from 'rxjs/Subscriber';
@Injectable()
export class UserStateService {

  constructor(private http: HttpClient) { }

  private _logs = new BehaviorSubject<User>(undefined);
  public currentUser = this._logs.asObservable();

  public Login(model: LoginViewModel): Observable<string> {
    let observer: Subscriber<string>;
    const observable = new Observable<string>(obs => {
      observer = obs;
    });
    this.http.post('http://localhost:65086/api/auth/login', model, { responseType: 'text' })
      .subscribe(
      event => {
        this._logs.next(
          {
            FirstName: 'FirstName hah',
            StudentID: 'Student ID TEST',
            id: 'big guid',
            Token: 'big token',
            UserName: 'tester@test.com'
          }
        );
        observer.next(event);
      },
      error => observer.error('Неверные email/пароль')
      );
    return observable;
  }

  public Register(model: RegisterViewModel): Observable<string> {
    let observer: Subscriber<string>;
    const observable = new Observable<string>(obs => {
      observer = obs;
    });
    this.http.post('http://localhost:65086/api/account', model, { responseType: 'text' })
      .subscribe(
      event => observer.next(event),
      error => observer.error('Email занят')
      );
    return observable;
  }
}
