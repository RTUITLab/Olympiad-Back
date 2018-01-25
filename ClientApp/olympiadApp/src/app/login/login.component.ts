import { Component, OnInit } from '@angular/core';
import { LoginViewModel } from '../../models/ViewModels/LoginViewModel';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor() { }
  model = LoginViewModel.Default();
  submitted = false;

  onSubmit() { this.submitted = true;
    console.log('Some');
  }
  ngOnInit() {
  }
  get diagnostic() { return JSON.stringify(this.model); }
}
