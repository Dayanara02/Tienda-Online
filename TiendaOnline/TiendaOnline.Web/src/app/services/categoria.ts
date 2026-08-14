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

// Importa el modelo de categoría.
import {
  ICategoria
} from '../model/ICategoria';

// Importa la configuración del ambiente.
import {
  environment
} from '../../environments/environment';

// Guarda la dirección principal de la API.
const baseUrl =
  environment.apiUrl;

// Permite utilizar el servicio en toda la aplicación.
@Injectable({
  providedIn: 'root'
})
export class CategoriaService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Obtiene todas las categorías.
  listar():
    Observable<ICategoria[]> {

    // Realiza una petición GET.
    return this.http.get<ICategoria[]>(
      `${baseUrl}/Categorias`
    );
  }

  // Crea una nueva categoría.
  crear(
    categoria: ICategoria
  ): Observable<ICategoria> {

    // Envía la categoría a la API.
    return this.http.post<ICategoria>(
      `${baseUrl}/Categorias`,
      categoria
    );
  }

  // Modifica una categoría.
  modificar(
    categoria: ICategoria
  ): Observable<void> {

    // Envía los cambios a la API.
    return this.http.put<void>(
      `${baseUrl}/Categorias/${categoria.idCategoria}`,
      categoria
    );
  }

  // Elimina una categoría.
  eliminar(
    idCategoria: number
  ): Observable<void> {

    // Solicita la eliminación.
    return this.http.delete<void>(
      `${baseUrl}/Categorias/${idCategoria}`
    );
  }
}

// Mantiene el nombre utilizado anteriormente.
export {
  CategoriaService as Categoria
};
