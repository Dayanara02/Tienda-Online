// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio de proformas.
import { Proforma } from './proforma';

// Agrupa las pruebas del servicio.
describe('Proforma', () => {

  // Guarda una instancia del servicio.
  let service: Proforma;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(Proforma);
  });

  // Comprueba que el servicio se pueda crear.
  it('should be created', () => {

    // Verifica que exista.
    expect(service).toBeTruthy();
  });
});
