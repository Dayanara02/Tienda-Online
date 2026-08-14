import { TestBed } from '@angular/core/testing';

import { CompraProveedor } from './compra-proveedor';

describe('CompraProveedor', () => {
  let service: CompraProveedor;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CompraProveedor);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
