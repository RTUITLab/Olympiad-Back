import { Component, OnInit } from '@angular/core';
import { User } from '../../models/User';
import { UserStateService } from '../../services/user-state.service';

@Component({
  selector: 'app-personal-page',
  templateUrl: './personal-page.component.html',
  styleUrls: ['./personal-page.component.scss']
})
export class PersonalPageComponent implements OnInit {

  user?: User;

  constructor(private usersService: UserStateService,
    ) { }
  ngOnInit() {
    this.usersService.currentUserStream.subscribe(U => this.user = U);
  }

}
