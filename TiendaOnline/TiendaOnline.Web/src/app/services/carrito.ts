// Permite crear e inyectar servicios.
import {
  inject,
  Injectable
} from '@angular/core';

// Permite realizar peticiones HTTP.
import {
  HttpClient
} from '@angular/common/http';

// Importa la configuración.
import {
  environment
} from '../../environments/environment';

// Guarda la dirección principal.
const baseUrl =
  environment.apiUrl;

// Permite utilizar el servicio.
@Injectable({
  providedIn: 'root'
})
export class CarritoService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Guarda la dirección del controlador.
  readonly apiUrl =
    `${baseUrl}/Carritos`;

  // Expone HttpClient para las operaciones
  // que se agregarán según el controlador real.
  obtenerHttpClient():
    HttpClient {

    // Devuelve la instancia HTTP.
    return this.http;
  }
}

// Mantiene el nombre anterior.
export {
  CarritoService as Carrito
};
