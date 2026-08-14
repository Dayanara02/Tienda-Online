import { TestBed } from '@angular/core/testing';

import { ListaDeseo } from './lista-deseo';

describe('ListaDeseo', () => {
  let service: ListaDeseo;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ListaDeseo);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
