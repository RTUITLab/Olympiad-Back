import { Component, OnInit } from '@angular/core';
import { UserStateService } from '../../../services/user-state.service';
import { ExerciseInfo } from 'src/app/models/Responses/ExerciseInfo';


@Component({
  selector: 'app-add-task',
  templateUrl: './add-task.component.html',
  styleUrls: ['./add-task.component.scss']
})
export class AddTaskComponent implements OnInit {

  exerciseInfo: ExerciseInfo = new ExerciseInfo();
  constructor(
    private usersService: UserStateService,
  ) { }
  ngOnInit() {
  }
  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
}
