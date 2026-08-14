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

// Importa el modelo general de pago.
import {
  IPago
} from '../model/IPago';

// Importa la configuración del ambiente.
import {
  environment
} from '../../environments/environment';

// Guarda la dirección principal.
const baseUrl =
  environment.apiUrl;

// Datos necesarios para pagar un pedido.
export interface PagarPedido {

  // Pedido seleccionado.
  idPedido: number;

  // Método seleccionado.
  idMetodoPago: number;
}

// Representa la respuesta del pago.
export interface RespuestaPago
  extends Pick<
    IPago,
    | 'idPago'
    | 'idPedido'
    | 'metodoPago'
    | 'referencia'
    | 'monto'
    | 'fechaPago'
  > {

  // Mensaje recibido.
  mensaje: string;

  // Estado del pago.
  estadoPago: string;

  // Estado del pedido.
  estadoPedido: string;

  // Indica si se envió el correo.
  correoEnviado?: boolean;

  // Mensaje relacionado con el correo.
  mensajeCorreo?: string;
}

// Permite utilizar el servicio.
@Injectable({
  providedIn: 'root'
})
export class PagoService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Realiza el pago.
  pagar(
    datos: PagarPedido
  ): Observable<RespuestaPago> {

    // Envía el pago al backend.
    return this.http.post<RespuestaPago>(
      `${baseUrl}/Pagos/pagar`,
      datos
    );
  }
}

// Mantiene el nombre utilizado anteriormente.
export {
  PagoService as Pago
};
