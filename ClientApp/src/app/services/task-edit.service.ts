import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Exercise } from '../models/Exercise';
import { environment } from '../../environments/environment';
import { ExerciseInfo } from '../models/Responses/ExerciseInfo';
import { ExerciseData } from '../models/ExerciseData';

@Injectable({
  providedIn: 'root'
})
export class TaskEditService {
  
  constructor(
    private http: HttpClient,
  ) { }
  EditedTask: Exercise;
  public SendEditedTask(Task: ExerciseInfo) {
    console.log(`Task-EditService_SendEditedTask`);
    this.EditedTask = {
      ExerciseID: Task.Id,
      ExerciseName: Task.Name,
      ExerciseTask: Task.TaskText,
      Score: Task.Score
    }
    return this.http.put(`${environment.baseUrl}/api/Exercises/${EditedTask.ExerciseID}`, EditedTask);
  }
  public AddTask(NewTask: Exercise) {
    console.log(`Task-EditService_AddTask`);
     return this.http.post(`${environment.baseUrl}/api/Exercises/`, NewTask);
  }
  public SendEditedCondition(NewCondition: ExerciseData [], EditedTaskId: string){
    console.log(`Task-EditService_SendEditedCondition`);
    console.log(NewCondition);
    return this.http.post(`${environment.baseUrl}/api/ExerciseData/${EditedTaskId}`, NewCondition);
  }
}
