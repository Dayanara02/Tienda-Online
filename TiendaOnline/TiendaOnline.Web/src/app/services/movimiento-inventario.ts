// Permite crear e inyectar servicios.
import {
  inject,
  Injectable
} from '@angular/core';

// Permite realizar peticiones HTTP.
import {
  HttpClient
} from '@angular/common/http';

// Permite manejar respuestas asíncronas.
import {
  Observable
} from 'rxjs';

// Importa el modelo.
import {
  IMovimientoInventario
} from '../model/IMovimientoInventario';

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
export class MovimientoInventarioService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Obtiene todos los movimientos.
  listar():
    Observable<IMovimientoInventario[]> {

    // Consulta los movimientos.
    return this.http.get<IMovimientoInventario[]>(
      `${baseUrl}/MovimientoInventarios`
    );
  }
}

// Mantiene el nombre anterior.
export {
  MovimientoInventarioService as MovimientoInventario
};
