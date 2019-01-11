import { Component, OnInit, OnDestroy } from '@angular/core';
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
import { Helpers } from 'src/app/Helpers/Helpers';
import { ExerciseStateService } from 'src/app/services/exercise-state.service';



@Component({
  selector: 'app-exercise-info',
  templateUrl: './exercise-info.component.html',
  styleUrls: ['./exercise-info.component.css']
})
export class ExerciseInfoComponent extends LoadingComponent implements OnInit, OnDestroy {

  private solutionCheckTimers: Array<any> = [];

  constructor(
    private usersService: UserStateService,
    private exercisesService: ExerciseService,
    private exerciseEditServise: ExerciseEditService,
    private route: ActivatedRoute,
    private router: Router,
    private currentExerciseState: ExerciseStateService) {
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
        this.startLoading();
        this.exercisesService.getExercise(this.model.ExerciseId)
          .subscribe(
            exInfo => {
              exInfo.Solutions = exInfo
                .Solutions
                .reverse();
              this.exerciseInfo = exInfo;
              this.exerciseInfo
                .Solutions
                .filter(s => s.Status === SolutionStatus.InProcessing || s.Status === SolutionStatus.InQueue)
                .forEach(s => this.solutionCheckLoop(s));
              this.stopLoading();
              this.currentExerciseState.setChallengeId(exInfo.ChallengeId);
            },
            fail => {
              console.log(fail);
            }
          );
      });
  }

  ngOnDestroy(): void {
    this.solutionCheckTimers.forEach(t => clearTimeout(t));
  }

  setFile(event) {
    this.model.File = event.srcElement.files[0];
  }
  onSubmit() {
    this.exercisesService.sendSolution(this.model)
      .subscribe(
        success => {
          if (!success) {
            return;
          }
          const f = () => this.solutionCheckLoop(success);
          f();
        }
      );
  }
  editExercise(id: string) {
    console.log(id);
    this.router.navigate(['exercises/edit/', id]);
  }
  solutionCheckLoop(checkSolution: Solution) {
    this.exercisesService.checkSolution(checkSolution.Id).subscribe(
      solution => {
        const target = this.exerciseInfo.Solutions.find(s => s.Id === solution.Id);
        if (!target) {
          this.exerciseInfo.Solutions.unshift(solution);
        } else {
          target.Status = solution.Status;
          target.StartCheckingTime = solution.StartCheckingTime;
          target.CheckedTime = solution.CheckedTime;
        }
        if (solution.Status === SolutionStatus.InQueue ||
          solution.Status === SolutionStatus.InProcessing) {
          this.solutionCheckTimers.push(
            setTimeout(() => this.solutionCheckLoop(checkSolution), 800)
          );
        }
      });
  }

  solutionStatusPresent(status: SolutionStatus): string {
    return SolutionStatusConverter.convertToPretty(status);
  }

  prettyTime(time: string): string {
    if (!time) {
      return 'нет данных';
    }
    return Helpers.prettyTime(time);
  }

  normalLang(lang: string): string {
    return LanguageConverter.normalFromWeb(lang);
  }
  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
}

