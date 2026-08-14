// Importa las herramientas de prueba de Angular.
import { TestBed } from '@angular/core/testing';

// Importa HttpClient para poder probar el servicio.
import { provideHttpClient } from '@angular/common/http';

// Importa herramientas para simular peticiones HTTP.
import {
  HttpTestingController,
  provideHttpClientTesting
} from '@angular/common/http/testing';

// Importa el servicio Auth.
import { Auth } from './auth';

// Agrupa las pruebas del servicio.
describe('Auth', () => {

  // Guarda una instancia del servicio.
  let service: Auth;

  // Permite controlar peticiones HTTP simuladas.
  let httpMock: HttpTestingController;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de pruebas.
    TestBed.configureTestingModule({
      providers: [
        // Habilita HttpClient.
        provideHttpClient(),

        // Habilita pruebas HTTP.
        provideHttpClientTesting()
      ]
    });

    // Obtiene el servicio.
    service =
      TestBed.inject(Auth);

    // Obtiene el controlador HTTP.
    httpMock =
      TestBed.inject(HttpTestingController);
  });

  // Se ejecuta después de cada prueba.
  afterEach(() => {

    // Comprueba que no queden peticiones pendientes.
    httpMock.verify();
  });

  // Comprueba que el servicio exista.
  it('should be created', () => {

    // Espera que el servicio sea válido.
    expect(service).toBeTruthy();
  });
});
