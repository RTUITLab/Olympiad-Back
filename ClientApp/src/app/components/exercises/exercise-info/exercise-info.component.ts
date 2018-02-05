import { Component, OnInit } from '@angular/core';
import { SolutionViewModel } from '../../../models/ViewModels/SolutionViewModel';
import { ExerciseService } from '../../../services/exercise.service';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { ParamMap } from '@angular/router/src/shared';
import { Solution } from '../../../models/Solution';
import { SolutionStatus } from '../../../models/SolutionStatus';
import { SolutionStatusConverter } from '../../../models/Common/SolutionStatusConverter';

@Component({
  selector: 'app-exercise-info',
  templateUrl: './exercise-info.component.html',
  styleUrls: ['./exercise-info.component.css']
})
export class ExerciseInfoComponent implements OnInit {

  constructor(private exercisesService: ExerciseService,
    private route: ActivatedRoute) { }
  solutions: Solution[] = [
    {
      Language: 'Java',
      ExerciseId: 'lolId',
      Id: 'LolIf',
      Raw: 'Source',
      Status: SolutionStatus.InQueue
    }
  ];

  get submitDisabled() {
    return !this.model.File;
  }
  model: SolutionViewModel = new SolutionViewModel();
  ngOnInit() {
    this.model.Language = 'Java';
    this.route.paramMap
      .subscribe((params: ParamMap) => {
        this.model.ExerciseId = params.get('ExerciseID');
      });
  }
  setFile(event) {
    this.model.File = event.srcElement.files[0];
  }
  onSubmit() {
    this.exercisesService.sendSolution(this.model);
  }

  solutionStatusPresent(status: SolutionStatus): string {
    return SolutionStatusConverter.convert(status);
  }
}
