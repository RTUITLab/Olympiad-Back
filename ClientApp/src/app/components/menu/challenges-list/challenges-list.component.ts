import { Component, OnInit } from '@angular/core';
import { ChallengesService } from 'src/app/services/challenges.service';
import { ChallengeCompactResponse } from 'src/app/models/Responses/Challenges/ChallengeCompactResponse';
import { Solution } from 'src/app/models/Solution';
import { Helpers } from 'src/app/Helpers/Helpers';
import { Router } from '@angular/router';
import { Challenge } from 'src/app/models/Responses/Challenges/Challenge';
import { ExerciseStateService } from 'src/app/services/exercise-state.service';

@Component({
  selector: 'app-challenges-list',
  templateUrl: './challenges-list.component.html',
  styleUrls: ['./challenges-list.component.css']
})
export class ChallengesListComponent implements OnInit {

  constructor(
    private router: Router,
    private challengesService: ChallengesService,
    private currentExerciseState: ExerciseStateService) { }

  public challenges: Array<ChallengeCompactResponse> = [];
  public currentChallengeId?: string;
  public currentChallenge?: Challenge;
  public currentChallengeTimeLeft: string;

  ngOnInit() {
    this.challengesService.getChallengesList().subscribe(c => {
      if (!c) {
        return;
      }
      this.challenges = c;
    });
    this.currentExerciseState.currentChallengeId.subscribe(id => {
      if (!id) {
        return;
      }
      this.currentChallengeId = id;
      if (!this.currentChallenge) {
        this.challengesService.getChallenge(id).subscribe(c => {
          this.currentExerciseState.setChallenge(c);
        });
      }
    });
    this.currentExerciseState.currentChallenge.subscribe(c => {
      if (!c) {
        return;
      }
      this.currentChallenge = c;
    });
    const timer = setInterval(() =>
      this.timeToEnd(), 1000);
  }

  public clearChallenge() {
    this.currentChallengeId = null;
  }

  public goToChallenge(challenge: ChallengeCompactResponse) {
    this.currentExerciseState.setChallengeId(challenge.Id);
    this.router.navigate(['challenges', challenge.Id]);
    this.challengesService.getChallenge(challenge.Id).subscribe(c => {
      this.currentExerciseState.setChallenge(c);
    });
  }

  goToExercise(id: string) {
    this.router.navigate(['exercises', id]);
  }

  public challengeEnd(timeStr: string): string {
    if (!timeStr) {
      return 'Бессрочное соревнованиие';
    }
    return `Окончание: ${Helpers.prettyTime(timeStr)}`;
  }

  private timeToEnd(): void {

    if (!this.currentChallenge) {
      return;
    }
    if (!this.currentChallenge.EndTime) {
      this.currentChallengeTimeLeft = null;
    }

    const oneSecond = 1000;
    const oneMinute = oneSecond * 60;
    const oneHour = oneMinute * 60;
    const oneDay = oneHour * 24;
    const now = new Date();
    const endTime = new Date(this.currentChallenge.EndTime);
    let difference = endTime.getTime() - now.getTime();

    let result = '';
    const days = Math.floor((difference / oneDay));
    if (days > 0) {
      result += `Дней: ${days}`;
      difference -= days * oneDay;
    }
    const hours = Math.floor((difference / oneHour));
    if (hours > 0) {
      result += ` Часов: ${hours}`;
      difference -= hours * oneHour;
    }
    const minutes = Math.floor((difference / oneMinute));
    if (minutes > 0) {
      result += ` Минут: ${minutes}`;
      difference -= minutes * oneMinute;
    }
    const seconds = Math.floor((difference / oneSecond));
    if (seconds >= 0) {
      result += ` Секунд: ${seconds}`;
    }
    if (difference < 0) {
      result += ` Секунд: 0`;
    }

    this.currentChallengeTimeLeft = result;
  }
}
