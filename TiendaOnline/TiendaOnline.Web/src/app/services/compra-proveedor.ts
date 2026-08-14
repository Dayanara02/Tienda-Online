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
  ICompraProveedor
} from '../model/ICompraProveedor';

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
export class CompraProveedorService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Obtiene las compras a proveedores.
  listar():
    Observable<ICompraProveedor[]> {

    // Consulta las compras.
    return this.http.get<ICompraProveedor[]>(
      `${baseUrl}/CompraProveedors`
    );
  }
}

// Mantiene el nombre anterior.
export {
  CompraProveedorService as CompraProveedor
};
