// Permite usar directivas como *ngIf.
import { CommonModule } from '@angular/common';

// Importa herramientas principales de Angular.
import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

// Permite leer el id del pedido desde la URL y navegar.
import {
  ActivatedRoute,
  Router
} from '@angular/router';

// Permite consultar el backend.
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders
} from '@angular/common/http';

// Importa iconos de Angular Material.
import { MatIconModule } from '@angular/material/icon';

// Importa botones de Angular Material.
import { MatButtonModule } from '@angular/material/button';

// Importa botones de PrimeNG.
import { ButtonModule } from 'primeng/button';

// Representa la información del envío que devuelve la API.
interface EnvioSeguimiento {
  idEnvio: number;
  idPedido: number;
  empresaEnvio?: string | null;
  numeroSeguimiento?: string | null;
  fechaEnvio?: string | null;
  fechaEntrega?: string | null;
  estado: string;
  idDireccion: number;
  direccion?: string | null;
  provincia?: string | null;
  canton?: string | null;
  distrito?: string | null;
}

@Component({
  selector: 'app-seguimiento-pedido',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    ButtonModule
  ],
  templateUrl: './seguimiento-pedido.html',
  styleUrl: './seguimiento-pedido.css'
})
export class SeguimientoPedido implements OnInit {

  // Guarda el identificador del pedido recibido por la URL.
  idPedido = 0;

  // Guarda la información del envío.
  envio: EnvioSeguimiento | null = null;

  // Controla el mensaje de carga.
  cargando = true;

  // Guarda cualquier error mostrado al cliente.
  mensajeError = '';

  // Dirección principal del controlador de envíos.
  private readonly apiUrl =
    'https://localhost:7196/api/Envios';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private cd: ChangeDetectorRef
  ) { }

  // Se ejecuta al abrir la pantalla.
  ngOnInit(): void {
    // Obtiene el idPedido desde la URL.
    this.idPedido =
      Number(
        this.route.snapshot.paramMap.get('id')
      );

    // Valida que el identificador sea correcto.
    if (!this.idPedido) {
      this.cargando = false;
      this.mensajeError =
        'El pedido solicitado no es válido.';
      return;
    }

    // Consulta el seguimiento del pedido.
    this.cargarSeguimiento();
  }

  // Indica si el envío está pendiente.
  get estadoPendiente(): boolean {
    return (
      this.envio?.estado
        ?.toLowerCase() ===
      'pendiente'
    );
  }

  // Indica si el envío ya salió.
  get estadoEnviado(): boolean {
    return (
      this.envio?.estado
        ?.toLowerCase() ===
      'enviado'
    );
  }

  // Indica si el pedido fue entregado.
  get estadoEntregado(): boolean {
    return (
      this.envio?.estado
        ?.toLowerCase() ===
      'entregado'
    );
  }

  // Primer paso del seguimiento.
  get pedidoPreparado(): boolean {
    return this.envio !== null;
  }

  // Segundo paso del seguimiento.
  get pedidoEnviado(): boolean {
    return (
      this.estadoEnviado ||
      this.estadoEntregado
    );
  }

  // Tercer paso del seguimiento.
  get pedidoEntregado(): boolean {
    return this.estadoEntregado;
  }

  // Construye la dirección completa mostrada al cliente.
  get direccionCompleta(): string {
    if (!this.envio) {
      return '';
    }

    // La dirección principal viene del pedido original.
    const partes = [
      this.envio.provincia,
      this.envio.canton,
      this.envio.distrito,
      this.envio.direccion
    ];

    return partes
      .filter(
        parte =>
          !!parte &&
          parte.trim() !== ''
      )
      .join(', ');
  }

  // Consulta el envío del pedido actual.
  cargarSeguimiento(): void {
    this.cargando = true;
    this.mensajeError = '';

    // Obtiene el token del usuario.
    const token =
      localStorage.getItem('token') ??
      localStorage.getItem('authToken') ??
      '';

    // Agrega el token a la solicitud.
    const headers =
      new HttpHeaders({
        Authorization:
          `Bearer ${token}`
      });

    this.http
      .get<EnvioSeguimiento>(
        `${this.apiUrl}/pedido/${this.idPedido}`,
        { headers }
      )
      .subscribe({
        next: respuesta => {
          // Guarda el seguimiento recibido.
          this.envio = respuesta;

          this.cargando = false;

          // Actualiza la pantalla.
          this.cd.detectChanges();
        },

        error: (
          error: HttpErrorResponse
        ) => {
          this.cargando = false;

          // Sesión no válida.
          if (error.status === 401) {
            this.mensajeError =
              'Debe iniciar sesión nuevamente.';
          }

          // El pedido no pertenece al cliente.
          else if (error.status === 403) {
            this.mensajeError =
              'No tiene permiso para consultar este pedido.';
          }

          // Todavía no existe un envío.
          else if (error.status === 404) {
            this.mensajeError =
              typeof error.error === 'string'
                ? error.error
                : 'Este pedido todavía no tiene un envío registrado.';
          }

          // API apagada.
          else if (error.status === 0) {
            this.mensajeError =
              'No se pudo conectar con la API.';
          }

          // Error general.
          else {
            this.mensajeError =
              'No fue posible consultar el seguimiento.';
          }

          this.cd.detectChanges();
        }
      });
  }

  // Regresa al detalle del pedido.
  volverDetalle(): void {
    this.router.navigate([
      '/detalle-pedido',
      this.idPedido
    ]);
  }

  // Regresa al historial de pedidos.
  volverPedidos(): void {
    this.router.navigate([
      '/mis-pedidos'
    ]);
  }

  // Regresa al inicio.
  volverInicio(): void {
    this.router.navigate([
      '/dashboard'
    ]);
  }
}
