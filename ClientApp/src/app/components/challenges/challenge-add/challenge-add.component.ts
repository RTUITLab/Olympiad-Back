import { Component, OnInit } from '@angular/core';
import { UserStateService } from 'src/app/services/user-state.service';
import { Challenge } from 'src/app/models/Responses/Challenges/Challenge';

@Component({
  selector: 'app-challenge-add',
  templateUrl: './challenge-add.component.html',
  styleUrls: ['./challenge-add.component.scss']
})
export class ChallengeAddComponent implements OnInit {

  public newChallenge: Challenge = new Challenge();
  public noTime = true;
  public challengeTime: Date[];
  public minTime = new Date();

  constructor(
    private usersService: UserStateService
  ) { }

  ngOnInit() {
    setInterval(() => console.log(this.challengeTime), 2000);
  }

  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
}
