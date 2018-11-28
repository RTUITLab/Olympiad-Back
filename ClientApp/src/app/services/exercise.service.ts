import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Exercise } from '../models/Exercise';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Subscriber } from 'rxjs';
import { error } from 'util';
import { EndPoints } from './EndPoints';
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
export class ExerciseService extends EndPoints implements OnInit {

  constructor(private http: HttpClient, private userService: UserStateService) { super(); }
  private solutionsBehavior = new BehaviorSubject<Solution>(undefined);
  public solutionStream = this.solutionsBehavior.asObservable();

  ngOnInit(): void {
  }
  getExercises(): Observable<Array<ExerciseListResponse>> {
    let observer = new Subscriber<Array<ExerciseListResponse>>(undefined);
    const observable = new Observable<Array<ExerciseListResponse>>(obs => {
      observer = obs;
    });
    this.http.get<Array<ExerciseListResponse>>(
      `${this.baseUrl}/api/exercises`,
      { headers: this.userService.authHeaders() })
      .subscribe(
      success => {
        console.log(success);
        observer.next(success);
      },
      err => {
        observer.next([]);
        console.log(err);
      }
      );
    return observable;
  }


  sendSolution(data: SolutionViewModel): Observable<string> {
    const observer = new BehaviorSubject<string>(undefined);
    const observable = observer.asObservable();

    const formData: FormData = new FormData();
    formData.append('file', data.File, data.File.name);
    this.http.post(
      `${this.baseUrl}/api/check/${LanguageConverter.webName(data.Language)}/${data.ExerciseId}`,
      formData, { headers: this.userService.authHeaders(), responseType: 'text' })
      .subscribe(
      success => {
        console.log(success);
        observer.next(success);
        console.log('sended');
      },
      fail => {
        console.log(fail);
        console.log('failed');
      }
      );


    return observable;
  }
  getExercise(exId: string): Observable<ExerciseInfo> {
    let observer: Subscriber<ExerciseInfo>;
    const observable = new Observable<ExerciseInfo>(obs => {
      observer = obs;
    });
    this.http.get<ExerciseInfo>(
      `${this.baseUrl}/api/exercises/${exId}`,
      { headers: this.userService.authHeaders() })
      .subscribe(
      success => {
        console.log(success);
        observer.next(success);
      },
      err => {
        observer.next(undefined);
        console.log(err);
      });
    return observable;
  }

  checkSolution(solutionId: string): Observable<Solution> {
    let observer: Subscriber<Solution>;
    const observable = new Observable<Solution>(obs => {
      observer = obs;
    });
    this.http.get<Solution>(
      `${this.baseUrl}/api/check/${solutionId}`,
      { headers: this.userService.authHeaders() }).subscribe(
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
    let observer: Subscriber<ExerciseData[]>;
    const observable = new Observable<ExerciseData[]>(obs => {
      observer = obs;
    });
    this.http.get<ExerciseData[]>(
      `${this.baseUrl}/api/ExerciseData/${exerciseId}`,
      { headers: this.userService.authHeaders() }).subscribe(
      success => {
        if (success) {
          observer.next(success);
        }
      },
      failure => {
        console.log('error');
        console.log(failure);
      }
      );
    return observable;
  }
}
