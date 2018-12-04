import { Component, OnInit, Input, ViewChild, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { Exercise } from 'src/app/models/Exercise';
import { UserStateService } from '../../../services/user-state.service';
import { LoadingComponent } from '../../helpers/loading-component';
import { ActivatedRoute } from '@angular/router';
import { SolutionViewModel } from '../../../models/ViewModels/SolutionViewModel';
import { ExerciseInfo } from '../../../models/Responses/ExerciseInfo';
import { ExerciseService } from '../../../services/exercise.service';
import { ParamMap } from '@angular/router/src/shared';
import { ExerciseInoutComponent } from '../exercise-inout/exercise-inout.component';
import { ExerciseData } from '../../../models/ExerciseData';
import { ExerciseNewCondition } from '../../../models/ExerciseNewCondition';
import { ExerciseEditService } from 'src/app/services/exercise-edit.service';







@Component({
  selector: 'app-exercise-edit',
  templateUrl: './exercise-edit.component.html',
  styleUrls: ['./exercise-edit.component.scss']
})
export class ExerciseEditComponent extends LoadingComponent  implements OnInit, AfterViewInit {


  model: SolutionViewModel = new SolutionViewModel();

  //  variable for sending data to the server
  EditedTask: ExerciseInfo;

  //  variable for task_text view
  task_text_edit: boolean;


  constructor(
    private exerciseEditServise: ExerciseEditService,
    private usersService: UserStateService,
    private exercisesService: ExerciseService,
    private router: Router,
    private route: ActivatedRoute,
    ) {
      super();
    }
    // get InOutConditionData from conditions component
  @ViewChild(ExerciseInoutComponent) InOutConditionData;
  public EditedCondition: ExerciseNewCondition[];
  ngAfterViewInit() {
    this.EditedCondition = this.InOutConditionData.EditedCondition;
  }

  ngOnInit() {
         this.route.paramMap
      .subscribe((params: ParamMap) => {
        this.model.ExerciseId = params.get('ExerciseID');
        //  console.log(this.model.ExerciseId);
        this.startLoading();
        this.exercisesService.getExercise(this.model.ExerciseId)
          .subscribe(
          exInfo => {
            this.EditedTask = exInfo;
            console.log(this.EditedTask);
            this.stopLoading();
          },
          fail => {
            console.log(fail);
          }
          );
      });
          // deny editing task data
         this.task_text_edit = false;
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
    this.ngAfterViewInit();
    console.log(this.EditedCondition);
    // send EditedTask to the server
   this.exerciseEditServise.SendEditedTask(this.EditedTask).subscribe(
     _ => {
       this.sendEditedCondition(this.EditedTask.Id);
       console.log(`sendEditedTask_complete`);
     },
     error => console.log(error),
   );
  }
  sendEditedCondition(EditedTaskId: string){
    console.log('sendEditedCondition()');
    this.ngAfterViewInit();
    console.log(this.EditedCondition);
    this.exerciseEditServise.SendEditedCondition(this.EditedCondition, EditedTaskId).subscribe(
      _ => {
        console.log(`sendEditedCondition_complete`);
        this.router.navigate(['exercises/',this.model.ExerciseId]);
      },
      error => console.log(error),
    );

  }
}
