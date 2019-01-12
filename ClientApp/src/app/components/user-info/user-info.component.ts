import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from '../../models/User';
import { UserStateService } from '../../services/user-state.service';
import { AuthGuardService } from '../../services/ComponentActivators/auth-guard.service';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss']
})
export class UserInfoComponent implements OnInit {

  user: User;

  constructor(private router: Router, private usersService: UserStateService) { }
  ngOnInit() {
    this.usersService.currentUserStream.subscribe(U => this.user = U);
  }
  logout(){ 
    this.usersService.logOut();
    this.router.navigate(['/login']);
  }

}
