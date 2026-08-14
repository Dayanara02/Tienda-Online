// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio de direcciones.
import {
  DireccionUsuario
} from './direccion-usuario';

// Agrupa las pruebas del servicio.
describe('DireccionUsuario', () => {

  // Guarda una instancia del servicio.
  let service: DireccionUsuario;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(
        DireccionUsuario
      );
  });

  // Comprueba que el servicio se pueda crear.
  it('should be created', () => {

    // Verifica que exista.
    expect(service).toBeTruthy();
  });
});
