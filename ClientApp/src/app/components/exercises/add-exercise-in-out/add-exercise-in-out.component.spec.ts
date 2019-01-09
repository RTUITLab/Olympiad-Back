import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddExerciseInOutComponent } from './add-exercise-in-out.component';

describe('AddExerciseInOutComponent', () => {
  let component: AddExerciseInOutComponent;
  let fixture: ComponentFixture<AddExerciseInOutComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddExerciseInOutComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddExerciseInOutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
