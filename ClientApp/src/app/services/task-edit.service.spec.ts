import { TestBed } from '@angular/core/testing';

import { TaskEditService } from './task-edit.service';

describe('TaskEditService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: TaskEditService = TestBed.get(TaskEditService);
    expect(service).toBeTruthy();
  });
});
