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
  public SendEditedTask(EditedTask: Exercise) {
    console.log(`Task-EditService_SendEditedTask`);
    return this.http.put(`${environment.baseUrl}/api/Exercises/${EditedTask.ExerciseID}`, EditedTask);
  }
  public AddTask(NewTask: Exercise) {
    console.log(`Task-EditService_AddTask`);
     return this.http.post(`${environment.baseUrl}/api/Exercises/`, NewTask);
  }
}
