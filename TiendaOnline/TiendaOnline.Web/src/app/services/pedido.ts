// Permite realizar peticiones HTTP.
import { HttpClient } from '@angular/common/http';

// Permite crear e inyectar servicios.
import {
  inject,
  Injectable
} from '@angular/core';

// Permite manejar respuestas asíncronas.
import { Observable } from 'rxjs';

// Importa el modelo de pedido.
import { IPedido } from '../model/IPedido';

// Permite usar el servicio en toda la aplicación.
@Injectable({
  providedIn: 'root'
})
export class Pedido {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Dirección del controlador Pedidos.
  private readonly apiUrl =
    'https://localhost:7196/api/Pedidos';

  // Obtiene los pedidos del cliente.
  listarMisPedidos():
    Observable<IPedido[]> {

    // Consulta los pedidos del usuario.
    return this.http.get<IPedido[]>(
      `${this.apiUrl}/mis-pedidos`
    );
  }

  // Obtiene un pedido por id.
  obtener(
    idPedido: number
  ): Observable<IPedido> {

    // Consulta el pedido.
    return this.http.get<IPedido>(
      `${this.apiUrl}/${idPedido}`
    );
  }

  // Cancela un pedido.
  cancelar(
    idPedido: number
  ): Observable<any> {

    // Envía la cancelación.
    return this.http.put(
      `${this.apiUrl}/${idPedido}/cancelar`,
      {}
    );
  }
}
