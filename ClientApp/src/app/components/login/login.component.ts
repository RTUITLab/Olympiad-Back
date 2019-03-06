import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { LoginViewModel } from '../../models/ViewModels/LoginViewModel';
import { UserStateService } from '../../services/user-state.service';
import { Router } from '@angular/router';
import { LoadingComponent } from '../helpers/loading-component';
import { ToastrService } from 'ngx-toastr';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { FormValidateService } from 'src/app/services/FormHelpers/form-validate.service';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent extends LoadingComponent implements OnInit {
  constructor(
    private userState: UserStateService,
     private router: Router,
     private toastr: ToastrService,
     private formValid: FormValidateService
     ) {
    super();
  }
  errorMessage: string;
  model: LoginViewModel;
  loginForm = new FormGroup({
    Login: new FormControl('', [
      Validators.required
    ]),
    Password: new FormControl('', [
      Validators.required,
      Validators.minLength(6)
    ])
  });
  get login() { return this.loginForm.controls['Login']; }
  get password() { return this.loginForm.controls['Password']; }
  onSubmit() {
    if (this.loginForm.valid) {
      this.model = this.loginForm.value;
      this.startLoading();
      this.userState.Login(this.model).subscribe(
        success => {
          this.router.navigate(['overview']);
          this.stopLoading();

        },
        error => {
          this.errorMessage = error;
          this.stopLoading();
          this.toastr.error(error, `Ошибка`);

        }
      );
    } else {
      this.formValid.validateAllFormFields(this.loginForm);
    }
  }
  ngOnInit() {
  }
}
