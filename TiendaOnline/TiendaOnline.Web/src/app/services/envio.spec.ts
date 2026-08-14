// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio de envíos.
import { Envio } from './envio';

// Agrupa las pruebas del servicio.
describe('Envio', () => {

  // Guarda una instancia del servicio.
  let service: Envio;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(Envio);
  });

  // Comprueba que el servicio se pueda crear.
  it('should be created', () => {

    // Verifica que exista.
    expect(service).toBeTruthy();
  });
});
