// Permite usar directivas comunes.
import { CommonModule } from '@angular/common';

// Importa herramientas del componente.
import {
  Component,
  inject
} from '@angular/core';

// Importa el modelo.
import {
  IMovimientoInventario
} from '../../model/IMovimientoInventario';

// Importa el servicio.
import {
  MovimientoInventario
} from '../../services/movimiento-inventario';

// Configura el componente.
@Component({
  // Nombre del componente.
  selector: 'app-movimientos-inventario',

  // Indica que es independiente.
  standalone: true,

  // Módulos utilizados.
  imports: [
    CommonModule
  ],

  // Archivo HTML.
  templateUrl:
    './movimientos-inventario.html',

  // Archivo CSS.
  styleUrl:
    './movimientos-inventario.css'
})
export class MovimientosInventario {

  // Inyecta el servicio.
  private readonly movimientoService =
    inject(MovimientoInventario);

  // Guarda los movimientos.
  movimientos:
    IMovimientoInventario[] = [];

  // Indica si está cargando.
  cargando = false;

  // Guarda mensajes de error.
  mensajeError = '';

  // Carga los datos al iniciar.
  constructor() {

    // Consulta los movimientos.
    this.listarMovimientos();
  }

  // Obtiene los movimientos.
  listarMovimientos(): void {

    // Activa la carga.
    this.cargando = true;

    // Limpia errores.
    this.mensajeError = '';

    // Consulta el servicio.
    this.movimientoService
      .listar()
      .subscribe({

        // Guarda los resultados.
        next: (respuesta) => {

          // Guarda la lista.
          this.movimientos =
            respuesta ?? [];

          // Finaliza la carga.
          this.cargando =
            false;
        },

        // Maneja errores.
        error: (error) => {

          // Muestra el error.
          console.error(
            'Error al cargar movimientos:',
            error
          );

          // Muestra un mensaje.
          this.mensajeError =
            'No se pudieron cargar los movimientos de inventario.';

          // Finaliza la carga.
          this.cargando =
            false;
        }
      });
  }
}
