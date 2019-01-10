import { Component, OnInit, Input } from '@angular/core';
import { ExerciseService } from 'src/app/services/exercise.service';
import { ExerciseData } from 'src/app/models/ExerciseData';

@Component({
  selector: 'app-condition-edit',
  templateUrl: './condition-edit.component.html',
  styleUrls: ['./condition-edit.component.css']
})
export class ConditionEditComponent implements OnInit {

  @Input()
  exerciseId: string;

  public conditions: Array<ExerciseData>;
  public newCondition: ExerciseData = {};

  constructor(private exerciseService: ExerciseService) { }

  ngOnInit() {
    this.exerciseService.getExerciseInOutData(this.exerciseId).subscribe(
      (conditions) => {
        if (!conditions) {
          return;
        }
        this.conditions = conditions;
      });
  }


  saveCondition(data: ExerciseData) {
    this.exerciseService.updateExerciseInOutData(data)
      .subscribe(
        n => console.log(n),
        err => console.log(err),
        () => console.log('complete')
      );
  }

  addCondition() {
    this.exerciseService.addExerciseInOutData(this.exerciseId, this.newCondition)
      .subscribe(o => {
        this.conditions.push(this.newCondition);
        this.newCondition = {};
      });
  }

}
