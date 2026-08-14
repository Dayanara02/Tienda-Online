// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Importa el servicio de notificaciones.
import { Notificacion } from './notificacion';

// Agrupa las pruebas del servicio.
describe('Notificacion', () => {

  // Guarda una instancia del servicio.
  let service: Notificacion;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({});

    // Obtiene el servicio.
    service =
      TestBed.inject(Notificacion);
  });

  // Comprueba que el servicio se pueda crear.
  it('should be created', () => {

    // Verifica que exista.
    expect(service).toBeTruthy();
  });
});
