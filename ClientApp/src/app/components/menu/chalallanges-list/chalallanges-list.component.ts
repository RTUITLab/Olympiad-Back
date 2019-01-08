import { Component, OnInit } from '@angular/core';
import { ChallengesService } from 'src/app/services/challenges.service';
import { ChallengeCompactResponse } from 'src/app/models/Responses/Challenges/ChallengeCompactResponse';
import { Solution } from 'src/app/models/Solution';
import { Helpers } from 'src/app/Helpers/Helpers';
import { Router } from '@angular/router';
import { Challenge } from 'src/app/models/Responses/Challenges/Challenge';
import { ExerciseStateService } from 'src/app/services/exercise-state.service';

@Component({
  selector: 'app-chalallanges-list',
  templateUrl: './chalallanges-list.component.html',
  styleUrls: ['./chalallanges-list.component.css']
})
export class ChalallangesListComponent implements OnInit {

  constructor(
    private router: Router,
    private challengesService: ChallengesService,
    private currentExerciseState: ExerciseStateService) { }

  public challenges: Array<ChallengeCompactResponse> = [];
  public currentChallengeId?: string;
  public currentChallenge?: Challenge;

  ngOnInit() {
    this.challengesService.getChallengesList().subscribe(c => {
      if (!c) {
        return;
      }
      this.challenges = c;
    });
    this.currentExerciseState.currentChallengeId.subscribe(id => {
      this.currentChallengeId = id;
    });
    this.currentExerciseState.currentChallenge.subscribe(c => {
      this.currentChallenge = c;
    });
  }

  public clearChallenge() {
    this.currentChallengeId = null;
    this.currentChallenge = null;
  }

  public goToChallenge(challenge: ChallengeCompactResponse) {
    this.currentExerciseState.setChallengeId(challenge.Id);
    this.router.navigate(['challenges', challenge.Id]);
  }

  public challengeEnd(timeStr: string): string {
    if (!timeStr) {
      return 'Бессрочное соревнованиие';
    }
    return `Окончание: ${Helpers.prettyTime(timeStr)}`;
  }


}
