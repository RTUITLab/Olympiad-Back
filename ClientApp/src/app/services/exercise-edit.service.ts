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
  EditedExercise: Exercise;
  public SendEditedExercise(exercise: ExerciseInfo) {
    this.EditedExercise = {
      ExerciseName: exercise.Name,
      ExerciseText: exercise.ExerciseText,
      Score: exercise.Score
    };
    return this.http.put(`${environment.baseUrl}/api/Exercises/${exercise.Id}`, this.EditedExercise, this.userService.authOptions);
  }
  public AddExercise(NewExercise: Exercise) {
     return this.http.post(`${environment.baseUrl}/api/Exercises/`, NewExercise, this.userService.authOptions);
  }
  public SendEditedCondition(EditedCondition: ExerciseNewCondition [], EditedExerciseId: string) {
    return this.http.put(`${environment.baseUrl}/api/ExerciseData/${EditedExerciseId}`, EditedCondition, this.userService.authOptions);
  }
  public SendNewCondition(NewCondition: ExerciseNewCondition [], EditedExerciseId: string) {
    return this.http.post(`${environment.baseUrl}/api/ExerciseData/${EditedExerciseId}`, NewCondition, this.userService.authOptions);
  }
}
