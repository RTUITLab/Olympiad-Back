import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Exercise } from '../models/Exercise';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Subscriber } from 'rxjs';
import { error } from 'util';
import { BaseHttpService } from './BaseHttpService';
import { UserStateService } from './user-state.service';
import { SolutionViewModel } from '../models/ViewModels/SolutionViewModel';
import { BehaviorSubject } from 'rxjs';
import { OnInit } from '@angular/core/src/metadata/lifecycle_hooks';
import { LanguageConverter } from '../models/Common/LanguageConverter';
import { ExerciseListResponse } from '../models/Responses/ExerciseListResponse';
import { ExerciseInfo } from '../models/Responses/ExerciseInfo';
import { SolutionStatus } from '../models/SolutionStatus';
import { Solution } from '../models/Solution';
import { ExerciseData } from '../models/ExerciseData';

@Injectable()
export class ExerciseService extends BaseHttpService implements OnInit {

  constructor(private http: HttpClient, private userService: UserStateService) { super(); }
  private solutionsBehavior = new BehaviorSubject<Solution>(undefined);
  public solutionStream = this.solutionsBehavior.asObservable();

  ngOnInit(): void {
  }
  getExercises(challengeId: string): Promise<Array<ExerciseListResponse>> {
    return this.http.get<Array<ExerciseListResponse>>(
      `${this.baseUrl}/api/Exercises?challengeId=${challengeId}`, this.userService.authOptions).toPromise();
  }


  sendSolution(data: SolutionViewModel): Observable<any> {
    const formData: FormData = new FormData();
    formData.append('file', data.File, data.File.name);
    return this.http.post<Solution>(
      `${this.baseUrl}/api/check/${LanguageConverter.webName(data.Language)}/${data.ExerciseId}`,
      formData, this.userService.authOptions);
  }

  downloadSolution(solutionId: string): Observable<any> {
    return this.http.get(
      `${this.baseUrl}/api/Check/download/${solutionId}`,
      { headers: this.userService.headers, responseType: 'text'});
  }

  getExercise(exId: string): Observable<ExerciseInfo> {
    return this.http.get<ExerciseInfo>(
      `${this.baseUrl}/api/exercises/${exId}`, this.userService.authOptions);
  }

  checkSolution(solutionId: string): Observable<Solution> {
    let observer: Subscriber<Solution>;
    const observable = new Observable<Solution>(obs => {
      observer = obs;
    });
    this.http.get<Solution>(
      `${this.baseUrl}/api/check/${solutionId}`, this.userService.authOptions).subscribe(
        success => {
          if (success) {
            observer.next(success);
            this.solutionsBehavior.next(success);
          }
        },
        failure => {
          console.log('error');
          console.log(failure);
        }
      );
    return observable;
  }

  getExerciseInOutData(exerciseId: string): Observable<ExerciseData[]> {
    return this.http.get<ExerciseData[]>(
      `${this.baseUrl}/api/ExerciseData/${exerciseId}`, this.userService.authOptions);
  }

  getAllExerciseInOutData(exerciseId: string): Observable<ExerciseData[]> {
    return this.http.get<ExerciseData[]>(
      `${this.baseUrl}/api/ExerciseData/all/${exerciseId}`, this.userService.authOptions);
  }

  updateExerciseInOutData(inOutData: ExerciseData): Observable<any> {
    return this.http.put(
      `${this.baseUrl}/api/ExerciseData/${inOutData.Id}`, inOutData, this.userService.authOptions
    );
  }

  addExerciseInOutData(exerciseId: string, inOutData: ExerciseData): Observable<any> {
    return this.http.post(
      `${this.baseUrl}/api/ExerciseData/${exerciseId}`, inOutData, this.userService.authOptions
    );
  }
}
