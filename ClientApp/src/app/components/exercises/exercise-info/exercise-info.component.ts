import { Component, OnInit } from '@angular/core';
import { SolutionViewModel } from '../../../models/ViewModels/SolutionViewModel';
import { ExerciseService } from '../../../services/exercise.service';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { ParamMap } from '@angular/router/src/shared';
import { Solution } from '../../../models/Solution';
import { SolutionStatus } from '../../../models/SolutionStatus';
import { SolutionStatusConverter } from '../../../models/Common/SolutionStatusConverter';
import { LanguageConverter } from '../../../models/Common/LanguageConverter';
import { ExerciseInfo } from '../../../models/Responses/ExerciseInfo';
import { Subject } from 'rxjs/Subject';

@Component({
  selector: 'app-exercise-info',
  templateUrl: './exercise-info.component.html',
  styleUrls: ['./exercise-info.component.css']
})
export class ExerciseInfoComponent implements OnInit {

  constructor(private exercisesService: ExerciseService,
    private route: ActivatedRoute) { }

  exerciseInfo: ExerciseInfo;
  availableLanguages = LanguageConverter.languages();

  get submitDisabled() {
    return !this.model.File || !this.model.File.name.endsWith(LanguageConverter.fileExtension(this.model.Language));
  }
  model: SolutionViewModel = new SolutionViewModel();
  ngOnInit() {
    this.model.Language = 'Java';
    this.route.paramMap
      .subscribe((params: ParamMap) => {
        this.model.ExerciseId = params.get('ExerciseID');
        console.log(this.model.ExerciseId);
        this.exercisesService.getExercise(this.model.ExerciseId)
          .subscribe(
            exInfo => {
              this.exerciseInfo = exInfo;
              console.log(exInfo);
            },
            fail => {
              console.log(fail);
            }
          );
      });
  }
  setFile(event) {
    this.model.File = event.srcElement.files[0];
  }
  onSubmit() {
    this.exercisesService.sendSolution(this.model)
      .subscribe(
        success => {
          this.solutionCheckLoop(success);
        }
      );
  }
  solutionCheckLoop(solutionId: string) {
    this.exercisesService.checkSolution(solutionId).subscribe(
      solution => {
        const target = this.exerciseInfo.Solutions.find(s => s.Id === solution.Id);
        if (!target) {
          this.exerciseInfo.Solutions.unshift(solution);
        } else {
          target.Status = solution.Status;
        }
        if (solution.Status === SolutionStatus.InQueue ||
          solution.Status === SolutionStatus.InProcessing) {
          setTimeout(this.solutionCheckLoop, 800);
        }
      });
  }

  solutionStatusPresent(status: SolutionStatus): string {
    return SolutionStatusConverter.convertToPretty(status);
  }

  prettyTime(time: string): string {
    return Solution.prettyTime(time);
  }

  normalLang(lang: string): string {
    return LanguageConverter.normalFromWeb(lang);
  }
}
