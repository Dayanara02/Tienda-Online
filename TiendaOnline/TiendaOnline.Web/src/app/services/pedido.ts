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

// Importa el modelo de pedido.
import {
  IPedido
} from '../model/IPedido';

// Importa la configuración del ambiente.
import {
  environment
} from '../../environments/environment';

// Guarda la dirección principal de la API.
const baseUrl =
  environment.apiUrl;

// Permite utilizar el servicio.
@Injectable({
  providedIn: 'root'
})
export class PedidoService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Obtiene los pedidos del cliente.
  listarMisPedidos():
    Observable<IPedido[]> {

    // Consulta los pedidos propios.
    return this.http.get<IPedido[]>(
      `${baseUrl}/Pedidos/mis-pedidos`
    );
  }

  // Obtiene un pedido específico.
  obtener(
    idPedido: number
  ): Observable<IPedido> {

    // Consulta el pedido por id.
    return this.http.get<IPedido>(
      `${baseUrl}/Pedidos/${idPedido}`
    );
  }

  // Cancela un pedido.
  cancelar(
    idPedido: number
  ): Observable<any> {

    // Solicita la cancelación.
    return this.http.put<any>(
      `${baseUrl}/Pedidos/${idPedido}/cancelar`,
      {}
    );
  }
}

// Mantiene el nombre usado por los componentes.
export {
  PedidoService as Pedido
};
