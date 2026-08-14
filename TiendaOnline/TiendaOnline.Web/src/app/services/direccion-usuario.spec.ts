import { TestBed } from '@angular/core/testing';

import { DireccionUsuario } from './direccion-usuario';

describe('DireccionUsuario', () => {
  let service: DireccionUsuario;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DireccionUsuario);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
