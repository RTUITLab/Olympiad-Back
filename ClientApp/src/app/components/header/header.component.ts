import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from '../../models/User';
import { UserStateService } from '../../services/user-state.service';
import { AuthGuardService } from '../../services/auth-guard.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

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
