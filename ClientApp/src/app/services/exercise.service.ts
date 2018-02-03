import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Exercise } from '../models/Exercise';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Subscriber } from 'rxjs/Subscriber';
import { error } from 'util';
import { EndPoints } from './EndPoints';
import { UserStateService } from './user-state.service';

@Injectable()
export class ExerciseService extends EndPoints {

  constructor(private http: HttpClient, private userService: UserStateService) { super(); }

  getExercises(): Observable<Array<Exercise>> {
    let observer: Subscriber<Exercise>;
    const observable = new Observable<Array<Exercise>>(obs => {
      observer = obs;
    });
    const headers = new HttpHeaders ({
      'Authorization': `Bearer ${this.userService.currentUser.Token}`
    });
    this.http.get<Array<Exercise>>(`http://${this.ip}:${this.port}/api/exercises`, { headers: headers}).subscribe(
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
