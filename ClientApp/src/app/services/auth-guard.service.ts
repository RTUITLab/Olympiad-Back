import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { UserStateService } from './user-state.service';

@Injectable()
export class AuthGuardService implements CanActivate {
  constructor(private router: Router, private userState: UserStateService) { }

  canActivate(): boolean {
    if (this.userState.currentUser) {
      return true;
    }
    const token = localStorage.getItem('userToken');
    if (token) {
      this.userState.GetMe(token);
      return true;
    }
    this.router.navigate(['login']);
    return false;
  }


}
