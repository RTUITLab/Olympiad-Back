import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { Exercise } from 'src/app/models/Exercise';
import { TaskEditService } from 'src/app/services/task-edit.service';
import { UserStateService } from '../../../services/user-state.service';
import { LoadingComponent } from '../../helpers/loading-component';
import { ActivatedRoute } from '@angular/router';
import { SolutionViewModel } from '../../../models/ViewModels/SolutionViewModel';
import { ExerciseInfo } from '../../../models/Responses/ExerciseInfo';
import { ExerciseService } from '../../../services/exercise.service';
import { ParamMap } from '@angular/router/src/shared';



@Component({
  selector: 'app-exercise-edit',
  templateUrl: './exercise-edit.component.html',
  styleUrls: ['./exercise-edit.component.scss']
})
export class ExerciseEditComponent extends LoadingComponent  implements OnInit {

  constructor(
    private taskEditServise: TaskEditService,
    private usersService: UserStateService,
    private exercisesService: ExerciseService,
    private router: Router,
    private route: ActivatedRoute,
    ) {
      super();
     }
    exerciseInfo: ExerciseInfo;
    model: SolutionViewModel = new SolutionViewModel();

  //  variable for sending data to the server
  EditedTask: Exercise;

  //  variable for task_text view
  task_text_edit: boolean;
  ngOnInit() {
         this.EditedTask = {};
          // deny editing task data
         this.task_text_edit = false;
         this.getExercise();
  }
  //  get exercise from server through ExerciseID from path of page
  getExercise() {
      this.route.paramMap
      .subscribe((params: ParamMap) => {
        this.model.ExerciseId = params.get('ExerciseID');
        //  console.log(this.model.ExerciseId);
        this.startLoading();
        this.exercisesService.getExercise(this.model.ExerciseId)
          .subscribe(
          exInfo => {
            exInfo.Solutions = exInfo.Solutions.reverse();
            this.EditedTask.ExerciseID = exInfo.Id;
            this.EditedTask.ExerciseName = exInfo.Name;
            this.EditedTask.ExerciseTask = exInfo.TaskText;
            this.EditedTask.Score = exInfo.Score;
            this.stopLoading();
          },
          fail => {
            console.log(fail);
          }
          );
      });
    }
  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
  turnOnEditing() {
    console.log('turnOnEditing()');
    if (this.task_text_edit === false) {
      this.task_text_edit = true;
    }
  }
  turnOffEditing() {
    console.log('turnOffEditing()');
    if (this.task_text_edit === true) {
      this.task_text_edit = false;
    }
  }
  sendEditedTask() {
    console.log('sendEditedTask()');
    console.log(this.EditedTask);
    // send EditedTask to the server
   this.taskEditServise.SendEditedTask(this.EditedTask).subscribe(
     _ => {
       console.log(`sendEditedTask_complete`);
       this.router.navigate(['exercises/',this.model.ExerciseId]);
     },
     error => console.log(error),
   );
  }
}
