import { Component, OnInit } from '@angular/core';
import { ChallengesService } from 'src/app/services/challenges.service';
import { Challenge } from 'src/app/models/Responses/Challenges/Challenge';
import { Router } from '@angular/router';
import { ExerciseStateService } from 'src/app/services/exercise-state.service';
import { Exercise } from 'src/app/models/Exercise';
import { ChallengeState } from 'src/app/models/General/ChallengeState';
import { ExerciseService } from 'src/app/services/exercise.service';
import { ChallengeHelpers } from 'src/app/Helpers/ChallengeHelpers';

@Component({
  selector: 'app-select-challenge',
  templateUrl: './select-challenge.component.html',
  styleUrls: ['./select-challenge.component.scss']
})
export class SelectChallengeComponent implements OnInit {
  constructor(
    private challengesService: ChallengesService,
    private router: Router,
    private exerciseService: ExerciseService,
    private currentExerciseState: ExerciseStateService,

  ) { }
  public challenges?: Array<Challenge> = [];
  public currentExercises: Array<Exercise>;
  public currentChallengeId?: string;
  public currentChallenge?: Challenge;
  public currentChallengeState?: ChallengeState;
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
      this.currentExercises = null;
    });
    this.currentExerciseState.currentChallengeState.subscribe(s => this.updateState(s));
    const timer = setInterval(() =>
      this.timeToEnd(), 1000);
  }
  selectChallenge(event: any) {
    console.log(event.target.value);
    this.currentExerciseState.setChallengeId(event.target.value);
    this.router.navigate(['challenges', event.target.value]);
    this.challengesService.getChallenge(event.target.value).subscribe(c => {
      this.currentExerciseState.setChallenge(c);
    });
  }
public clearChallenge() {
  this.currentChallengeId = null;
}

public goToChallenge(challenge: Challenge) {
  this.currentExerciseState.setChallengeId(challenge.Id);
  this.router.navigate(['challenges', challenge.Id]);
  this.challengesService.getChallenge(challenge.Id).subscribe(c => {
    this.currentExerciseState.setChallenge(c);
  });
}

public challengeTime(challenge: Challenge): string {
  return ChallengeHelpers.ChallengeTime(challenge);
}

private timeToEnd(): void {
  if (!this.currentChallenge) {
    return;
  }
  const state = ChallengeHelpers.CalcChallengeState(this.currentChallenge);
  this.currentExerciseState.setChallengeState(state);
}
  private updateState(state: ChallengeState): void {
    if (state === null) {
      return;
    }
    this.currentChallengeState = state;
    this.currentChallengeTimeLeft = this.challengeTime(this.currentChallenge);
    if ((state === ChallengeState.InProgress ||
      state === ChallengeState.Ended) &&
      !this.currentExercises && this.currentChallengeId) {
      this.exerciseService.getExercises(this.currentChallengeId)
        .subscribe(ex => this.currentExercises = ex);
    }
  }
  selectExercise(event: any) {
    console.log(event.target.value);
    this.router.navigate(['exercises', event.target.value]);
  }

}
