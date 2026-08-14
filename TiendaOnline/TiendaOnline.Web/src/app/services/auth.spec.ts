// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio de autenticación.
import { Auth } from './auth';

// Agrupa las pruebas.
describe('Auth', () => {

  // Guarda una instancia del servicio.
  let service: Auth;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(Auth);
  });

  // Comprueba que el servicio exista.
  it('should be created', () => {

    // Verifica que se haya creado.
    expect(service).toBeTruthy();
  });
});
