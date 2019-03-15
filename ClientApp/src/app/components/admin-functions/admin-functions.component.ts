import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserStateService } from 'src/app/services/user-state.service';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-admin-functions',
  templateUrl: './admin-functions.component.html',
  styleUrls: ['./admin-functions.component.scss']
})
export class AdminFunctionsComponent implements OnInit {

  constructor(private router: Router,
    private titleService: Title,
    private usersService: UserStateService
    ) { }

  ngOnInit() {
    this.titleService.setTitle('Функции администратора');
  }
  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
  addExercise() {
    this.router.navigate(['add-exercise']);
  }

  addChallenge() {
    this.router.navigate(['add-challenge']);
  }
  usersList() {
    this.router.navigate(['users-list']);
  }
  usersGeneration() {
    this.router.navigate(['users-generation']);
  }
}
