import { Component, OnInit, OnChanges, SimpleChanges, Input, ChangeDetectorRef } from '@angular/core';
import { Challenge } from 'src/app/models/Responses/Challenges/Challenge';
import { ChallengeHelpers } from 'src/app/Helpers/ChallengeHelpers';

@Component({
  selector: 'app-challenge-deadline',
  templateUrl: './challenge-deadline.component.html',
  styleUrls: ['./challenge-deadline.component.scss']
})
export class ChallengeDeadlineComponent implements OnInit, OnChanges {
  @Input() challenge: Challenge;
  constructor(
    private cdr: ChangeDetectorRef,
    ) { }
  ngOnChanges(changes: SimpleChanges): void {
    this.challenge = changes['challenge'].currentValue;
    this.cdr.detectChanges();

  }
  ngOnInit() {
  }
  public challengeTime(): string {
    return ChallengeHelpers.ChallengeTime(this.challenge);
  }
}
