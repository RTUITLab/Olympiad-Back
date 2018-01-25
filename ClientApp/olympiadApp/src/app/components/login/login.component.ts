import { Component, OnInit } from '@angular/core';
import { LoginViewModel } from '../../models/ViewModels/LoginViewModel';
import { UserStateService } from '../../services/user-state.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  errorMessage: string;
  loading = false;
  constructor(private userState: UserStateService, private router: Router) { }
  model = new LoginViewModel('tester@test.com', '123456');

  onSubmit() {
    this.loading = true;
    this.userState.Login(this.model).subscribe(
      success => {
        this.router.navigate(['exercises']);
        this.loading = false;
      },
      error => {
        this.errorMessage = error;
        this.loading = false;
      }
    );
  }
  ngOnInit() {
  }
}
