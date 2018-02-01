import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Exercise } from '../models/Exercise';
import { HttpClient } from '@angular/common/http';
import { Subscriber } from 'rxjs/Subscriber';
import { error } from 'util';
import { EndPoints } from './EndPoints';

@Injectable()
export class ExerciseService extends EndPoints {

  constructor(private http: HttpClient) { super(); }

  getExercises(): Observable<Array<Exercise>> {
    let observer: Subscriber<Exercise>;
    const observable = new Observable<Array<Exercise>>(obs => {
      observer = obs;
    });

    this.http.get<Array<Exercise>>(`http://${this.ip}:${this.port}/api/exercises`).subscribe(
      success => {
        observer.next(success);
      },
      err => {
        observer.next([]);
        console.log(err);
      }
    );
    return observable;
  }

}
