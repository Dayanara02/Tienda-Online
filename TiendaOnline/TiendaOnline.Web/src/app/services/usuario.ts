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
  IUsuario
} from '../model/IUsuario';

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
export class UsuarioService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Obtiene todos los usuarios.
  listar():
    Observable<IUsuario[]> {

    // Consulta los usuarios.
    return this.http.get<IUsuario[]>(
      `${baseUrl}/Usuarios`
    );
  }
}

// Mantiene el nombre anterior.
export {
  UsuarioService as Usuario
};
