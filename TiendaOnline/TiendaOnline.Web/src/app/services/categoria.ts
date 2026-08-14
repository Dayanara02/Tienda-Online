// Permite realizar peticiones HTTP.
import { HttpClient } from '@angular/common/http';

// Permite crear e inyectar servicios.
import {
  inject,
  Injectable
} from '@angular/core';

// Permite trabajar con respuestas asíncronas.
import { Observable } from 'rxjs';

// Importa el modelo de categoría.
import { ICategoria } from '../model/ICategoria';

// Permite utilizar el servicio en toda la aplicación.
@Injectable({
  providedIn: 'root'
})
export class Categoria {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Dirección del controlador Categorias.
  private readonly apiUrl =
    'https://localhost:7196/api/Categorias';

  // Obtiene todas las categorías.
  listar(): Observable<ICategoria[]> {

    // Realiza la consulta GET.
    return this.http.get<ICategoria[]>(
      this.apiUrl
    );
  }

  // Obtiene una categoría por id.
  obtener(
    id: number
  ): Observable<ICategoria> {

    // Consulta una categoría.
    return this.http.get<ICategoria>(
      `${this.apiUrl}/${id}`
    );
  }

  // Crea una categoría.
  crear(
    categoria: ICategoria
  ): Observable<ICategoria> {

    // Envía la categoría a la API.
    return this.http.post<ICategoria>(
      this.apiUrl,
      categoria
    );
  }

  // Modifica una categoría.
  modificar(
    categoria: ICategoria
  ): Observable<void> {

    // Envía los cambios.
    return this.http.put<void>(
      `${this.apiUrl}/${categoria.idCategoria}`,
      categoria
    );
  }

  // Elimina una categoría.
  eliminar(
    id: number
  ): Observable<void> {

    // Solicita la eliminación.
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}
