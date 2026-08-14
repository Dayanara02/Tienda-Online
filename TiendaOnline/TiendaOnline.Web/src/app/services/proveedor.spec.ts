// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio de proveedores.
import { Proveedor } from './proveedor';

// Agrupa las pruebas del servicio.
describe('Proveedor', () => {

  // Guarda una instancia del servicio.
  let service: Proveedor;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(Proveedor);
  });

  // Comprueba que el servicio se pueda crear.
  it('should be created', () => {

    // Verifica que exista.
    expect(service).toBeTruthy();
  });
});
