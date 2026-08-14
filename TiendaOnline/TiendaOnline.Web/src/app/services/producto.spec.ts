// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio de productos.
import { Producto } from './producto';

// Agrupa las pruebas del servicio.
describe('Producto', () => {

  // Guarda una instancia del servicio.
  let service: Producto;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(Producto);
  });

  // Comprueba que el servicio se pueda crear.
  it('should be created', () => {

    // Verifica que exista.
    expect(service).toBeTruthy();
  });
});
