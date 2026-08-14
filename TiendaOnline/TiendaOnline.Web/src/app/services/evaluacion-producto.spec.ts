import { TestBed } from '@angular/core/testing';

import { EvaluacionProducto } from './evaluacion-producto';

describe('EvaluacionProducto', () => {
  let service: EvaluacionProducto;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EvaluacionProducto);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
