import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { BaseHttpService } from './BaseHttpService';

@Injectable({
  providedIn: 'root'
})
export class AvailableRegistrationCheckService extends BaseHttpService {

  constructor(
    private http: HttpClient,
  ) { super(); }
  checkAvailableRegistration(): Observable<boolean> {
    return this.http.get<boolean>(
      `${this.baseUrl}/api/account/isRegisterAvailable`
    );
    return new BehaviorSubject(true).asObservable(); // TODO invoke get request to server
  }
}
