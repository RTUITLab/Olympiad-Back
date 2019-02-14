import { Component, OnInit } from '@angular/core';
import { RegisterViewModel } from '../../models/ViewModels/RegisterViewModel';
import { UserStateService } from '../../services/user-state.service';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { ToastrService } from 'ngx-toastr';
import { LoadingComponent } from '../helpers/loading-component';
import { AvailableRegistrationCheckService as AvailableReg } from '../../services/available-registration-check.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent extends LoadingComponent implements OnInit {
  loading = false;
  errorMessage: string;
  regIsAvaliable = false;
  model: RegisterViewModel = RegisterViewModel.Default;
  registrationForm = new FormGroup({
    FirstName: new FormControl('', [
      Validators.required
    ]),
    LastName: new FormControl('', [
      Validators.required
    ]),
    SurName: new FormControl(''),
    Email: new FormControl('', [
      Validators.required,
      Validators.email
    ]),
    StudentId: new FormControl('', [
      Validators.required
    ]),
    Password: new FormControl('', [
      Validators.required,
      Validators.minLength(6)
    ]),
    RepeatPassword: new FormControl('', [
      Validators.required,
      Validators.minLength(6)
    ]),
    RecaptchaToken: new FormControl('', [
      Validators.required
    ])
  });
  constructor(
    private userService: UserStateService,
    private router: Router,
    private toastr: ToastrService,
    private regCheck: AvailableReg,

    ) {
      super();
     }
  ngOnInit() {
    this.startLoading();
    this.regCheck.checkAvailableRegistration()
      .subscribe(s => {
        this.regIsAvaliable = s;
      });
      this.stopLoading();
  }

  onSubmit() {
    console.log(this.registrationForm);
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
          this.toastr.success(`Регистрация прошла успешно`);
        },
        error => {
          this.errorMessage = error;
          this.loading = false;
          this.toastr.error(error, `Ошибка`);
        });
  }

  public get captchaKey(): string {
    return environment.recaptchaClientToken;
  }
  public handleCorrectCaptcha(token: string) {
    this.model.RecaptchaToken = token;
  }
}
