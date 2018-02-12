import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserStateService } from '../../services/user-state.service';
import { User } from '../../models/User';
import { ExerciseService } from '../../services/exercise.service';
import { Exercise } from '../../models/Exercise';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {

  user: User;
  exercises: Array<Exercise>;
  constructor(
    private router: Router,
    private usersService: UserStateService,
    private exercisesService: ExerciseService) { }

  ngOnInit() {
    this.usersService.currentUserStream.subscribe(U => this.user = U);
    this.exercisesService.getExercises();
    this.exercisesService.exercisesStream.subscribe(exercises => this.exercises = exercises);
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
