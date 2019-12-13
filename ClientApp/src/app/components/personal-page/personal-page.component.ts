import { Component, OnInit } from '@angular/core';
import { User } from '../../models/User';
import { UserStateService } from '../../services/user-state.service';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-personal-page',
  templateUrl: './personal-page.component.html',
  styleUrls: ['./personal-page.component.scss']
})
export class PersonalPageComponent implements OnInit {

  user?: User;

  constructor(private usersService: UserStateService,
    private titleService: Title,

    ) { }
  ngOnInit() {
    this.titleService.setTitle('Моя страница');
    this.usersService.currentUserStream.subscribe(U => this.user = U);
  }

}
