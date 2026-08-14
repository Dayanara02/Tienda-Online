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
  IProforma
} from '../model/IProforma';

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
export class ProformaService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Obtiene las proformas.
  listar():
    Observable<IProforma[]> {

    // Consulta las proformas.
    return this.http.get<IProforma[]>(
      `${baseUrl}/Proformas`
    );
  }
}

// Mantiene el nombre anterior.
export {
  ProformaService as Proforma
};
