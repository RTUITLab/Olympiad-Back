import { Component, OnInit } from '@angular/core';
import { LoginViewModel } from '../../models/ViewModels/LoginViewModel';
import { UserStateService } from '../../services/user-state.service';
import { Router } from '@angular/router';
import { LoadingComponent } from '../helpers/loading-component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent extends LoadingComponent implements OnInit {
  errorMessage: string;
  constructor(private userState: UserStateService, private router: Router) {
    super();
  }
  model = new LoginViewModel('', '');

  onSubmit() {
    this.startLoading();
    this.userState.Login(this.model).subscribe(
      success => {
        this.router.navigate(['exercises']);
        this.stopLoading();
      },
      error => {
        this.errorMessage = error;
        this.stopLoading();
      }
    );
  }
  ngOnInit() {
  }
}
