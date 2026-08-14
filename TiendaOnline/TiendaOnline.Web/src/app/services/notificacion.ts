// Permite realizar peticiones HTTP.
import { HttpClient } from '@angular/common/http';

// Permite crear e inyectar servicios.
import {
  inject,
  Injectable
} from '@angular/core';

// Permite trabajar con respuestas asíncronas.
import { Observable } from 'rxjs';

// Importa el modelo de notificación.
import { INotificacion } from '../model/INotificacion';

// Datos para crear una notificación.
export interface NotificacionCrear {

  // Usuario que recibe el mensaje.
  idUsuario: number;

  // Título de la notificación.
  titulo: string;

  // Mensaje enviado.
  mensaje: string;

  // Tipo de notificación.
  tipo: string | null;
}

// Permite usar el servicio en toda la aplicación.
@Injectable({
  providedIn: 'root'
})
export class Notificacion {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Dirección del controlador.
  private readonly apiUrl =
    'https://localhost:7196/api/Notificaciones';

  // Obtiene las notificaciones del cliente.
  listarMisNotificaciones():
    Observable<INotificacion[]> {

    // Consulta las notificaciones propias.
    return this.http.get<INotificacion[]>(
      `${this.apiUrl}/mis-notificaciones`
    );
  }

  // Crea una notificación manual.
  crear(
    datos: NotificacionCrear
  ): Observable<INotificacion> {

    // Envía la notificación.
    return this.http.post<INotificacion>(
      this.apiUrl,
      datos
    );
  }

  // Marca una notificación como leída.
  marcarLeida(
    idNotificacion: number
  ): Observable<void> {

    // Actualiza el estado de lectura.
    return this.http.put<void>(
      `${this.apiUrl}/${idNotificacion}/marcar-leida`,
      {}
    );
  }
}
