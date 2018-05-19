import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExerciseInoutComponent } from './exercise-inout.component';

describe('ExerciseInoutComponent', () => {
  let component: ExerciseInoutComponent;
  let fixture: ComponentFixture<ExerciseInoutComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExerciseInoutComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExerciseInoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
