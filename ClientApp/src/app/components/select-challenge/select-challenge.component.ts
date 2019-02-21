import { Component, OnInit } from '@angular/core';
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
    private router: Router,
    private exerciseService: ExerciseService,
    private currentExerciseState: ExerciseStateService,

  ) { }
  public challenges: Array<Challenge> = [];
  public currentChallenge: Challenge;

  public currentExercises: Array<ExerciseCompact> = [];
  public currentExercise: ExerciseCompact;

  public currentChallengeState?: ChallengeState;
  public currentChallengeTimeLeft: string;

  private initChallengeId: string;
  private initExerciseId: string;
  private defaultChallenge: Challenge = {
    ChallengeAccessType: ChallengeAccessType.Public,
    CreationTime: '',
    Id: '',
    Name: 'Выберите соревнование',
  };
  private defaultExercise: ExerciseCompact = {
    Name: 'Выберите задание',
  };

  isDefault(item: Challenge) {
    return item === this.defaultChallenge
    || item === this.defaultExercise;
  }

  async ngOnInit() {
    const allchallenges = await this.challengesService.getChallengesList();
    allchallenges.unshift(this.defaultChallenge);
    this.challenges = allchallenges;

    this.currentChallenge = this.defaultChallenge;
    this.currentExercises.push(this.defaultExercise);
    this.currentExercise = this.defaultExercise;

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

  async goToChallenge() {
    this.currentExerciseState.setChallengeId(this.currentChallenge.Id);
    this.router.navigate(['challenges', this.currentChallenge.Id]);
    this.initExerciseId = null;
    await this.loadExercises();
  }

  goToExercise() {
    this.router.navigate(['exercises', this.currentExercise.Id]);
  }

  private async loadExercises() {
    const exercises = await this.exerciseService.getExercises(this.currentChallenge.Id);
    exercises.unshift(this.defaultExercise);
    this.currentExercises = exercises;
    this.currentExercise = this.defaultExercise;
    if (this.initExerciseId) {
      this.currentExercise = this.currentExercises.find(ex => ex.Id === this.initExerciseId);
    }
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
