// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio de inventario.
import { Inventario } from './inventario';

// Agrupa las pruebas del servicio.
describe('Inventario', () => {

  // Guarda una instancia del servicio.
  let service: Inventario;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(Inventario);
  });

  // Comprueba que el servicio se pueda crear.
  it('should be created', () => {

    // Verifica que exista.
    expect(service).toBeTruthy();
  });
});
