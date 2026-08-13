// Permite usar directivas y pipes.
import { CommonModule } from '@angular/common';

// Permite crear y actualizar el componente.
import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

// Permite consultar la API.
import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';

// Permite navegar entre páginas.
import { Router } from '@angular/router';

// Importa iconos de Angular Material.
import { MatIconModule } from '@angular/material/icon';

// Importa botones de PrimeNG.
import { ButtonModule } from 'primeng/button';

// Define una notificación.
interface Notificacion {
  idNotificacion: number;
  titulo: string;
  mensaje: string;
  tipo: string | null;
  fechaCreacion: string;
  leida: boolean;
}

// Configura el componente.
@Component({
  selector: 'app-notificaciones',

  // Permite usar el componente solo.
  standalone: true,

  // Registra los módulos usados.
  imports: [
    CommonModule,
    MatIconModule,
    ButtonModule
  ],

  // Archivo HTML.
  templateUrl: './notificaciones.html',

  // Archivo CSS.
  styleUrl: './notificaciones.css'
})
export class Notificaciones implements OnInit {

  // Guarda la dirección de la API.
  private readonly apiUrl =
    'https://localhost:7196/api/Notificaciones';

  // Guarda las notificaciones.
  notificaciones: Notificacion[] = [];

  // Guarda el nombre del cliente.
  nombreUsuario = 'Cliente';

  // Controla el estado de carga.
  cargando = false;

  // Guarda mensajes de error.
  mensajeError = '';

  // Recibe los servicios necesarios.
  constructor(
    private http: HttpClient,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    // Busca el nombre guardado.
    const nombreGuardado =
      localStorage.getItem(
        'nombreUsuario'
      );

    // Usa el nombre si existe.
    if (nombreGuardado) {
      this.nombreUsuario =
        nombreGuardado;
    }
  }

  // Se ejecuta al abrir la página.
  ngOnInit(): void {

    // Carga las notificaciones.
    this.cargarNotificaciones();
  }

  // Crea los encabezados con JWT.
  private obtenerHeaders(): HttpHeaders {

    // Obtiene el token.
    const token =
      localStorage.getItem(
        'token'
      );

    // Devuelve los encabezados.
    return new HttpHeaders({
      Authorization:
        `Bearer ${token}`
    });
  }

  // Consulta las notificaciones.
  cargarNotificaciones(): void {

    // Activa la carga.
    this.cargando =
      true;

    // Limpia errores.
    this.mensajeError =
      '';

    // Actualiza la pantalla.
    this.cdr.detectChanges();

    // Consulta la API.
    this.http.get<Notificacion[]>(
      `${this.apiUrl}/mis-notificaciones`,
      {
        headers:
          this.obtenerHeaders()
      }
    ).subscribe({

      // Si responde correctamente.
      next: (respuesta) => {

        // Muestra la respuesta en consola.
        console.log(
          'Notificaciones recibidas:',
          respuesta
        );

        // Guarda las notificaciones.
        this.notificaciones =
          Array.isArray(respuesta)
            ? respuesta
            : [];

        // Finaliza la carga.
        this.cargando =
          false;

        // Actualiza la pantalla.
        this.cdr.detectChanges();
      },

      // Si ocurre un error.
      error: (error) => {

        // Muestra el error en consola.
        console.error(
          'Error al cargar notificaciones:',
          error
        );

        // Limpia la lista.
        this.notificaciones =
          [];

        // Muestra el mensaje.
        this.mensajeError =
          'No se pudieron cargar las notificaciones.';

        // Finaliza la carga.
        this.cargando =
          false;

        // Actualiza la pantalla.
        this.cdr.detectChanges();
      }
    });
  }

  // Marca una notificación como leída.
  marcarComoLeida(
    notificacion: Notificacion
  ): void {

    // Evita repetir la petición.
    if (notificacion.leida) {
      return;
    }

    // Llama al endpoint.
    this.http.put(
      `${this.apiUrl}/${notificacion.idNotificacion}/marcar-leida`,
      {},
      {
        headers:
          this.obtenerHeaders()
      }
    ).subscribe({

      // Si funciona.
      next: () => {

        // Cambia el estado.
        notificacion.leida =
          true;

        // Actualiza la pantalla.
        this.cdr.detectChanges();
      },

      // Si falla.
      error: (error) => {

        // Muestra el error.
        console.error(
          'Error al marcar como leída:',
          error
        );
      }
    });
  }

  // Cuenta las notificaciones no leídas.
  get cantidadNoLeidas(): number {

    // Filtra las pendientes.
    return this.notificaciones
      .filter(
        notificacion =>
          !notificacion.leida
      )
      .length;
  }

  // Regresa al Dashboard.
  irAlInicio(): void {
    this.router.navigate([
      '/dashboard'
    ]);
  }

  // Abre Productos.
  irAProductos(): void {
    this.router.navigate([
      '/productos'
    ]);
  }

  // Abre Carrito.
  irAlCarrito(): void {
    this.router.navigate([
      '/carrito'
    ]);
  }

  // Abre Favoritos.
  irAFavoritos(): void {
    this.router.navigate([
      '/lista-deseos'
    ]);
  }

  // Abre Mis pedidos.
  irAMisPedidos(): void {
    this.router.navigate([
      '/mis-pedidos'
    ]);
  }

  // Abre Descuentos.
  irADescuentos(): void {
    this.router.navigate([
      '/descuentos'
    ]);
  }

  // Abre el Perfil.
  irAPerfil(): void {
    this.router.navigate([
      '/perfil'
    ]);
  }

  // Cierra la sesión.
  cerrarSesion(): void {

    // Elimina el token.
    localStorage.removeItem(
      'token'
    );

    // Elimina el rol.
    localStorage.removeItem(
      'rol'
    );

    // Elimina el id.
    localStorage.removeItem(
      'idUsuario'
    );

    // Elimina el nombre.
    localStorage.removeItem(
      'nombreUsuario'
    );

    // Elimina el correo.
    localStorage.removeItem(
      'correoUsuario'
    );

    // Regresa al Login.
    this.router.navigate([
      '/login'
    ]);
  }
}
