// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio de métodos de pago.
import { MetodoPago } from './metodo-pago';

// Agrupa las pruebas del servicio.
describe('MetodoPago', () => {

  // Guarda una instancia del servicio.
  let service: MetodoPago;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(MetodoPago);
  });

  // Comprueba que el servicio se pueda crear.
  it('should be created', () => {

    // Verifica que exista.
    expect(service).toBeTruthy();
  });
});
