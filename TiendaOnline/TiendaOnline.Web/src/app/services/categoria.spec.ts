// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio de categorías.
import { Categoria } from './categoria';

// Agrupa las pruebas.
describe('Categoria', () => {

  // Guarda una instancia del servicio.
  let service: Categoria;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(Categoria);
  });

  // Comprueba que el servicio exista.
  it('should be created', () => {

    // Verifica que se haya creado.
    expect(service).toBeTruthy();
  });
});
