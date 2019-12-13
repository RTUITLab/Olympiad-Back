import { TestBed } from '@angular/core/testing';

import { FormValidateService } from './form-validate.service';

describe('FormValidateService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: FormValidateService = TestBed.get(FormValidateService);
    expect(service).toBeTruthy();
  });
});
