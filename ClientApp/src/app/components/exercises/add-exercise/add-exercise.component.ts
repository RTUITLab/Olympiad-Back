import { Component, OnInit, Input, ViewChild, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { Exercise } from 'src/app/models/Exercise';
import { UserStateService } from '../../../services/user-state.service';
import { LoadingComponent } from '../../helpers/loading-component';
import { ActivatedRoute } from '@angular/router';
import { ExerciseInfo } from '../../../models/Responses/ExerciseInfo';
import { ExerciseService } from '../../../services/exercise.service';
import { ExerciseInoutComponent } from '../exercise-inout/exercise-inout.component';
import { ExerciseData } from '../../../models/ExerciseData';
import { ExerciseNewCondition } from '../../../models/ExerciseNewCondition';
import { ExerciseEditService } from 'src/app/services/exercise-edit.service';





@Component({
  selector: 'app-add-exercise',
  templateUrl: './add-exercise.component.html',
  styleUrls: ['./add-exercise.component.scss']
})
export class AddExerciseComponent extends LoadingComponent implements OnInit, AfterViewInit {

  exerciseInfo: ExerciseInfo = new ExerciseInfo();
  //  variable for sending data to the server
  NewTask: Exercise;
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
  @ViewChild(ExerciseNewCondition) InOutNewConditionData;
  public NewCondition: ExerciseNewCondition[];
  ngAfterViewInit() {
    this.NewCondition = [];
    console.log(`${this.InOutNewConditionData.NewCondition}`);
    this.NewCondition = this.InOutNewConditionData.NewCondition;
  }

  ngOnInit() {
    this.startLoading();
    this.NewTask = {};
    this.NewCondition = [];
    // deny editing task data
    this.task_text_edit = true;
    this.stopLoading();
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
  AddTask() {
    console.log('AddTask()');
    console.log(this.NewTask);
    this.ngAfterViewInit();
    // console.log(this.NewCondition);
    // send Task to the server
    this.exerciseEditServise.AddExercise(this.NewTask).subscribe(
      _ => {
        console.log(`sendEditedTask_complete`);
      },
      error => console.log(error),
    );
  }
  sendNewCondition(NewTaskId: string) {
    console.log('sendEditedCondition()');
    this.ngAfterViewInit();
    console.log(this.NewCondition);
    this.exerciseEditServise.SendNewCondition(this.NewCondition, NewTaskId).subscribe(
      _ => {
        console.log(`sendEditedCondition_complete`);
        this.router.navigate(['exercises/', NewTaskId]);
      },
      error => console.log(error),
    );

  }
  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
}
