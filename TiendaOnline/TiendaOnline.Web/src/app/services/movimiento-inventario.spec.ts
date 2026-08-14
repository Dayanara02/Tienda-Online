// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio de movimientos.
import {
  MovimientoInventario
} from './movimiento-inventario';

// Agrupa las pruebas del servicio.
describe('MovimientoInventario', () => {

  // Guarda una instancia del servicio.
  let service: MovimientoInventario;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(
        MovimientoInventario
      );
  });

  // Comprueba que el servicio se pueda crear.
  it('should be created', () => {

    // Verifica que exista.
    expect(service).toBeTruthy();
  });
});
