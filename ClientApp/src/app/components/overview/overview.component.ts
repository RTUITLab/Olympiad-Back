import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from '../../models/User';
import { UserStateService } from '../../services/user-state.service';
import { Title } from '@angular/platform-browser';


@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss']
})
export class OverviewComponent implements OnInit {

  user: User;

  constructor(
    private router: Router,
    private titleService: Title,
    private usersService: UserStateService
  ) { }
  ngOnInit() {
    this.titleService.setTitle('Общие положения');
    this.usersService.currentUserStream.subscribe(U => this.user = U);
  }

}
