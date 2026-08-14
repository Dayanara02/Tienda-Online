// Permite crear e inyectar servicios.
import {
  inject,
  Injectable
} from '@angular/core';

// Permite realizar peticiones HTTP.
import { HttpClient } from '@angular/common/http';

// Permite manejar respuestas asíncronas.
import { Observable } from 'rxjs';

// Importa el modelo de producto.
import { IProducto } from '../model/IProducto';

// Importa la configuración del ambiente.
import {
  environment
} from '../../environments/environment';

// Guarda la dirección principal de la API.
const baseUrl =
  environment.apiUrl;

// Permite usar el servicio en toda la aplicación.
@Injectable({
  providedIn: 'root',
})
export class ProductoService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Obtiene todos los productos.
  listar():
    Observable<IProducto[]> {

    // Consulta el controlador Productos.
    return this.http.get<IProducto[]>(
      `${baseUrl}/Productos`
    );
  }

  // Obtiene un producto por id.
  obtener(
    id: number
  ): Observable<IProducto> {

    // Consulta un producto.
    return this.http.get<IProducto>(
      `${baseUrl}/Productos/${id}`
    );
  }

  // Crea un producto.
  insertar(
    producto: IProducto
  ): Observable<IProducto> {

    // Envía el producto a la API.
    return this.http.post<IProducto>(
      `${baseUrl}/Productos`,
      producto
    );
  }

  // Modifica un producto.
  modificar(
    producto: IProducto
  ): Observable<void> {

    // Envía los cambios.
    return this.http.put<void>(
      `${baseUrl}/Productos/${producto.idProducto}`,
      producto
    );
  }

  // Elimina un producto.
  eliminar(
    id: number
  ): Observable<void> {

    // Solicita la eliminación.
    return this.http.delete<void>(
      `${baseUrl}/Productos/${id}`
    );
  }
}

// Permite utilizar el nombre Producto.
export {
  ProductoService as Producto
};
