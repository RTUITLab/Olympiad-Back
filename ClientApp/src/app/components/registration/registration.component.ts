import { Component, OnInit } from '@angular/core';
import { RegisterViewModel } from '../../models/ViewModels/RegisterViewModel';
import { UserStateService } from '../../services/user-state.service';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { ToastrService } from 'ngx-toastr';
import { LoadingComponent } from '../helpers/loading-component';
import { AvailableRegistrationCheckService as AvailableReg }  from '../../services/ComponentActivators/available-registration-check.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent extends LoadingComponent implements OnInit {
  loading = false;
  errorMessage: string;
  regIsAvaliable?: boolean;
  model: RegisterViewModel = RegisterViewModel.Default;
  constructor(
    private userService: UserStateService,
    private router: Router,
    private toastr: ToastrService,
    private regCheck: AvailableReg,

    ) {
      super();
     }
  ngOnInit(){
    this.startLoading();
    this.regIsAvaliable=this.regCheck.check();
    this.stopLoading();
  }

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
