import { TestBed } from '@angular/core/testing';

import { RepeatPasswordValidService } from './repeat-password-valid.service';

describe('RepeatPasswordValidService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: RepeatPasswordValidService = TestBed.get(RepeatPasswordValidService);
    expect(service).toBeTruthy();
  });
});
