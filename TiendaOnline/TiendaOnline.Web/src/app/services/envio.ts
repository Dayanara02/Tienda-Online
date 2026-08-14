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

// Importa el modelo de envío.
import {
  IEnvio
} from '../model/IEnvio';

// Importa la configuración.
import {
  environment
} from '../../environments/environment';

// Guarda la dirección principal.
const baseUrl =
  environment.apiUrl;

// Representa los datos de un envío.
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

// Permite utilizar el servicio.
@Injectable({
  providedIn: 'root'
})
export class EnvioService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Obtiene el seguimiento de un pedido.
  obtenerPorPedido(
    idPedido: number
  ): Observable<IEnvio> {

    // Consulta el envío relacionado.
    return this.http.get<IEnvio>(
      `${baseUrl}/Envios/pedido/${idPedido}`
    );
  }

  // Obtiene todos los envíos.
  listar():
    Observable<IEnvio[]> {

    // Consulta los registros.
    return this.http.get<IEnvio[]>(
      `${baseUrl}/Envios`
    );
  }

  // Crea un nuevo envío.
  crear(
    envio: EnvioGuardar
  ): Observable<IEnvio> {

    // Envía el registro a la API.
    return this.http.post<IEnvio>(
      `${baseUrl}/Envios`,
      envio
    );
  }

  // Modifica un envío.
  modificar(
    envio: EnvioGuardar
  ): Observable<void> {

    // Envía los cambios.
    return this.http.put<void>(
      `${baseUrl}/Envios/${envio.idEnvio}`,
      envio
    );
  }
}

// Mantiene el nombre anterior.
export {
  EnvioService as Envio
};
