import { Component, OnInit } from '@angular/core';
import { LoadingComponent } from '../../helpers/loading-component';
import { ExerciseService } from '../../../services/exercise.service';
import { Exercise } from '../../../models/Exercise';
import { error } from 'util';

@Component({
  selector: 'app-exercises-list',
  templateUrl: './exercises-list.component.html',
  styleUrls: ['./exercises-list.component.css']
})
export class ExercisesListComponent extends LoadingComponent implements OnInit {

  exercises: Array<Exercise> = [];

  constructor(private exersicesManager: ExerciseService) {
    super(true);
    this.stopLoading();
  }

  ngOnInit() {
  }

}
