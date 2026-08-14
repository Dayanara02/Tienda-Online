// Permite crear e inyectar servicios.
import {
  inject,
  Injectable
} from '@angular/core';

// Permite realizar peticiones HTTP.
import {
  HttpClient
} from '@angular/common/http';

// Permite trabajar con respuestas asíncronas.
import {
  Observable
} from 'rxjs';

// Importa el modelo de método de pago.
import {
  IMetodoPago
} from '../model/IMetodoPago';

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
export class MetodoPagoService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Obtiene los métodos disponibles.
  listarDisponibles():
    Observable<IMetodoPago[]> {

    // Consulta los métodos activos.
    return this.http.get<IMetodoPago[]>(
      `${baseUrl}/MetodoPagos/disponibles`
    );
  }
}

// Mantiene el nombre anterior.
export {
  MetodoPagoService as MetodoPago
};
