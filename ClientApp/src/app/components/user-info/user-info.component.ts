import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from '../../models/User';
import { UserStateService } from '../../services/user-state.service';
import { AuthGuardService } from '../../services/auth-guard.service';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss']
})
export class UserInfoComponent implements OnInit {

  user: User;

  constructor(private router: Router, private usersService: UserStateService,
    private usersAuthService: AuthGuardService) { }
  ngOnInit() {
    this.usersService.currentUserStream.subscribe(U => this.user = U);
  }
  logout(){
    localStorage.removeItem('userToken'); //delete auth token to logout
    window.location.reload(); //reload page
    this.usersAuthService.canActivate();//check user's authorization

  }

}
