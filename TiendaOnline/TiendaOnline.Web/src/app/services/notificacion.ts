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

// Importa el modelo.
import {
  INotificacion
} from '../model/INotificacion';

// Importa la configuración.
import {
  environment
} from '../../environments/environment';

// Guarda la dirección principal.
const baseUrl =
  environment.apiUrl;

// Datos necesarios para crear una notificación.
export interface NotificacionCrear {

  // Usuario que recibe.
  idUsuario: number;

  // Título mostrado.
  titulo: string;

  // Mensaje enviado.
  mensaje: string;

  // Tipo de notificación.
  tipo: string | null;
}

// Permite utilizar el servicio.
@Injectable({
  providedIn: 'root'
})
export class NotificacionService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Obtiene las notificaciones del cliente.
  listarMisNotificaciones():
    Observable<INotificacion[]> {

    // Consulta las notificaciones propias.
    return this.http.get<INotificacion[]>(
      `${baseUrl}/Notificaciones/mis-notificaciones`
    );
  }

  // Crea una nueva notificación.
  crear(
    datos: NotificacionCrear
  ): Observable<INotificacion> {

    // Envía la notificación.
    return this.http.post<INotificacion>(
      `${baseUrl}/Notificaciones`,
      datos
    );
  }

  // Marca una notificación como leída.
  marcarLeida(
    idNotificacion: number
  ): Observable<void> {

    // Actualiza la notificación.
    return this.http.put<void>(
      `${baseUrl}/Notificaciones/${idNotificacion}/marcar-leida`,
      {}
    );
  }
}

// Mantiene el nombre anterior.
export {
  NotificacionService as Notificacion
};
