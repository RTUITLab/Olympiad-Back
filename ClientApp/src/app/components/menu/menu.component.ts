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
  constructor(
    private router: Router,
    private usersService: UserStateService) { }

  ngOnInit() {
    this.usersService.currentUserStream.subscribe(U => {
      this.user = U;
    });
  }
  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
  addExercise() {
    this.router.navigate(['add-exercise']);
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
