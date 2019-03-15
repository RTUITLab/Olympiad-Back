import { Component, OnInit } from '@angular/core';
import { RegisterViewModel } from '../../models/ViewModels/RegisterViewModel';
import { UserStateService } from '../../services/user-state.service';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { ToastrService } from 'ngx-toastr';
import { LoadingComponent } from '../helpers/loading-component';
import { AvailableRegistrationCheckService as AvailableReg } from '../../services/available-registration-check.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { repeat } from 'rxjs/operators';
import { FormValidateService } from 'src/app/services/FormHelpers/form-validate.service';
import { RepeatPasswordValidService } from 'src/app/services/FormHelpers/repeat-password-valid.service';
import { Title } from '@angular/platform-browser';


@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent extends LoadingComponent implements OnInit {
  constructor(
    private userService: UserStateService,
    private router: Router,
    private toastr: ToastrService,
    private regCheck: AvailableReg,
    private formValid: FormValidateService,
    private titleService: Title,
    private repeatPassValid: RepeatPasswordValidService
    ) {
      super();
     }
  regIsAvaliable = false;
  model: RegisterViewModel = RegisterViewModel.Default;
  registrationForm = new FormGroup({
    FirstName: new FormControl('', [
      Validators.required
    ]),
    LastName: new FormControl('', [
      Validators.required
    ]),
    Email: new FormControl('', [
      Validators.required,
      Validators.email
    ]),
    StudentId: new FormControl('', [
      Validators.required
    ]),
    Password: new FormControl('', [
      Validators.required,
      Validators.minLength(6),
    ]),
    RepeatPassword: new FormControl('', [
      Validators.required,
      Validators.minLength(6),
    ]),
    RecaptchaToken: new FormControl('', [
      Validators.required
    ])
  }, {validators: this.repeatPassValid.RepeatPasswordValidator}
  );
  get firstName() { return this.registrationForm.controls['FirstName']; }
  get lastName() { return this.registrationForm.controls['LastName']; }
  get email() { return this.registrationForm.controls['Email']; }
  get id() { return this.registrationForm.controls['StudentId']; }
  get password() { return this.registrationForm.controls['Password']; }
  get repeatPassword() { return this.registrationForm.controls['RepeatPassword']; }
  get recaptchaToken() { return this.registrationForm.controls['RecaptchaToken']; }
  ngOnInit() {
    this.titleService.setTitle('Регистрация');
    this.startLoading();
    this.regCheck.checkAvailableRegistration()
      .subscribe(s => {
        this.regIsAvaliable = s;
      });
      this.stopLoading();
  }
  onSubmit() {
    if (this.registrationForm.valid) {
      console.log(this.registrationForm);
      this.model  = this.registrationForm.value;
      if (!this.model.RecaptchaToken) {
        alert('Выполните проверку!');
        return;
      }
      this.userService.Register(this.model)
      .subscribe(
        success => {
          this.router.navigate(['exercises']);
          this.toastr.success(`Регистрация прошла успешно`);
        },
        error => {
          this.toastr.error(error, `Ошибка`);
        });
      } else {
        this.formValid.validateAllFormFields(this.registrationForm);
      }
  }

  public get captchaKey(): string {
    return environment.recaptchaClientToken;
  }
  public handleCorrectCaptcha(token: string) {
    this.model.RecaptchaToken = token;
  }
}
