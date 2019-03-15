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
    private challangesService: ChallengesService) { }

  public challenge?: Challenge;

  ngOnInit() {
    this.route.paramMap.subscribe(
      async params => {
        const id = params.get('ChallengeId');
        this.currentExerciseState.setChallengeId(id);
        this.challenge = await this.challangesService.getChallenge(id);
        this.currentExerciseState.setChallenge(this.challenge);
        this.titleService.setTitle(`${this.challenge.Name}`);
      }
    );
  }
  public challengeTime(challenge: Challenge): string {
    return ChallengeHelpers.ChallengeTime(challenge);
  }
  public prettyTime(time: string): string {
    return DateHelpers.prettyTime(time);
  }

  editChallenge() {
    this.router.navigate(['edit-challenge', this.challenge.Id]);
  }


  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
}
