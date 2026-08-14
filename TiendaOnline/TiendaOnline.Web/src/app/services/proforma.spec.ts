import { TestBed } from '@angular/core/testing';

import { Proforma } from './proforma';

describe('Proforma', () => {
  let service: Proforma;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Proforma);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
