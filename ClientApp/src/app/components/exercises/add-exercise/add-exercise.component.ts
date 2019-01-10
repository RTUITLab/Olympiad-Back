import { Component, OnInit } from '@angular/core';
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
import { ChallengeCompactResponse } from 'src/app/models/Responses/Challenges/ChallengeCompactResponse';
import { ChallengesService } from 'src/app/services/challenges.service';





@Component({
  selector: 'app-add-exercise',
  templateUrl: './add-exercise.component.html',
  styleUrls: ['./add-exercise.component.scss']
})
export class AddExerciseComponent extends LoadingComponent implements OnInit {

  exerciseInfo: ExerciseInfo = new ExerciseInfo();
  //  variable for sending data to the server
  NewExercise: Exercise;
  //  variable for exercise_text view
  exercise_text_edit: boolean;

  constructor(
    private exerciseEditServise: ExerciseEditService,
    private usersService: UserStateService,
    private exercisesService: ExerciseService,
    private challengesService: ChallengesService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    super();
  }
  public NewCondition?: ExerciseNewCondition[];
  public challenges?: Array<ChallengeCompactResponse> = [];

  ngOnInit() {
    this.startLoading();
    this.NewExercise = {};
    this.NewCondition = [];
    this.stopLoading();
    this.challengesService.getChallengesList().subscribe(c => {
      if (!c) {
        return;
      }
      this.challenges = c;
    });
  }
  turnOnEditing() {
    console.log('turnOnEditing()');
  }
  turnOffEditing() {
    console.log('turnOffEditing()');
  }
  AddExercise() {
    console.log('Addexercise()');
    console.log(this.NewExercise);
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
