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
import { FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { MatAutocompleteSelectedEvent } from '@angular/material';
import { CreateExerciseModel } from 'src/app/models/ViewModels/CreateExerciseModel';
import { Title } from '@angular/platform-browser';





@Component({
  selector: 'app-add-exercise',
  templateUrl: './add-exercise.component.html',
  styleUrls: ['./add-exercise.component.scss']
})
export class AddExerciseComponent extends LoadingComponent implements OnInit {

  //  variable for sending data to the server
  newExercise: CreateExerciseModel = new CreateExerciseModel();
  public challenges?: Array<Challenge> = [];
  public challengesNames?: Array<String> = [];
  filteredOptions: Observable<Challenge[]>;
  myControl = new FormControl();

  constructor(
    private exerciseEditServise: ExerciseEditService,
    private usersService: UserStateService,
    private challengesService: ChallengesService,
    private router: Router,
    private titleService: Title,
    private toastr: ToastrService
  ) {
    super();
  }
  async ngOnInit() {
    this.titleService.setTitle('Добавление задания');
    this.startLoading();
    this.stopLoading();
    this.challenges = await this.challengesService.getChallengesList();
    this.filteredOptions = this.myControl.valueChanges
    .pipe(
      startWith<string | Challenge>(''),
      map(value => typeof value === 'string' ? value : value.Name),
      map(name => name ? this._filter(name) : this.challenges.slice())
    );
  }

  displayFn(challenge?: Challenge): string | undefined {
    return challenge ? challenge.Name : undefined;
  }

  getSelectedOption(event: MatAutocompleteSelectedEvent) {
    const selectedChallenge = event.option.value as Challenge;
    this.newExercise.ChallengeId = selectedChallenge.Id;
  }

  private _filter(challenge: string): Challenge[] {
    const filterValue = challenge.toLowerCase();

    return this.challenges.filter(option => option.Name.toLowerCase().indexOf(filterValue) === 0);
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
