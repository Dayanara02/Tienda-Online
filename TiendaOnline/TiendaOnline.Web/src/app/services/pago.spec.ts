// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio de pagos.
import { Pago } from './pago';

// Agrupa las pruebas del servicio.
describe('Pago', () => {

  // Guarda una instancia del servicio.
  let service: Pago;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(Pago);
  });

  // Comprueba que el servicio se pueda crear.
  it('should be created', () => {

    // Verifica que exista.
    expect(service).toBeTruthy();
  });
});
