import { Component, OnInit } from '@angular/core';
import { RegisterViewModel } from '../../models/ViewModels/RegisterViewModel';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnInit {

  model: RegisterViewModel = RegisterViewModel.Default();
  constructor() { }

  ngOnInit() {
  }
  onSubmit() {
    console.log('Registration with model ' + JSON.stringify(this.model));
  }
}
