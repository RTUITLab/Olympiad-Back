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
  check(): boolean{
    // return this.http.get();
    return true;
  }
}
