// Permite usar directivas comunes.
import { CommonModule } from '@angular/common';

// Importa herramientas del componente.
import {
  Component,
  inject
} from '@angular/core';

// Permite utilizar ngModel.
import { FormsModule } from '@angular/forms';

// Permite navegar entre pantallas.
import { Router } from '@angular/router';

// Importa el modelo de categoría.
import { ICategoria } from '../../model/ICategoria';

// Importa el servicio de categorías.
import { Categoria } from '../../services/categoria';

// Configura el componente.
@Component({
  // Nombre del componente.
  selector: 'app-gestion-categorias',

  // Indica que es independiente.
  standalone: true,

  // Módulos utilizados.
  imports: [
    CommonModule,
    FormsModule
  ],

  // Archivo HTML.
  templateUrl:
    './gestion-categorias.html',

  // Archivo CSS.
  styleUrl:
    './gestion-categorias.css'
})
export class GestionCategorias {

  // Inyecta el servicio de categorías.
  private readonly categoriaService =
    inject(Categoria);

  // Inyecta Router.
  private readonly router =
    inject(Router);

  // Guarda las categorías.
  categorias: ICategoria[] = [];

  // Guarda la categoría seleccionada.
  categoriaSeleccionada: ICategoria =
    this.crearCategoriaVacia();

  // Indica si se está editando.
  editando = false;

  // Indica si está cargando.
  cargando = false;

  // Guarda mensajes de error.
  mensajeError = '';

  // Guarda mensajes exitosos.
  mensajeExito = '';

  // Carga las categorías al iniciar.
  constructor() {
    this.listarCategorias();
  }

  // Crea una categoría vacía.
  private crearCategoriaVacia():
    ICategoria {

    // Devuelve los valores iniciales.
    return {
      idCategoria: 0,
      idFamilia: 0,
      nombre: '',
      descripcion: '',
      estado: true
    };
  }

  // Obtiene todas las categorías.
  listarCategorias(): void {

    // Activa la carga.
    this.cargando = true;

    // Limpia errores.
    this.mensajeError = '';

    // Consulta el servicio.
    this.categoriaService
      .listar()
      .subscribe({

        // Guarda las categorías.
        next: (respuesta) => {

          // Guarda la lista.
          this.categorias =
            respuesta ?? [];

          // Finaliza la carga.
          this.cargando =
            false;
        },

        // Maneja errores.
        error: (error) => {

          // Muestra el error.
          console.error(
            'Error al cargar categorías:',
            error
          );

          // Muestra un mensaje.
          this.mensajeError =
            'No se pudieron cargar las categorías.';

          // Finaliza la carga.
          this.cargando =
            false;
        }
      });
  }

  // Guarda o modifica una categoría.
  guardarCategoria(): void {

    // Limpia mensajes.
    this.mensajeError = '';
    this.mensajeExito = '';

    // Valida la familia.
    if (
      this.categoriaSeleccionada
        .idFamilia <= 0
    ) {
      this.mensajeError =
        'Debe indicar una familia válida.';

      return;
    }

    // Valida el nombre.
    if (
      !this.categoriaSeleccionada
        .nombre
        .trim()
    ) {
      this.mensajeError =
        'Debe escribir el nombre de la categoría.';

      return;
    }

    // Comprueba si está editando.
    if (this.editando) {

      // Modifica la categoría.
      this.modificarCategoria();

      return;
    }

    // Crea la categoría.
    this.crearCategoria();
  }

  // Crea una categoría.
  private crearCategoria(): void {

    // Envía la categoría.
    this.categoriaService
      .crear(
        this.categoriaSeleccionada
      )
      .subscribe({

        // Se ejecuta si funciona.
        next: () => {

          // Muestra éxito.
          this.mensajeExito =
            'Categoría creada correctamente.';

          // Limpia el formulario.
          this.limpiarFormulario();

          // Actualiza la lista.
          this.listarCategorias();
        },

        // Maneja errores.
        error: (error) => {

          // Muestra el error.
          console.error(
            'Error al crear categoría:',
            error
          );

          // Muestra mensaje.
          this.mensajeError =
            'No se pudo crear la categoría.';
        }
      });
  }

  // Selecciona una categoría.
  seleccionarCategoria(
    categoria: ICategoria
  ): void {

    // Copia los datos.
    this.categoriaSeleccionada = {
      ...categoria
    };

    // Activa edición.
    this.editando =
      true;

    // Limpia mensajes.
    this.mensajeError = '';
    this.mensajeExito = '';
  }

  // Modifica una categoría.
  private modificarCategoria(): void {

    // Envía los cambios.
    this.categoriaService
      .modificar(
        this.categoriaSeleccionada
      )
      .subscribe({

        // Se ejecuta si funciona.
        next: () => {

          // Muestra éxito.
          this.mensajeExito =
            'Categoría modificada correctamente.';

          // Limpia el formulario.
          this.limpiarFormulario();

          // Actualiza la lista.
          this.listarCategorias();
        },

        // Maneja errores.
        error: (error) => {

          // Muestra el error.
          console.error(
            'Error al modificar categoría:',
            error
          );

          // Muestra mensaje.
          this.mensajeError =
            'No se pudo modificar la categoría.';
        }
      });
  }

  // Elimina una categoría.
  eliminarCategoria(
    categoria: ICategoria
  ): void {

    // Solicita confirmación.
    const confirmar =
      confirm(
        `¿Desea eliminar la categoría ${categoria.nombre}?`
      );

    // Detiene si cancela.
    if (!confirmar) {
      return;
    }

    // Limpia mensajes.
    this.mensajeError = '';
    this.mensajeExito = '';

    // Solicita la eliminación.
    this.categoriaService
      .eliminar(
        categoria.idCategoria
      )
      .subscribe({

        // Se ejecuta si funciona.
        next: () => {

          // Muestra éxito.
          this.mensajeExito =
            'Categoría eliminada correctamente.';

          // Actualiza la lista.
          this.listarCategorias();
        },

        // Maneja errores.
        error: (error) => {

          // Muestra el error.
          console.error(
            'Error al eliminar categoría:',
            error
          );

          // Muestra mensaje.
          this.mensajeError =
            'No se pudo eliminar la categoría.';
        }
      });
  }

  // Limpia el formulario.
  limpiarFormulario(): void {

    // Crea una categoría vacía.
    this.categoriaSeleccionada =
      this.crearCategoriaVacia();

    // Desactiva edición.
    this.editando =
      false;
  }

  // Regresa al dashboard.
  volver(): void {

    // Navega al administrador.
    this.router.navigate([
      '/admin-dashboard'
    ]);
  }
}
