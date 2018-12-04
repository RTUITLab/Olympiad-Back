import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Exercise } from '../models/Exercise';
import { environment } from '../../environments/environment';
import { ExerciseInfo } from '../models/Responses/ExerciseInfo';
import { ExerciseNewCondition } from '../models/ExerciseNewCondition';
import { UserStateService } from './user-state.service';

@Injectable({
  providedIn: 'root'
})
export class ExerciseEditService {

  constructor(
    private http: HttpClient,
    private userService: UserStateService
  ) { }
  EditedTask: Exercise;
  public SendEditedTask(Task: ExerciseInfo) {
    console.log(`Task-EditService_SendEditedTask`);
    this.EditedTask = {
      ExerciseName: Task.Name,
      ExerciseTask: Task.TaskText,
      Score: Task.Score
    };
    return this.http.put(`${environment.baseUrl}/api/Exercises/${Task.Id}`, this.EditedTask, this.userService.authOptions);
  }
  public AddExercise(NewTask: Exercise) {
    console.log(`Task-EditService_AddTask`);
     return this.http.post(`${environment.baseUrl}/api/Exercises/`, NewTask, this.userService.authOptions);
  }
  public SendEditedCondition(EditedCondition: ExerciseNewCondition [], EditedTaskId: string) {
    console.log(`Task-EditService_SendEditedCondition`);
    console.log(EditedCondition);
    return this.http.put(`${environment.baseUrl}/api/ExerciseData/${EditedTaskId}`, EditedCondition, this.userService.authOptions);
  }
  public SendNewCondition(NewCondition: ExerciseNewCondition [], EditedTaskId: string) {
    console.log(`Task-EditService_SendNewCondition`);
    console.log(NewCondition);
    return this.http.post(`${environment.baseUrl}/api/ExerciseData/${EditedTaskId}`, NewCondition, this.userService.authOptions);
  }
}
