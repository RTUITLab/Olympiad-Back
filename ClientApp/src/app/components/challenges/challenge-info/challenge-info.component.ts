import { Component, OnInit } from '@angular/core';
import { ChallengesService } from 'src/app/services/challenges.service';
import { ExerciseStateService } from 'src/app/services/exercise-state.service';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { Challenge } from 'src/app/models/Responses/Challenges/Challenge';
import { DateHelpers } from 'src/app/Helpers/DateHelpers';
import { UserStateService } from 'src/app/services/user-state.service';

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
    private currentExerciseState: ExerciseStateService) { }

  public challenge?: Challenge;

  ngOnInit() {
    this.route.paramMap
      .subscribe((params: ParamMap) => {
        const id = params.get('ChallengeId');
        this.currentExerciseState.setChallengeId(id);
        this.currentExerciseState.currentChallenge.subscribe(c => {
          if (!c || c.Id !== id) {
            return;
          }
          this.challenge = c;
        });
      });
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
