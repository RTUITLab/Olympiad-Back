import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Exercise } from 'src/app/models/Exercise';
import { TaskEditService } from 'src/app/services/task-edit.service';
import { UserStateService } from '../../../services/user-state.service';


@Component({
  selector: 'app-exercise-edit',
  templateUrl: './exercise-edit.component.html',
  styleUrls: ['./exercise-edit.component.scss']
})
export class ExerciseEditComponent implements OnInit {

  constructor(
    private taskEditServise: TaskEditService,
    private usersService: UserStateService,
    private router: Router, ) { }
  // variable for sending data to the server
  EditedTask: Exercise;

  // variables for task data
  task_name: string;
  task_text: string;
  task_score: number;
  // variables for task html elements
  edit_task_btn_doc: HTMLElement;
  task_name_doc: HTMLElement;
  task_score_doc: HTMLElement;
  task_text_doc: HTMLElement;
  // variable for task_text view
  task_text_edit: boolean;
  ngOnInit() {
         // set default value to variable for task_text view
         this.task_text_edit = false;
         //
         this.EditedTask = {};
  }
  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
  editTask() {
    // this.router.navigate(['edit-task']);
    console.log('EditTask()');
  // set variables value of documentID to interact with them
  this.edit_task_btn_doc = document.getElementById('editTask');
  this.task_name_doc = document.getElementById('task_name');
  this.task_score_doc = document.getElementById('task_score');
  this.task_text_doc = document.getElementById('task_text');

  if (this.task_name_doc.contentEditable === 'true') {
    // set default values for function for editing task content
    this.task_name_doc.contentEditable = 'false';
    this.task_score_doc.contentEditable = 'false';
    this.task_text_doc.contentEditable = 'false';
    // change button text
    this.edit_task_btn_doc.innerHTML = 'Изменить данные задания';
    this.edit_task_btn_doc.style.color = '#c300ff';
    this.edit_task_btn_doc.style.borderColor = '#c300ff';
    // change border of blocks with task data
    this.task_name_doc.style.border = 'unset';
    this.task_score_doc.style.border = 'unset';
    this.task_text_doc.style.border = 'unset';
    // get value from site to send them to the server
    this.task_name = this.task_name_doc.innerHTML.toString();
    this.task_score = parseInt(this.task_score_doc.innerHTML.toString(), 10);
    this.task_text = this.task_text_doc.innerHTML.toString();
    console.log(this.task_name);
    console.log(this.task_score);
    console.log(this.task_text);
    this.EditedTask.ExerciseName = this.task_name;
    this.EditedTask.ExerciseTask = this.task_text;
    this.EditedTask.Score = this.task_score;
    console.log(this.EditedTask);
    // is.task_score_doc.contentEditable = 'true';
    this.task_text_doc.contentEditable = 'true';
    // change button text
    this.edit_task_btn_doc.innerHTML = 'Сохранить';
    this.edit_task_btn_doc.style.color = '#16c210';
    this.edit_task_btn_doc.style.borderColor = '#16c210';

    // change border of blocks with task data
    this.task_name_doc.style.border = '1px solid #00eeffe7';
    this.task_score_doc.style.border = '1px solid #00eeffe7';
    this.task_text_doc.style.border = '1px solid #00eeffe7';

  }
  }
}
