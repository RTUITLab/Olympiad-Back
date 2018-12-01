import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Exercise } from '../models/Exercise';
import { environment } from '../../environments/environment';
@Injectable({
  providedIn: 'root'
})
export class TaskEditService {

  constructor(
    private http: HttpClient,
  ) { }
  public SendEditedTask(Task: Exercise) {
    return this.http.post(`${environment.baseUrl}/api/Exercises/${Task}`, Task);
  }
}
