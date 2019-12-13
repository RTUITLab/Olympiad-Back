import { Component, OnInit, Input, OnChanges, SimpleChanges} from '@angular/core';
import { ExerciseService } from '../../../services/exercise.service';
import { ExerciseData } from '../../../models/ExerciseData';

@Component({
  selector: 'app-exercise-inout',
  templateUrl: './exercise-inout.component.html',
  styleUrls: ['./exercise-inout.component.scss']
})
export class ExerciseInoutComponent implements OnInit, OnChanges {
  @Input() exId: string;
  constructor(
    private exerciseService: ExerciseService
    ) { }
    public exerciseDatas: ExerciseData[];
  ngOnChanges(changes: SimpleChanges): void {
    this.exerciseService.getExerciseInOutData(changes['exId'].currentValue)
    .subscribe(obj => {
      this.exerciseDatas = obj;
    });
  }
  ngOnInit() {
  }
  

  rawHTML(input: string): string {
    const l = input.replace(' ', '&#160;');
    return l;
  }


}
