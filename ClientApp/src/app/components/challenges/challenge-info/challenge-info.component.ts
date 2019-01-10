import { Component, OnInit } from '@angular/core';
import { ChallengesService } from 'src/app/services/challenges.service';
import { ExerciseStateService } from 'src/app/services/exercise-state.service';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Challenge } from 'src/app/models/Responses/Challenges/Challenge';
import { Helpers } from 'src/app/Helpers/Helpers';

@Component({
  selector: 'app-challenge-info',
  templateUrl: './challenge-info.component.html',
  styleUrls: ['./challenge-info.component.css']
})
export class ChallengeInfoComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private challengesService: ChallengesService,
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
    return Helpers.prettyTime(time);
  }

}
