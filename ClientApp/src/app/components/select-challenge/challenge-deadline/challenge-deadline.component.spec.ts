import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChallengeDeadlineComponent } from './challenge-deadline.component';

describe('ChallengeDeadlineComponent', () => {
  let component: ChallengeDeadlineComponent;
  let fixture: ComponentFixture<ChallengeDeadlineComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChallengeDeadlineComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChallengeDeadlineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
