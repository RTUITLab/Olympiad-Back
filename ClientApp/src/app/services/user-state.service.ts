import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/';
import { User } from '../models/User';
import { LoginViewModel } from '../models/ViewModels/LoginViewModel';
import { RegisterViewModel } from '../models/ViewModels/RegisterViewModel';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/';
import { Subscriber } from 'rxjs/';
import { EndPoints } from './EndPoints';
import { LoginResponse } from '../models/Responses/LoginResponse';
import { environment } from '../../environments/environment';

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
    this.http.post<LoginResponse>(`${this.baseUrl}/api/auth/login`, model, { responseType: 'json' })
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
    this.http.post(`${this.baseUrl}/api/account`, model, { responseType: 'text' })
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
    this.http.get<LoginResponse>(`${this.baseUrl}/api/auth/getme`,
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


  public IsAdmin(): boolean {
    if (environment.replaceAdmin)
      return environment.isAdmin;
    return this.currentUser.Roles.indexOf('Admin') !== -1;
  }

  private InitUser(response: LoginResponse) {
    const user: User = {
      FirstName: response.FirstName,
      StudentID: response.StudentId,
      id: response.Id,
      Token: response.Token,
      Email: response.Email,
      Roles: this.parseJwt(response.Token)
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
  private parseJwt(token: string): string[] {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace('-', '+').replace('_', '/');
    return JSON.parse(window.atob(base64))['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
  }
}
