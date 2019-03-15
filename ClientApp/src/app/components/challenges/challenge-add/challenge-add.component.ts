import { Component, OnInit } from '@angular/core';
import { UserStateService } from 'src/app/services/user-state.service';
import { Challenge } from 'src/app/models/Responses/Challenges/Challenge';
import { ToastrService } from 'ngx-toastr';
import { ChallengeAccessType } from 'src/app/models/General/ChallengeAccessType';
import { LoadingComponent } from '../../helpers/loading-component';
import { ChallengesService } from 'src/app/services/challenges.service';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-challenge-add',
  templateUrl: './challenge-add.component.html',
  styleUrls: ['./challenge-add.component.scss']
})
export class ChallengeAddComponent extends LoadingComponent implements OnInit {

  public newChallenge: Challenge = new Challenge();
  public noTime = true;
  public challengeTime: Date[];
  public minTime = new Date();

  constructor(
    private usersService: UserStateService,
    private challengesService: ChallengesService,
    private titleService: Title,
    private toastr: ToastrService,
  ) { super(); }

  ngOnInit() {
    this.titleService.setTitle('Добавление соревнования');
    this.newChallenge.ChallengeAccessType = ChallengeAccessType.Public;
  }

  addChallenge(): void {
    if (!this.noTime && (!this.challengeTime || this.challengeTime.length !== 2)) {
      this.toastr.error('Введите время');
    }
    this.startLoading();
    if (!this.noTime) {
      this.newChallenge.StartTime = this.challengeTime[0].toISOString();
      this.newChallenge.EndTime = this.challengeTime[1].toISOString();
    }
    this.challengesService.createChallenge(this.newChallenge).subscribe(
      c => {
        this.toastr.success(`Событие ${this.newChallenge.Name} создано`);
        this.stopLoading();
      }
    );
  }

  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
}
