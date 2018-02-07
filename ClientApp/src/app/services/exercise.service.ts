import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Exercise } from '../models/Exercise';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Subscriber } from 'rxjs/Subscriber';
import { error } from 'util';
import { EndPoints } from './EndPoints';
import { UserStateService } from './user-state.service';
import { SolutionViewModel } from '../models/ViewModels/SolutionViewModel';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { OnInit } from '@angular/core/src/metadata/lifecycle_hooks';
import { LanguageConverter } from '../models/Common/LanguageConverter';
import { ExerciseListResponse } from '../models/Responses/ExerciseListResponse';
import { ExerciseInfo } from '../models/Responses/ExerciseInfo';

@Injectable()
export class ExerciseService extends EndPoints implements OnInit {

  constructor(private http: HttpClient, private userService: UserStateService) { super(); }
  private exercisesBehavior = new BehaviorSubject<Array<Exercise>>(undefined);
  public exercisesStream = this.exercisesBehavior.asObservable();

  currentExercises: Array<Exercise>;

  ngOnInit(): void {
    this.exercisesStream.subscribe(exs => {
      this.currentExercises = exs;
    });
  }
  getExercises(): Observable<Array<ExerciseListResponse>> {
    let observer: Subscriber<Exercise>;
    const observable = new Observable<Array<ExerciseListResponse>>(obs => {
      observer = obs;
    });
    this.http.get<Array<ExerciseListResponse>>(
      `http://${this.ip}:${this.port}/api/exercises`,
      { headers: this.userService.authHeaders() })
      .subscribe(
      success => {
        console.log(success);
        this.exercisesBehavior.next(success);
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
    let observer: Subscriber<string>;
    const observable = new Observable<string>(obs => {
      observer = obs;
    });

    const formData: FormData = new FormData();
    formData.append('file', data.File, data.File.name);

    this.http.post(
      `http://${this.ip}:${this.port}/api/check/${LanguageConverter.webName(data.Language)}/${data.ExerciseId}`,
      formData, { headers: this.userService.authHeaders() })
      .subscribe(
      success => {
        console.log(success);
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
      `http://${this.ip}:${this.port}/api/exercises/${exId}`,
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
}