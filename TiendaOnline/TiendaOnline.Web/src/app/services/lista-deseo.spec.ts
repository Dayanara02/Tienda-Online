// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio de lista de deseos.
import { ListaDeseo } from './lista-deseo';

// Agrupa las pruebas del servicio.
describe('ListaDeseo', () => {

  // Guarda una instancia del servicio.
  let service: ListaDeseo;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(ListaDeseo);
  });

  // Comprueba que el servicio se pueda crear.
  it('should be created', () => {

    // Verifica que exista.
    expect(service).toBeTruthy();
  });
});
