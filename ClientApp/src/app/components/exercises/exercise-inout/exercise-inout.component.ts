import { Component, OnInit, Input, Output } from '@angular/core';
import { ExerciseService } from '../../../services/exercise.service';
import { ExerciseData } from '../../../models/ExerciseData';

@Component({
  selector: 'app-exercise-inout',
  templateUrl: './exercise-inout.component.html',
  styleUrls: ['./exercise-inout.component.css']
})
export class ExerciseInoutComponent implements OnInit {
  @Input() exId: string;
  @Input() task_text_edit: boolean;
  constructor(
    private exerciseService: ExerciseService
  ) { }
  public exerciseDatas: ExerciseData[];
  ngOnInit() {
    console.log(this.exId+'inOut'+this.task_text_edit);
    this.exerciseService.getExerciseInOutData(this.exId)
      .subscribe(obj => {
        this.exerciseDatas = obj;
        console.log(obj);
      });
  }

}
