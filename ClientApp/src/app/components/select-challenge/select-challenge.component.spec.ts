import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectChallengeComponent } from './select-challenge.component';

describe('SelectChallengeComponent', () => {
  let component: SelectChallengeComponent;
  let fixture: ComponentFixture<SelectChallengeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SelectChallengeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectChallengeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
