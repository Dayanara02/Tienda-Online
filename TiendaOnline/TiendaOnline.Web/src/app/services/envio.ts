// Permite realizar peticiones HTTP.
import { HttpClient } from '@angular/common/http';

// Permite crear e inyectar servicios.
import {
  inject,
  Injectable
} from '@angular/core';

// Permite manejar respuestas asíncronas.
import { Observable } from 'rxjs';

// Importa el modelo de envío.
import { IEnvio } from '../model/IEnvio';

// Datos necesarios para guardar un envío.
export interface EnvioGuardar {

  // Identificador del envío.
  idEnvio: number;

  // Pedido relacionado.
  idPedido: number;

  // Dirección relacionada.
  idDireccion: number;

  // Empresa encargada.
  empresaEnvio: string | null;

  // Número de seguimiento.
  numeroSeguimiento: string | null;

  // Fecha de envío.
  fechaEnvio: string | null;

  // Fecha de entrega.
  fechaEntrega: string | null;

  // Estado actual.
  estado: string;
}

// Permite usar el servicio.
@Injectable({
  providedIn: 'root'
})
export class Envio {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Dirección del controlador.
  private readonly apiUrl =
    'https://localhost:7196/api/Envios';

  // Obtiene el envío de un pedido.
  obtenerPorPedido(
    idPedido: number
  ): Observable<IEnvio> {

    // Consulta el seguimiento.
    return this.http.get<IEnvio>(
      `${this.apiUrl}/pedido/${idPedido}`
    );
  }

  // Obtiene todos los envíos.
  listar():
    Observable<IEnvio[]> {

    // Consulta todos los envíos.
    return this.http.get<IEnvio[]>(
      this.apiUrl
    );
  }

  // Crea un nuevo envío.
  crear(
    envio: EnvioGuardar
  ): Observable<IEnvio> {

    // Envía el nuevo registro.
    return this.http.post<IEnvio>(
      this.apiUrl,
      envio
    );
  }

  // Actualiza un envío.
  modificar(
    envio: EnvioGuardar
  ): Observable<void> {

    // Envía los cambios.
    return this.http.put<void>(
      `${this.apiUrl}/${envio.idEnvio}`,
      envio
    );
  }
}
