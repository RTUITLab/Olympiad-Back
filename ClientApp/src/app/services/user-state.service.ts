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
        this.InitUser(event);
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
  public GetMe(token: string): Observable<boolean> {
    let observer: Subscriber<boolean>;
    const observable = new Observable<boolean>(obs => {
      observer = obs;
    });
    this.http.get<LoginResponse>(`http://${this.ip}:${this.port}/api/auth/getme`,
      { headers: { 'Authorization': `Bearer ${token}` } })
      .subscribe(
        response => {
          this.InitUser(response);
          observer.next(true);
        },
        failure => {
          observer.next(false);
        }
      );
      return observable;
  }

  private InitUser(response: LoginResponse) {
    const user: User = {
      FirstName: response.FirstName,
      StudentID: response.StudentId,
      id: response.Id,
      Token: response.Token,
      Email: response.Email
    };
    localStorage.setItem('userToken', response.Token);
    this.usersBehavior.next(user);
    this.currentUser = user;
  }
  public authHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Authorization': `Bearer ${localStorage.getItem('userToken')}`
    });
  }
}
