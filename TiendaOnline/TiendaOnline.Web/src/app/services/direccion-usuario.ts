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
  IDireccionUsuario
} from '../model/IDireccionUsuario';

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
export class DireccionUsuarioService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Obtiene las direcciones.
  listar():
    Observable<IDireccionUsuario[]> {

    // Consulta las direcciones.
    return this.http.get<IDireccionUsuario[]>(
      `${baseUrl}/DireccionUsuarios`
    );
  }
}

// Mantiene el nombre anterior.
export {
  DireccionUsuarioService as DireccionUsuario
};
