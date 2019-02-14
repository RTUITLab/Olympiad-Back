import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from '../models/User';
import { UserStateService } from '../services/user-state.service';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  user: User;

  constructor(private router: Router, private usersService: UserStateService) { }
  ngOnInit() {
    this.usersService.currentUserStream.subscribe(U => this.user = U);
  }
}
