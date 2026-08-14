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

// Representa los datos de una evaluación.
export interface EvaluacionProductoCrear {

  // Producto evaluado.
  idProducto: number;

  // Calificación asignada.
  calificacion: number;

  // Comentario opcional.
  comentario: string | null;
}

// Permite utilizar el servicio.
@Injectable({
  providedIn: 'root'
})
export class EvaluacionProductoService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Crea una evaluación.
  crear(
    datos: EvaluacionProductoCrear
  ): Observable<any> {

    // Envía la evaluación.
    return this.http.post<any>(
      `${baseUrl}/EvaluacionProductos`,
      datos
    );
  }
}

// Mantiene el nombre anterior.
export {
  EvaluacionProductoService as EvaluacionProducto
};
