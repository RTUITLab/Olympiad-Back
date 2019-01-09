import { Component, OnInit } from '@angular/core';
import { ChallengesService } from 'src/app/services/challenges.service';
import { ExerciseStateService } from 'src/app/services/exercise-state.service';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Challenge } from 'src/app/models/Responses/Challenges/Challenge';

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
        this.challengesService.getChallenge(params.get('ChallengeId')).subscribe(c => {
          this.currentExerciseState.setChallenge(c);
          this.challenge = c;
        });
      });
  }

}
