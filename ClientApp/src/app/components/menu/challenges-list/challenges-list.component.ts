import { Component, OnInit } from '@angular/core';
import { ChallengesService } from 'src/app/services/challenges.service';
import { Solution } from 'src/app/models/Solution';
import { DateHelpers } from 'src/app/Helpers/DateHelpers';
import { Router } from '@angular/router';
import { Challenge } from 'src/app/models/Responses/Challenges/Challenge';
import { ExerciseStateService } from 'src/app/services/exercise-state.service';
import { ChallengeState } from 'src/app/models/General/ChallengeState';
import { ChallengeHelpers } from 'src/app/Helpers/ChallengeHelpers';
import { Exercise } from 'src/app/models/Exercise';
import { ExerciseService } from 'src/app/services/exercise.service';

@Component({
  selector: 'app-challenges-list',
  templateUrl: './challenges-list.component.html',
  styleUrls: ['./challenges-list.component.css']
})
export class ChallengesListComponent implements OnInit {

  constructor(
    private router: Router,
    private challengesService: ChallengesService,
    private exerciseService: ExerciseService,
    private currentExerciseState: ExerciseStateService) { }

  public challenges: Array<Challenge> = [];
  public currentChallengeId?: string;
  public currentChallenge?: Challenge;
  public currentChallengeState?: ChallengeState;
  public currentChallengeTimeLeft: string;
  public currentExercises: Array<Exercise>;

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
    this.currentExerciseState.currentChallengeState.subscribe(s => this.updateState(s));
    const timer = setInterval(() =>
      this.timeToEnd(), 1000);
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

  goToExercise(id: string) {
    this.router.navigate(['exercises', id]);
  }

  public challengeTime(challenge: Challenge): string {
    return ChallengeHelpers.ChallengeTime(challenge);
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

  private timeToEnd(): void {
    if (!this.currentChallenge) {
      return;
    }
    const state = ChallengeHelpers.CalcChallengeState(this.currentChallenge);
    this.currentExerciseState.setChallengeState(state);
  }
}
