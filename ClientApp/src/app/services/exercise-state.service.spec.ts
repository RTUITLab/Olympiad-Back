import { TestBed } from '@angular/core/testing';

import { ExerciseStateService } from './exercise-state.service';

describe('ExerciseStateService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ExerciseStateService = TestBed.get(ExerciseStateService);
    expect(service).toBeTruthy();
  });
});
