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
  NewExercise: Exercise;
  //  variable for exercise_text view
  exercise_text_edit: boolean;

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
    this.NewExercise = {};
    this.NewCondition = [];
    // deny editing exercise data
    this.exercise_text_edit = true;
    this.stopLoading();
  }
  turnOnEditing() {
    console.log('turnOnEditing()');
    if (this.exercise_text_edit === false) {
      this.exercise_text_edit = true;
    }
  }
  turnOffEditing() {
    console.log('turnOffEditing()');
    if (this.exercise_text_edit === true) {
      this.exercise_text_edit = false;
    }
  }
  AddExercise() {
    console.log('Addexercise()');
    console.log(this.NewExercise);
    this.ngAfterViewInit();
    // console.log(this.NewCondition);
    // send exercise to the server
    this.exerciseEditServise.AddExercise(this.NewExercise).subscribe(
      _ => {
        console.log(`sendEditedexercise_complete`);
      },
      error => console.log(error),
    );
  }
  sendNewCondition(NewExerciseId: string) {
    console.log('sendEditedCondition()');
    this.ngAfterViewInit();
    console.log(this.NewCondition);
    this.exerciseEditServise.SendNewCondition(this.NewCondition, NewExerciseId).subscribe(
      _ => {
        console.log(`sendEditedCondition_complete`);
        this.router.navigate(['exercises/', NewExerciseId]);
      },
      error => console.log(error),
    );

  }
  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
}
