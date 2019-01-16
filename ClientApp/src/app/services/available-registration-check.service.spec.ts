import { TestBed } from '@angular/core/testing';

import { AvailableRegistrationCheckService } from './available-registration-check.service';

describe('AvailableRegistrationCheckService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AvailableRegistrationCheckService = TestBed.get(AvailableRegistrationCheckService);
    expect(service).toBeTruthy();
  });
});
