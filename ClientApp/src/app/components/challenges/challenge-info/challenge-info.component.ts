import { Component, OnInit } from '@angular/core';
import { ChallengesService } from 'src/app/services/challenges.service';
import { ExerciseStateService } from 'src/app/services/exercise-state.service';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { Challenge } from 'src/app/models/Responses/Challenges/Challenge';
import { DateHelpers } from 'src/app/Helpers/DateHelpers';
import { UserStateService } from 'src/app/services/user-state.service';
import { ChallengeHelpers } from 'src/app/Helpers/ChallengeHelpers';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-challenge-info',
  templateUrl: './challenge-info.component.html',
  styleUrls: ['./challenge-info.component.scss']
})
export class ChallengeInfoComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private usersService: UserStateService,
    private titleService: Title,
    private currentExerciseState: ExerciseStateService,
    private challengesService: ChallengesService) { }

  public challenge?: Challenge;
  public dump: object;
  public exerciseNames: string[] = [];
  public statusMode = 'allNumbers';
  ngOnInit() {
    this.route.paramMap.subscribe(
      async params => {
        const id = params.get('ChallengeId');
        this.currentExerciseState.setChallengeId(id);
        this.challenge = await this.challengesService.getChallenge(id);
        this.currentExerciseState.setChallenge(this.challenge);
        this.titleService.setTitle(`${this.challenge.Name}`);
        const dump = await this.challengesService.getDump(id);
        for (const userId in dump) {
          if (dump.hasOwnProperty(userId)) {
            const element = dump[userId];
            for (const exerciseName in element) {
              if (element.hasOwnProperty(exerciseName) && this.exerciseNames.indexOf(exerciseName) === -1) {
                this.exerciseNames.push(exerciseName);
              }
            }
          }
        }
        this.exerciseNames.sort();
        this.dump = dump;
        console.log(dump);
      }
    );
  }
  public challengeTime(challenge: Challenge): string {
    return ChallengeHelpers.ChallengeTime(challenge);
  }
  public prettyTime(time: string): string {
    return DateHelpers.prettyTime(time);
  }

  public editChallenge() {
    this.router.navigate(['edit-challenge', this.challenge.Id]);
  }

  public userIds(): string[] {
    return Object.keys(this.dump);
  }

  public solutionStatus(userId: string, exerciseName: string): number {
    if (this.dump[userId][exerciseName]) {
      return this.dump[userId][exerciseName].Status;
    }
    return 0;
  }

  public score(userId: string, exerciseName: string): number {
    if (this.dump[userId][exerciseName]) {
      return this.dump[userId][exerciseName].ExerciseScore;
    }
    return 0;
  }

  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
}
