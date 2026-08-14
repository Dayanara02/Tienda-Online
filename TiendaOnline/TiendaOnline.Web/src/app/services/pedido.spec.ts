// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Permite utilizar HttpClient.
import { provideHttpClient } from '@angular/common/http';

// Permite simular peticiones HTTP.
import {
  HttpTestingController,
  provideHttpClientTesting
} from '@angular/common/http/testing';

// Importa el servicio.
import { Pedido } from './pedido';

// Agrupa las pruebas.
describe('Pedido', () => {

  // Guarda el servicio.
  let service: Pedido;

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
      TestBed.inject(Pedido);

    // Obtiene el controlador HTTP.
    httpMock =
      TestBed.inject(HttpTestingController);
  });

  // Se ejecuta al terminar.
  afterEach(() => {

    // Verifica las peticiones.
    httpMock.verify();
  });

  // Comprueba que exista.
  it('should be created', () => {

    // Valida el servicio.
    expect(service).toBeTruthy();
  });
});
