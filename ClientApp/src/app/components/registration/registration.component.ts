import { Component, OnInit } from '@angular/core';
import { RegisterViewModel } from '../../models/ViewModels/RegisterViewModel';
import { UserStateService } from '../../services/user-state.service';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent {
  loading = false;
  errorMessage: string;
  model: RegisterViewModel = RegisterViewModel.Default;
  constructor(private userService: UserStateService, private router: Router) { }

  onSubmit() {

    if (!this.model.RecaptchaToken) {
      alert('Выполните проверку!');
      return;
    }
    this.loading = true;
    this.errorMessage = null;
    this.userService.Register(this.model)
      .subscribe(
        success => {
          this.router.navigate(['exercises']);
          this.loading = false;
        },
        error => {
          this.errorMessage = error;
          this.loading = false;
        });
  }

  public get captchaKey(): string {
    return environment.recaptchaClientToken;
  }
  public handleCorrectCaptcha(token: string) {
    this.model.RecaptchaToken = token;
  }
}
