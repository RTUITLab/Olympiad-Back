import { Component, OnInit, Input } from '@angular/core';
import { ExerciseService } from 'src/app/services/exercise.service';
import { ExerciseData } from 'src/app/models/ExerciseData';
import { ToastrService } from 'ngx-toastr';


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

  constructor(
    private exerciseService: ExerciseService,
    private toastr: ToastrService,
    ) { }

  ngOnInit() {
    this.exerciseService.getAllExerciseInOutData(this.exerciseId).subscribe(
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
        err => this.toastr.error(`Ошибка добавления`, err),
        () => this.toastr.success(`Условие успешно сохранено`)
      );
  }

  addCondition() {
    this.exerciseService.addExerciseInOutData(this.exerciseId, this.newCondition)
      .subscribe(o => {
        this.conditions.push(this.newCondition);
        this.newCondition = {};
        this.toastr.success(`Условие успешно добавлено`);
      },
      err => {
        this.toastr.error(`Ошибка добавления`);
      });
  }

}
