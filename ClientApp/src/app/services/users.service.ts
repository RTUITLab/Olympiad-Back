import { Injectable } from '@angular/core';
import { BaseHttpService } from './BaseHttpService';
import { Observable } from 'rxjs';
import { ExerciseListResponse } from '../models/Responses/ExerciseListResponse';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserInfo } from '../models/Responses/UserInfo';
import { UserStateService } from './user-state.service';


@Injectable({
  providedIn: 'root'
})
export class UsersService extends BaseHttpService {

  constructor(private http: HttpClient, private userService: UserStateService) { super(); }

  getUsers(match: string = ''): Observable<Array<UserInfo>> {
    return this.http.get<Array<UserInfo>>(
      `${this.baseUrl}/api/account?match=${match}`,
      this.userService.authOptions
    );
  }
}
