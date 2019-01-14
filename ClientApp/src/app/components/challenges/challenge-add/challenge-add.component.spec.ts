import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChallengeAddComponent } from './challenge-add.component';

describe('ChallengeAddComponent', () => {
  let component: ChallengeAddComponent;
  let fixture: ComponentFixture<ChallengeAddComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChallengeAddComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChallengeAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
