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
  IProveedor
} from '../model/IProveedor';

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
export class ProveedorService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Obtiene todos los proveedores.
  listar():
    Observable<IProveedor[]> {

    // Consulta los proveedores.
    return this.http.get<IProveedor[]>(
      `${baseUrl}/Proveedores`
    );
  }
}

// Mantiene el nombre anterior.
export {
  ProveedorService as Proveedor
};
