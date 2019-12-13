import { TestBed } from '@angular/core/testing';

import { UsersGenerateService } from './users-generate.service';

describe('UsersGenerateService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: UsersGenerateService = TestBed.get(UsersGenerateService);
    expect(service).toBeTruthy();
  });
});
