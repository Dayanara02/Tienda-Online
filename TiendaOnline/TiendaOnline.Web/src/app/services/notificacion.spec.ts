// Importa herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Permite usar HttpClient.
import { provideHttpClient } from '@angular/common/http';

// Permite simular peticiones HTTP.
import {
  HttpTestingController,
  provideHttpClientTesting
} from '@angular/common/http/testing';

// Importa el servicio.
import { Notificacion } from './notificacion';

// Agrupa las pruebas.
describe('Notificacion', () => {

  // Guarda el servicio.
  let service: Notificacion;

  // Controla las peticiones HTTP.
  let httpMock: HttpTestingController;

  // Configura cada prueba.
  beforeEach(() => {

    // Configura Angular.
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    // Obtiene el servicio.
    service =
      TestBed.inject(Notificacion);

    // Obtiene el controlador HTTP.
    httpMock =
      TestBed.inject(HttpTestingController);
  });

  // Se ejecuta después.
  afterEach(() => {

    // Verifica peticiones pendientes.
    httpMock.verify();
  });

  // Comprueba que exista.
  it('should be created', () => {

    // Valida el servicio.
    expect(service).toBeTruthy();
  });
});
