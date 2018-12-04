import { Component, OnInit } from '@angular/core';
import { SolutionViewModel } from '../../../models/ViewModels/SolutionViewModel';
import { ExerciseService } from '../../../services/exercise.service';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/';
import { ParamMap } from '@angular/router/src/shared';
import { Solution } from '../../../models/Solution';
import { SolutionStatus } from '../../../models/SolutionStatus';
import { SolutionStatusConverter } from '../../../models/Common/SolutionStatusConverter';
import { LanguageConverter } from '../../../models/Common/LanguageConverter';
import { ExerciseInfo } from '../../../models/Responses/ExerciseInfo';
import { Subject } from 'rxjs';
import { LoadingComponent } from '../../helpers/loading-component';
import { UserStateService } from '../../../services/user-state.service';
import { Router } from '@angular/router';
import { Exercise } from 'src/app/models/Exercise';
import { ExerciseEditService } from 'src/app/services/exercise-edit.service';
// import { timingSafeEqual } from 'crypto';



@Component({
  selector: 'app-exercise-info',
  templateUrl: './exercise-info.component.html',
  styleUrls: ['./exercise-info.component.css']
})
export class ExerciseInfoComponent extends LoadingComponent implements OnInit {



  constructor(
    private usersService: UserStateService,
    private exercisesService: ExerciseService,
    private exerciseEditServise: ExerciseEditService,
    private route: ActivatedRoute,
    private router: Router, ) {
    super();
  }

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
        this.startLoading();
        this.exercisesService.getExercise(this.model.ExerciseId)
          .subscribe(
          exInfo => {
            exInfo.Solutions = exInfo.Solutions.reverse();
            this.exerciseInfo = exInfo;
            this.exerciseInfo
              .Solutions
              .filter(s => s.Status === SolutionStatus.InProcessing || s.Status === SolutionStatus.InQueue)
              .forEach(s => this.solutionCheckLoop(s.Id));
            // console.log(exInfo);
            this.stopLoading();
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
        const f = () => this.solutionCheckLoop(success);
        f();
      }
      );
  }
  editTask(id: string) {
    console.log(id);
    this.router.navigate(['exercises/edit/', id]);
  }
  solutionCheckLoop(solutionId: string) {
    // console.log(this);
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
          setTimeout(() => this.solutionCheckLoop(solutionId), 800);
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
  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
}

