import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ChallengesService } from 'src/app/services/challenges.service';
import { Challenge } from 'src/app/models/Responses/Challenges/Challenge';
import { Router } from '@angular/router';
import { ExerciseStateService } from 'src/app/services/exercise-state.service';
import { Exercise } from 'src/app/models/Exercise';
import { ChallengeState } from 'src/app/models/General/ChallengeState';
import { ExerciseService } from 'src/app/services/exercise.service';
import { ChallengeHelpers } from 'src/app/Helpers/ChallengeHelpers';
import { ChallengeAccessType } from 'src/app/models/General/ChallengeAccessType';
import { ExerciseCompact } from 'src/app/models/Responses/ExerciseCompact';

@Component({
  selector: 'app-select-challenge',
  templateUrl: './select-challenge.component.html',
  styleUrls: ['./select-challenge.component.scss']
})
export class SelectChallengeComponent implements OnInit {
  constructor(
    private challengesService: ChallengesService,
    private exerciseService: ExerciseService,
    private currentExerciseState: ExerciseStateService,
    private cdRef: ChangeDetectorRef
  ) { }
  public challenges: Array<Challenge> = [];
  public currentChallenge: Challenge;

  public currentExercises: Array<ExerciseCompact> = [];
  public currentExercise: ExerciseCompact;

  public currentChallengeState?: ChallengeState;
  public currentChallengeTimeLeft: string;

  private initChallengeId: string;
  private initExerciseId: string;

  async ngOnInit() {
    const allchallenges = await this.challengesService.getChallengesList();
    this.challenges = allchallenges;

    if (this.initChallengeId) {
      this.currentChallenge = this.challenges.find(c => c.Id === this.initChallengeId);
      this.loadExercises();
    }
    this.currentExerciseState.currentChallengeId.subscribe(challengeId => {
      if (!challengeId) {
        return;
      }
      this.initChallengeId = challengeId;
      if (this.challenges) {
        this.currentChallenge = this.challenges.find(c => c.Id === this.initChallengeId);
        this.cdRef.detectChanges();
        this.loadExercises();
      }
    });
    this.currentExerciseState.currentExerciseId.subscribe(exerciseId => {
      if (!exerciseId) {
        return;
      }
      this.initExerciseId = exerciseId;
      if (this.currentExercises) {
        this.currentExercise = this.currentExercises.find(ex => ex.Id === exerciseId);
      }
    });
    this.currentExerciseState.currentChallengeState.subscribe(s => this.updateState(s));
    const timer = setInterval(() => this.timeToEnd(), 1000);
  }

  private async loadExercises() {
    const exercises = await this.exerciseService.getExercises(this.currentChallenge.Id);
    this.currentExercises = exercises.sort((a, b) => a.Name < b.Name ? -1 : 1);
    if (this.initExerciseId) {
      this.currentExercise = this.currentExercises.find(ex => ex.Id === this.initExerciseId);
    }
  }
  setDefaultExercise () {
    this.currentExercise = null;
    this.initExerciseId = null;
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
  private async updateState(state: ChallengeState) {
    if (state === null) {
      return;
    }
    this.currentChallengeState = state;
    this.currentChallengeTimeLeft = this.challengeTime(this.currentChallenge);
    if ((state === ChallengeState.InProgress ||
      state === ChallengeState.Ended) &&
      !this.currentExercises && this.initChallengeId) {
      this.loadExercises();
    }
  }
}
