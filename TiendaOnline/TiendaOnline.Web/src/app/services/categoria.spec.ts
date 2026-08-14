// Importa las herramientas de prueba.
import { TestBed } from '@angular/core/testing';

// Permite usar HttpClient en pruebas.
import { provideHttpClient } from '@angular/common/http';

// Permite simular peticiones HTTP.
import {
  HttpTestingController,
  provideHttpClientTesting
} from '@angular/common/http/testing';

// Importa el servicio Categoria.
import { Categoria } from './categoria';

// Agrupa las pruebas del servicio.
describe('Categoria', () => {

  // Guarda una instancia del servicio.
  let service: Categoria;

  // Controla las peticiones HTTP.
  let httpMock: HttpTestingController;

  // Se ejecuta antes de cada prueba.
  beforeEach(() => {

    // Configura el entorno de prueba.
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
      TestBed.inject(Categoria);

    // Obtiene el controlador HTTP.
    httpMock =
      TestBed.inject(HttpTestingController);
  });

  // Se ejecuta después de cada prueba.
  afterEach(() => {

    // Verifica que no queden peticiones.
    httpMock.verify();
  });

  // Comprueba que el servicio exista.
  it('should be created', () => {

    // Espera que el servicio sea válido.
    expect(service).toBeTruthy();
  });
});
