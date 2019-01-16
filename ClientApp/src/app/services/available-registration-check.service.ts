import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AvailableRegistrationCheckService  {

  constructor(
    private http: HttpClient,
  ) {
  }
  checkAvailableRegistration(): boolean {
    console.warn(`Using not implement service`);
    return true; // TODO invoke get request to server
  }
}
