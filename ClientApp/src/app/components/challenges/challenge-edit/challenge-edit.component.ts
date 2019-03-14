import { Component, OnInit } from '@angular/core';
import { UserStateService } from 'src/app/services/user-state.service';
import { Challenge } from 'src/app/models/Responses/Challenges/Challenge';
import { ToastrService } from 'ngx-toastr';
import { ChallengeAccessType } from 'src/app/models/General/ChallengeAccessType';
import { LoadingComponent } from '../../helpers/loading-component';
import { ChallengesService } from 'src/app/services/challenges.service';
import { ParamMap, ActivatedRoute } from '@angular/router';
import { ExerciseStateService } from 'src/app/services/exercise-state.service';
import { ChallengeEditViewModel } from 'src/app/models/ViewModels/ChallengeEditViewModel';
import { FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import {map, switchMap, filter, debounceTime} from 'rxjs/operators';
import { UsersService } from 'src/app/services/users.service';
import { UserInfo } from 'src/app/models/Responses/UserInfo';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-challenge-edit',
  templateUrl: './challenge-edit.component.html',
  styleUrls: ['./challenge-edit.component.scss']
})
export class ChallengeEditComponent extends LoadingComponent implements OnInit {

  private challengeId: string;

  public challenge: ChallengeEditViewModel = new ChallengeEditViewModel();
  public bounded = false;
  public challengeTime: Date[];
  myControl = new FormControl();
  users: Observable<UserInfo[]>;
  lols: string[] = ['lpol1', 'adwd2', '132f'];

  constructor(
    private userStateService: UserStateService,
    private challengesService: ChallengesService,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private titleService: Title,
    private currentExerciseState: ExerciseStateService,
    private usersService: UsersService
  ) { super(); }

  ngOnInit() {

    this.users = this.myControl.valueChanges
    .pipe(
      debounceTime(300),
      switchMap(v => this.usersService.getUsers(v))
    );


    this.route.paramMap
      .subscribe((params: ParamMap) => {
        const id = params.get('ChallengeId');
        this.currentExerciseState.setChallengeId(id);
        this.challengeId = id;
        this.currentExerciseState.currentChallenge.subscribe(c => {
          if (!c || c.Id !== id) {
            return;
          }
          this.challenge.Name = c.Name;
          this.titleService.setTitle(`Редактирование - ${this.challenge.Name}`);
          this.challenge.ChallengeAccessType = c.ChallengeAccessType;
          if (c.StartTime && c.EndTime) {
            this.bounded = true;
            this.challengeTime = [new Date(c.StartTime), new Date(c.EndTime)];
          }
          this.challenge.StartTime = c.StartTime;
          this.challenge.EndTime = c.EndTime;
        });
      });
  }

  addChallenge(): void {
    if (this.bounded && (!this.challengeTime || this.challengeTime.length !== 2)) {
      this.toastr.error('Введите время');
    }
    this.startLoading();
    if (this.bounded) {
      this.challenge.StartTime = this.challengeTime[0].toISOString();
      this.challenge.EndTime = this.challengeTime[1].toISOString();
    }
    this.challengesService.editChallenge(this.challengeId, this.challenge).subscribe(
      c => {
        this.toastr.success(`Событие ${this.challenge.Name} изменено`);
        this.stopLoading();
      }
    );
  }

  isAdmin(): boolean {
    return this.userStateService.IsAdmin();
  }
}
