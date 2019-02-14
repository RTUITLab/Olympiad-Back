import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { LoginViewModel } from '../../models/ViewModels/LoginViewModel';
import { UserStateService } from '../../services/user-state.service';
import { Router } from '@angular/router';
import { LoadingComponent } from '../helpers/loading-component';
import { ToastrService } from 'ngx-toastr';
import { FormGroup, FormControl, Validators } from '@angular/forms';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent extends LoadingComponent implements OnInit {
  errorMessage: string;
  model: LoginViewModel;
  loginForm = new FormGroup({
    Email: new FormControl('',[
      Validators.email,
      Validators.required
    ]),
    Password: new FormControl('',[
      Validators.required,
      Validators.minLength(6)
    ])
  });
  constructor(
    private userState: UserStateService,
     private router: Router,
     private toastr: ToastrService,
     ) {
    super();
  }

  onSubmit() {
    this.model = this.loginForm.value;
    console.log(this.model);
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
  }
  ngOnInit() {
  }
}
