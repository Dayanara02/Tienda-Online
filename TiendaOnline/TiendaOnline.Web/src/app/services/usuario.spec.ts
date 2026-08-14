// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio de compras.
import {
  CompraProveedor
} from './compra-proveedor';

// Agrupa las pruebas del servicio.
describe('CompraProveedor', () => {

  // Guarda una instancia del servicio.
  let service: CompraProveedor;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(
        CompraProveedor
      );
  });

  // Comprueba que el servicio se pueda crear.
  it('should be created', () => {

    // Verifica que exista.
    expect(service).toBeTruthy();
  });
});
