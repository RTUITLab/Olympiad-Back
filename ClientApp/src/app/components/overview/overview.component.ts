import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from '../../models/User';
import { UserStateService } from '../../services/user-state.service';


@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.css']
})
export class OverviewComponent implements OnInit {

  user: User;

  constructor(private router: Router, private usersService: UserStateService) { }
  ngOnInit() {
    this.usersService.currentUserStream.subscribe(U => this.user = U);
  }

}
