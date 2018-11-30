import { Component, OnInit } from '@angular/core';
import { UserStateService } from '../../../services/user-state.service';


@Component({
  selector: 'app-add-task',
  templateUrl: './add-task.component.html',
  styleUrls: ['./add-task.component.scss']
})
export class AddTaskComponent implements OnInit {

  constructor(
    private usersService: UserStateService,
  ) { }

  ngOnInit() {
  }
  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
}
