import { Component, OnInit, OnDestroy } from '@angular/core';
import { SolutionViewModel } from '../../../models/ViewModels/SolutionViewModel';
import { ExerciseService } from '../../../services/exercise.service';
import { ActivatedRoute } from '@angular/router';
import { ParamMap } from '@angular/router/src/shared';
import { Solution } from '../../../models/Solution';
import { SolutionStatus } from '../../../models/SolutionStatus';
import { SolutionStatusConverter } from '../../../models/Common/SolutionStatusConverter';
import { LanguageConverter } from '../../../models/Common/LanguageConverter';
import { ExerciseInfo } from '../../../models/Responses/ExerciseInfo';
import { LoadingComponent } from '../../helpers/loading-component';
import { UserStateService } from '../../../services/user-state.service';
import { Router } from '@angular/router';
import { ExerciseEditService } from 'src/app/services/exercise-edit.service';
import { DateHelpers } from 'src/app/Helpers/DateHelpers';
import { ExerciseStateService } from 'src/app/services/exercise-state.service';
import { HttpErrorResponse } from '@angular/common/http';
import { ChallengeState } from 'src/app/models/General/ChallengeState';
import { SolutionHelpers } from 'src/app/Helpers/SolutionHelpers';
import { ToastrService } from 'ngx-toastr';



@Component({
  selector: 'app-exercise-info',
  templateUrl: './exercise-info.component.html',
  styleUrls: ['./exercise-info.component.scss']
})
export class ExerciseInfoComponent extends LoadingComponent implements OnInit, OnDestroy {

  private solutionCheckTimers: Array<any> = [];
  private challengeState: ChallengeState;
  constructor(
    private usersService: UserStateService,
    private exercisesService: ExerciseService,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService,
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
              this.currentExerciseState.setExerciseId(exInfo.Id);
            },
            () => {
              this.router.navigate(['overview']);
            }
          );
        this.currentExerciseState.currentChallengeState.subscribe(s => {
          this.challengeState = s;
        });
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
        createdSolution => {
          if (!createdSolution) {
            this.toastr.warning(`Решение не загружено`);
            return;
          }
          if (this.exerciseInfo.Solutions.some(s => s.Id === createdSolution.Id)) {
            this.toastr.warning(`Вы уже отправляли такое решение ${this.prettyTime(createdSolution.SendingTime)}`);
            return;
          }
          const f = () => this.solutionCheckLoop(createdSolution);
          this.toastr.success(`Решение успешно загружено`);
          f();
        },
        (error: HttpErrorResponse) => {
          if (error.status === 429) { // TooManyRequests HTTP Status
            this.toastr.warning(`Отправлять решения можно только раз в минуту`);
          }
          this.toastr.error('Не удалось отправить решение');
        }
      );
  }


  needSendForm(): boolean {
    return this.challengeState === ChallengeState.InProgress ||
      this.challengeState === ChallengeState.NoLimits;
  }


  editExercise(id: string) {
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
    return DateHelpers.prettyTime(time);
  }

  normalLang(lang: string): string {
    return LanguageConverter.normalFromWeb(lang);
  }
  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }

  downloadSolution(solution: Solution): void {
    this.exercisesService.downloadSolution(solution.Id).subscribe(s => {
      SolutionHelpers.downloadSolution(solution, s);
      this.toastr.success(`Загрузка начата`);

    }, fail => {
      // console.log(fail)
      this.toastr.error(`Невозможно скачать решение`);
    }
    );
  }
}

