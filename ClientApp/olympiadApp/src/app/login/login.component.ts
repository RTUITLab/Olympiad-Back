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

  onSubmit() {
    console.log('Some');
  }
  ngOnInit() {
  }
}
