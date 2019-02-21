import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from 'src/app/models/User';
import { Observable, Observer } from 'rxjs';
import { environment } from '../../environments/environment';



@Injectable({
  providedIn: 'root'
})
export class UsersGenerateService {
  test: Array<User> = [
    {
      'Email': 'ads',
      'Password': '123456',
      'StudentID': '23213'
    },
    {
      'Email': 'ads2',
      'Password': '1234586',
      'StudentID': '232193'
    }
  ];
  obs: Observable<User[]> = new Observable(observer => { observer.next(this.test); });
  constructor(
    private http: HttpClient,
    ) { }
    generateUsers(NewUsersId: string[]): Observable<User[]> {
      console.log({NewUsersId});
      console.warn(`TODO Add url to generate Users`);
      return this.http.post<User[]>(`${environment.baseUrl}/api/UserGenerate`, NewUsersId); // TODO Add url to generate Users
      // return this.obs;
  }
}
