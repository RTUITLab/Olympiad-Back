import { Injectable, OnInit } from '@angular/core';
import { BaseHttpService } from './BaseHttpService';
import { UserStateService } from './user-state.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Challenge } from '../models/Responses/Challenges/Challenge';
import { ChallengeEditViewModel } from '../models/ViewModels/ChallengeEditViewModel';

@Injectable({
  providedIn: 'root'
})
export class ChallengesService  extends BaseHttpService implements OnInit  {
  ngOnInit(): void {
  }

  constructor(private http: HttpClient, private userService: UserStateService) { super(); }

  public getChallengesList(): Observable<Array<Challenge>> {
    return this.http.get<Array<Challenge>>(
      `${this.baseUrl}/api/challenges`, this.userService.authOptions);
  }
  public getChallenge(id: string): Observable<Challenge> {
    return this.http.get<Challenge>(
      `${this.baseUrl}/api/challenges/${id}`, this.userService.authOptions);
  }

  public createChallenge(challenge: Challenge): Observable<Challenge> {
    return this.http.post<Challenge>(
      `${this.baseUrl}/api/challenges/`, challenge, this.userService.authOptions);
  }

  editChallenge(id: string, challenge: ChallengeEditViewModel): Observable<Challenge> { // TODO Use specific model
    return this.http.put<Challenge>(
      `${this.baseUrl}/api/challenges/${id}`, challenge, this.userService.authOptions);
  }
}
