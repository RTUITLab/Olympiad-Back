import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Exercise } from '../models/Exercise';
import { HttpClient } from '@angular/common/http';
import { Subscriber } from 'rxjs/Subscriber';
import { error } from 'util';

@Injectable()
export class ExerciseService {

  constructor(private http: HttpClient) { }

  getExercises(): Observable<Array<Exercise>> {
    let observer: Subscriber<Exercise>;
    const observable = new Observable<Array<Exercise>>(obs => {
      observer = obs;
    });

    this.http.get<Array<Exercise>>('http://localhost:65086/api/exercises').subscribe(
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
