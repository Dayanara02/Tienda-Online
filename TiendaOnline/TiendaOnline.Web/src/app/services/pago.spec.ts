// Importa herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Permite utilizar HttpClient.
import { provideHttpClient } from '@angular/common/http';

// Permite simular peticiones HTTP.
import {
  HttpTestingController,
  provideHttpClientTesting
} from '@angular/common/http/testing';

// Importa el servicio.
import { Pago } from './pago';

// Agrupa las pruebas.
describe('Pago', () => {

  // Guarda el servicio.
  let service: Pago;

  // Controla las peticiones.
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
      TestBed.inject(Pago);

    // Obtiene el controlador HTTP.
    httpMock =
      TestBed.inject(HttpTestingController);
  });

  // Se ejecuta al finalizar.
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
