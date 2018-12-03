import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { Exercise } from 'src/app/models/Exercise';
import { UserStateService } from '../../../services/user-state.service';
import { LoadingComponent } from '../../helpers/loading-component';
import { ActivatedRoute } from '@angular/router';
import { ExerciseInfo } from '../../../models/Responses/ExerciseInfo';
import { TaskEditService } from 'src/app/services/task-edit.service';
import { ExerciseService } from '../../../services/exercise.service';




@Component({
  selector: 'app-add-task',
  templateUrl: './add-task.component.html',
  styleUrls: ['./add-task.component.scss']
})
export class AddTaskComponent extends LoadingComponent implements OnInit {

  exerciseInfo: ExerciseInfo = new ExerciseInfo();
  constructor(
    private taskEditServise: TaskEditService,
    private usersService: UserStateService,
    private exercisesService: ExerciseService,
    private router: Router,
    private route: ActivatedRoute,
    ) {
      super();
     }
    //  variable for sending data to the server
    NewTask: Exercise;
    //  variable for task_text view
  task_text_edit: boolean;
  ngOnInit() {
    this.startLoading();
    this.NewTask = {};
    // deny editing task data
    this.task_text_edit = true;
    this.stopLoading();
  }
  turnOnEditing() {
    console.log('turnOnEditing()');
    if (this.task_text_edit === false) {
      this.task_text_edit = true;
    }
  }
  turnOffEditing() {
    console.log('turnOffEditing()');
    if (this.task_text_edit === true) {
      this.task_text_edit = false;
    }
  }
  AddTask() {
    console.log('AddTask()');
    console.log(this.NewTask);
    // send Task to the server
   this.taskEditServise.AddTask(this.NewTask).subscribe(
     _ => {
       console.log(`sendEditedTask_complete`);
     },
     error => console.log(error),
   );
  }
  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
}
