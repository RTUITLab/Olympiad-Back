import { Injectable, OnInit } from '@angular/core';
import { Observable, Subscriber, BehaviorSubject } from 'rxjs';
import { Exercise } from '../models/Exercise';
import { Challenge } from '../models/Responses/Challenges/Challenge';

@Injectable({
  providedIn: 'root'
})
export class ExerciseStateService {

  private currentExerciseIdObserver = new BehaviorSubject<string>(null);
  private currentExerciseObserver = new BehaviorSubject<Exercise>(null);
  private currentChallengeIdObserver = new BehaviorSubject<string>(null);
  private currentChallengeObserver = new BehaviorSubject<Challenge>(null);

  public currentExerciseId = this.currentExerciseIdObserver.asObservable();
  public exerciseId: string;
  public currentExercise = this.currentExerciseObserver.asObservable();
  public exercise: Exercise;
  public currentChallengeId = this.currentChallengeIdObserver.asObservable();
  public challengeId: string;
  public currentChallenge = this.currentChallengeObserver.asObservable();
  public challenge: Challenge;

  public setExerciseId(exerciseId: string): void {
    this.currentExerciseIdObserver.next(exerciseId);
    this.exerciseId = exerciseId;
  }
  public setExercise(exercise: Exercise): void {
    this.currentExerciseObserver.next(exercise);
    this.exercise = exercise;
  }
  public setChallengeId(challengeId: string): void {
    this.currentChallengeIdObserver.next(challengeId);
    this.challengeId = challengeId;
  }
  public setChallenge(challenge: Challenge): void {
    this.currentChallengeObserver.next(challenge);
    this.currentChallengeIdObserver.next(challenge.Id);
    this.challenge = challenge;
  }
}
