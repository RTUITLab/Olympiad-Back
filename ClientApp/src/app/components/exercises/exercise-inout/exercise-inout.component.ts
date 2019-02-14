import { Component, OnInit, Input} from '@angular/core';
import { ExerciseService } from '../../../services/exercise.service';
import { ExerciseData } from '../../../models/ExerciseData';

@Component({
  selector: 'app-exercise-inout',
  templateUrl: './exercise-inout.component.html',
  styleUrls: ['./exercise-inout.component.scss']
})
export class ExerciseInoutComponent implements OnInit {
  @Input() exId: string;
  constructor(
    private exerciseService: ExerciseService
  ) { }
  public exerciseDatas: ExerciseData[];
  ngOnInit() {
    this.exerciseService.getExerciseInOutData(this.exId)
      .subscribe(obj => {
        this.exerciseDatas = obj;
      });
  }

}
