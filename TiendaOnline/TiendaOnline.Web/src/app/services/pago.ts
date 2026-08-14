// Permite realizar peticiones HTTP.
import { HttpClient } from '@angular/common/http';

// Permite crear e inyectar servicios.
import {
  inject,
  Injectable
} from '@angular/core';

// Permite trabajar con respuestas asíncronas.
import { Observable } from 'rxjs';

// Guarda los datos necesarios para pagar.
export interface PagarPedido {

  // Pedido seleccionado.
  idPedido: number;

  // Método seleccionado.
  idMetodoPago: number;
}

// Representa la respuesta del pago.
export interface RespuestaPago {

  // Mensaje recibido.
  mensaje: string;

  // Pedido pagado.
  idPedido: number;

  // Pago creado.
  idPago: number;

  // Estado del pago.
  estadoPago: string;

  // Estado del pedido.
  estadoPedido: string;

  // Método utilizado.
  metodoPago: string;

  // Monto pagado.
  monto: number;

  // Referencia generada.
  referencia: string;

  // Fecha del pago.
  fechaPago: string;

  // Indica si se envió el correo.
  correoEnviado?: boolean;

  // Mensaje relacionado con el correo.
  mensajeCorreo?: string;
}

// Permite usar el servicio.
@Injectable({
  providedIn: 'root'
})
export class Pago {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Dirección del controlador Pagos.
  private readonly apiUrl =
    'https://localhost:7196/api/Pagos';

  // Realiza el pago.
  pagar(
    datos: PagarPedido
  ): Observable<RespuestaPago> {

    // Envía el pago a la API.
    return this.http.post<RespuestaPago>(
      `${this.apiUrl}/pagar`,
      datos
    );
  }
}
