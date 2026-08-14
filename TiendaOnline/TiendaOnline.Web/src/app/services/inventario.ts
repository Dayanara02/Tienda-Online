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
  IInventario
} from '../model/IInventario';

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
export class InventarioService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Obtiene todos los inventarios.
  listar():
    Observable<IInventario[]> {

    // Consulta los registros.
    return this.http.get<IInventario[]>(
      `${baseUrl}/Inventarios`
    );
  }
}

// Mantiene el nombre anterior.
export {
  InventarioService as Inventario
};
