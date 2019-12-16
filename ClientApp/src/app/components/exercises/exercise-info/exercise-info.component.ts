import { Component, OnInit, OnDestroy, DoCheck, ChangeDetectorRef } from '@angular/core';
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
import { Title } from '@angular/platform-browser';
import { ShownResults } from '../../helpers/ShownResults';
import { switchMap } from 'rxjs/operators';



@Component({
  selector: 'app-exercise-info',
  templateUrl: './exercise-info.component.html',
  styleUrls: ['./exercise-info.component.scss']
})
export class ExerciseInfoComponent extends LoadingComponent implements OnInit, DoCheck, OnDestroy {

  private solutionCheckTimers: Array<any> = [];
  private challengeState: ChallengeState;

  constructor(
    private usersService: UserStateService,
    private exercisesService: ExerciseService,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService,
    private titleService: Title,
    private currentExerciseState: ExerciseStateService,
    private shownResultsService: ShownResults
  ) {
    super();
  }

  exerciseInfo: ExerciseInfo;
  solutionPreview?: string | ArrayBuffer;
  availableLanguages = LanguageConverter.languages();
  model: SolutionViewModel;

  get shownResults() { return this.shownResultsService.ShownResults; }

  get submitDisabled() {
    if (!this.model.File) {
      this.toastr.warning('Загрузите файл');
      return true;
    } else {
      if (this.model.Language === null) {
        this.toastr.warning('Выберите язык программирования');
        return true;
      } else {
        if (!this.model.File.name.endsWith(LanguageConverter.fileExtensionByPrettyName(this.model.Language))) {
          this.toastr.warning(`Расширение загружаемого вами файла не соответсвует выбранному языку программирования`);
          return true;
        } else {
          return false;
        }
      }
    }
  }
  get selectedLanguage() {
    if (this.model.Language) {
      return LanguageConverter.fileExtensionByPrettyName(this.model.Language);
    }
  }

  ngOnInit() {
    this.model = new SolutionViewModel();
    this.solutionPreview = null;
    this.model.Language = null;
    this.model.ExerciseId = this.route.snapshot.paramMap.get('ExerciseID');
    this.startLoading();
    this.exercisesService.getExercise(this.model.ExerciseId)
      .subscribe(
        (exInfo: ExerciseInfo) => {
          if (document.getElementById('Source') && document.getElementById('Source').files[0] != null) {
            document.getElementById('Source').files[0] = null;
          }
          this.exerciseInfo = exInfo;
          this.exerciseInfo.Solutions.sort((a, b) => new Date(a.SendingTime) < new Date(b.SendingTime) ? 1 : -1);
          this.titleService.setTitle(`${this.exerciseInfo.Name}`);
          // this.exerciseInfo
          //   .Solutions
          //   .filter(s => s.Status === SolutionStatus.InProcessing || s.Status === SolutionStatus.InQueue)
          //   .forEach(s => this.solutionCheckLoop(s));
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
  }
  async ngDoCheck() {
    if (this.model.ExerciseId !== this.route.snapshot.paramMap.get('ExerciseID')) {
      this.ngOnInit();
    }
  }
  ngOnDestroy(): void {
    this.solutionCheckTimers.forEach(t => clearTimeout(t));
  }
  selectLanguage(Language: string) {
    this.model.Language = Language;
  }
  setFile(event) {

    if (event.srcElement.files[0]) {
      if (this.model.Language === null) {
        this.toastr.warning('Выберите язык программирования');
      }
      this.model.File = event.srcElement.files[0];
      const fileReader = new FileReader();
      fileReader.onload = _ => {
        this.solutionPreview = fileReader.result;
      };
      fileReader.readAsText(this.model.File);
    }
  }
  onSubmit() {
    if (!this.submitDisabled) {
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
            } else {
              this.toastr.error('Не удалось отправить решение');
            }
          }
        );
    }
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
  async startRecheck(exerciseId: string) {
    const solutionsToRecheck = await this.exercisesService.recheckSolutions(exerciseId);
    this.toastr.success(`Будет перепроверено решений: ${solutionsToRecheck}`);
  }
}

