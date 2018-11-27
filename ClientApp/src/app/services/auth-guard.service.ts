import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { UserStateService } from './user-state.service';

@Injectable()
export class AuthGuardService implements CanActivate {
  constructor(private router: Router, private userState: UserStateService) { }

  canActivate(): boolean {
    const token = localStorage.getItem('userToken');
    if (this.userState.currentUser) {
      this.userState.GetMe(token)
        .subscribe(success => {
          console.log('auth');
          console.log(success);
          if (!success) {
            this.router.navigate(['login']);
          }
        });
      return true;
    }
    if (token) {
      this.userState.GetMe(token)
        .subscribe(success => {
          console.log('auth');
          console.log(success);
          if (!success) {
            this.router.navigate(['login']);
          }
        });
      return true;
    }
    this.router.navigate(['login']);
    return false;
  }
}
