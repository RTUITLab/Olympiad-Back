import { Injectable } from '@angular/core';
import { Observable, Subscriber } from 'rxjs';
import { Exercise } from '../models/Exercise';
import { Challenge } from '../models/Responses/Challenges/Challenge';

@Injectable({
  providedIn: 'root'
})
export class ExerciseStateService {

  public currentExerciseId: Observable<string>;
  public currentExercise: Observable<Exercise>;
  public currentChallengeId: Observable<string>;
  public currentChallenge: Observable<Challenge>;

  private currentExerciseIdObserver: Subscriber<string>;
  private currentExerciseObserver: Subscriber<Exercise>;
  private currentChallengeIdObserver: Subscriber<string>;
  private currentChallengeObserver: Subscriber<Challenge>;

  constructor() {
    this.currentExerciseId = new Observable<string>(obs => {
      this.currentExerciseIdObserver = obs;
    });
    this.currentExercise = new Observable<Exercise>(obs => {
      this.currentExerciseObserver = obs;
    });
    this.currentChallengeId = new Observable<string>(obs => {
      this.currentChallengeIdObserver = obs;
    });
    this.currentChallenge = new Observable<Challenge>(obs => {
      this.currentChallengeObserver = obs;
    });
  }

  public setExerciseId(exerciseId: string): void {
    this.currentExerciseIdObserver.next(exerciseId);
  }
  public setExercise(exercise: Exercise): void {
    this.currentExerciseObserver.next(exercise);
  }
  public setChallengeId(challengeId: string): void {
    this.currentChallengeIdObserver.next(challengeId);
  }
  public setChallenge(challenge: Challenge): void {
    this.currentChallengeObserver.next(challenge);
  }
}
