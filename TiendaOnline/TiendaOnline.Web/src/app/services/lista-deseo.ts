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

// Importa la configuración.
import {
  environment
} from '../../environments/environment';

// Guarda la dirección principal.
const baseUrl =
  environment.apiUrl;

// Representa una lista de deseos.
export interface IListaDeseo {

  // Identificador de la lista.
  idListaDeseos: number;

  // Usuario propietario.
  idUsuario: number;

  // Fecha de creación.
  fechaCreacion: string;
}

// Permite utilizar el servicio.
@Injectable({
  providedIn: 'root'
})
export class ListaDeseoService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Obtiene todas las listas.
  listar():
    Observable<IListaDeseo[]> {

    // Consulta las listas.
    return this.http.get<IListaDeseo[]>(
      `${baseUrl}/ListaDeseos`
    );
  }

  // Obtiene una lista específica.
  obtener(
    idLista: number
  ): Observable<IListaDeseo> {

    // Consulta la lista.
    return this.http.get<IListaDeseo>(
      `${baseUrl}/ListaDeseos/${idLista}`
    );
  }

  // Crea una lista.
  crear(
    lista: IListaDeseo
  ): Observable<IListaDeseo> {

    // Envía la lista a la API.
    return this.http.post<IListaDeseo>(
      `${baseUrl}/ListaDeseos`,
      lista
    );
  }

  // Modifica una lista.
  modificar(
    lista: IListaDeseo
  ): Observable<void> {

    // Envía los cambios.
    return this.http.put<void>(
      `${baseUrl}/ListaDeseos/${lista.idListaDeseos}`,
      lista
    );
  }

  // Elimina una lista.
  eliminar(
    idLista: number
  ): Observable<void> {

    // Solicita la eliminación.
    return this.http.delete<void>(
      `${baseUrl}/ListaDeseos/${idLista}`
    );
  }
}

// Mantiene el nombre anterior.
export {
  ListaDeseoService as ListaDeseo
};
