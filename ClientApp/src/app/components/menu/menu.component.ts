import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserStateService } from '../../services/user-state.service';
import { User } from '../../models/User';
import { ExerciseService } from '../../services/exercise.service';
import { Exercise } from '../../models/Exercise';
import { ExercisesListComponent } from '../exercises/exercises-list/exercises-list.component';
import { ExerciseListResponse } from '../../models/Responses/ExerciseListResponse';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {

  user: User;
  exercises: ExerciseListResponse[] = [];
  constructor(
    private router: Router,
    private usersService: UserStateService,
    private exercisesService: ExerciseService) { }

  ngOnInit() {
    this.usersService.currentUserStream.subscribe(U => {
      this.user = U;
      this.exercisesService.getExercises()
        .subscribe(exercises => this.exercises = exercises || []);
    });
    this.exercisesService.solutionStream.subscribe(S => {
      if (!S) {
        return;
      }
      const targetEx = this.exercises.find(E => E.Id === S.ExerciseId);
      // console.log('target ex');
      // console.log(targetEx);
      // console.log(S);
      if (targetEx) {
        if (targetEx.Status < S.Status) {
          targetEx.Status = S.Status;
        }
      }
    });
  }
  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
  addExercise() {
    this.router.navigate(['add-exercise']);
  }
  needMenu(): boolean {
    // if (!this.exercises) {
      return true;
    // }
    // return this.exercises.length > 0;
  }
  exCount(): number {
    if (!this.exercises) {
      return 0;
    }
    return this.exercises.length;
  }
  login() {
    this.router.navigate(['login']);
  }

  register() {
    this.router.navigate(['register']);
  }

  overView() {
    this.router.navigate(['overview']);
  }

  goToExercise(id: string) {
    this.router.navigate(['exercises', id]);
  }
}
