// Permite realizar peticiones HTTP.
import { HttpClient } from '@angular/common/http';

// Permite crear e inyectar servicios.
import {
  inject,
  Injectable
} from '@angular/core';

// Permite trabajar con respuestas asíncronas.
import { Observable } from 'rxjs';

// Importa el modelo.
import { IMetodoPago } from '../model/IMetodoPago';

// Permite utilizar el servicio.
@Injectable({
  providedIn: 'root'
})
export class MetodoPago {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Dirección del controlador.
  private readonly apiUrl =
    'https://localhost:7196/api/MetodoPagos';

  // Obtiene los métodos disponibles.
  listarDisponibles():
    Observable<IMetodoPago[]> {

    // Consulta los métodos activos.
    return this.http.get<IMetodoPago[]>(
      `${this.apiUrl}/disponibles`
    );
  }
}
