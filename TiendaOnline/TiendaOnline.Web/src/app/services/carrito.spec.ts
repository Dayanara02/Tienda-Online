// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio del carrito.
import { Carrito } from './carrito';

// Agrupa las pruebas del servicio.
describe('Carrito', () => {

  // Guarda una instancia del servicio.
  let service: Carrito;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(Carrito);
  });

  // Comprueba que el servicio se pueda crear.
  it('should be created', () => {

    // Verifica que exista.
    expect(service).toBeTruthy();
  });
});
