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
import { ChallengesService } from 'src/app/services/challenges.service';
import { Challenge } from 'src/app/models/Responses/Challenges/Challenge';
import { ToastrService } from 'ngx-toastr';





@Component({
  selector: 'app-add-exercise',
  templateUrl: './add-exercise.component.html',
  styleUrls: ['./add-exercise.component.scss']
})
export class AddExerciseComponent extends LoadingComponent implements OnInit {

  //  variable for sending data to the server
  newExercise: Exercise = new Exercise();

  constructor(
    private exerciseEditServise: ExerciseEditService,
    private usersService: UserStateService,
    private challengesService: ChallengesService,
    private router: Router,
    private toastr: ToastrService
  ) {
    super();
  }
  public challenges?: Array<Challenge> = [];

  ngOnInit() {
    this.startLoading();
    this.stopLoading();
    this.challengesService.getChallengesList().subscribe(c => {
      if (!c) {
        return;
      }
      this.challenges = c;
    });
  }
  addExercise() {
    this.exerciseEditServise.AddExercise(this.newExercise).subscribe(
      ex => {
        this.toastr.success(`Задание добавлено успешно`);
        this.router.navigate(['exercises/edit', ex.Id]);
      },
      error => {
        this.toastr.error(error, `Ошибка добавления задания`);
      }
    );
  }

  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
}
