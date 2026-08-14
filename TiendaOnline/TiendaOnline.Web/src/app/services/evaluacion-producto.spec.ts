// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio de evaluaciones.
import {
  EvaluacionProducto
} from './evaluacion-producto';

// Agrupa las pruebas del servicio.
describe('EvaluacionProducto', () => {

  // Guarda una instancia del servicio.
  let service: EvaluacionProducto;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(
        EvaluacionProducto
      );
  });

  // Comprueba que el servicio se pueda crear.
  it('should be created', () => {

    // Verifica que exista.
    expect(service).toBeTruthy();
  });
});
