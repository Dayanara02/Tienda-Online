// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio de pedidos.
import { Pedido } from './pedido';

// Agrupa las pruebas del servicio.
describe('Pedido', () => {

  // Guarda una instancia del servicio.
  let service: Pedido;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(Pedido);
  });

  // Comprueba que el servicio se pueda crear.
  it('should be created', () => {

    // Verifica que exista.
    expect(service).toBeTruthy();
  });
});
