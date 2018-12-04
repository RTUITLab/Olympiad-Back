import { TestBed } from '@angular/core/testing';
import { ExerciseEditService } from './exercise-edit.service';



describe('TaskEditService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ExerciseEditService = TestBed.get(ExerciseEditService);
    expect(service).toBeTruthy();
  });
});
