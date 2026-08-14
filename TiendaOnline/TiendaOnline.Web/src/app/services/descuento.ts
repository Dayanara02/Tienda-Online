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
  IDescuento
} from '../model/IDescuento';

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
export class DescuentoService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Obtiene los descuentos.
  listar():
    Observable<IDescuento[]> {

    // Consulta los descuentos.
    return this.http.get<IDescuento[]>(
      `${baseUrl}/Descuentos`
    );
  }
}

// Mantiene el nombre anterior.
export {
  DescuentoService as Descuento
};
