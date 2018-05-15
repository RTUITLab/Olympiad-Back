import { Component, OnInit } from '@angular/core';
import { RegisterViewModel } from '../../models/ViewModels/RegisterViewModel';
import { UserStateService } from '../../services/user-state.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnInit {
  loading = false;
  errorMessage: string;
  model: RegisterViewModel = new RegisterViewModel('', '', '', '');
  constructor(private userService: UserStateService, private router: Router) { }

  ngOnInit() {
  }
  onSubmit() {
    console.log('Registration with model ' + JSON.stringify(this.model));
    this.loading = true;
    this.errorMessage = undefined;
    this.userService.Register(this.model)
      .subscribe(
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
}
