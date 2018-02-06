import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { User } from '../models/User';
import { LoginViewModel } from '../models/ViewModels/LoginViewModel';
import { RegisterViewModel } from '../models/ViewModels/RegisterViewModel';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { Subscriber } from 'rxjs/Subscriber';
import { EndPoints } from './EndPoints';
import { LoginResponse } from '../models/Responses/LoginResponse';
@Injectable()
export class UserStateService extends EndPoints {

  constructor(private http: HttpClient) { super(); }

  private usersBehavior = new BehaviorSubject<User>(undefined);
  public currentUserStream = this.usersBehavior.asObservable();
  public currentUser: User;


  public Login(model: LoginViewModel): Observable<LoginResponse> {
    let observer: Subscriber<LoginResponse>;
    const observable = new Observable<LoginResponse>(obs => {
      observer = obs;
    });
    this.http.post<LoginResponse>(`http://${this.ip}:${this.port}/api/auth/login`, model, { responseType: 'json' })
      .subscribe(
      event => {
        const user: User = {
          FirstName: event.FirstName,
          StudentID: event.StudentId,
          id: event.Id,
          Token: event.Token,
          Email: event.Email
        };
        // localStorage.setItem('userToken', event.auth_token);
        console.log(event);
        this.usersBehavior.next(user);
        this.currentUser = user;

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
    this.http.post(`http://${this.ip}:${this.port}/api/account`, model, { responseType: 'text' })
      .subscribe(
      event => observer.next(event),
      error => observer.error('Email занят')
      );
    return observable;
  }



  public authHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Authorization': `Bearer ${this.currentUser.Token}`
    });
  }
}
