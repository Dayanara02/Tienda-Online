// Importa CommonModule para usar *ngIf, *ngFor y pipes.
import { CommonModule } from '@angular/common';

// Importa las herramientas principales del componente.
import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

// Importa las herramientas para leer la URL y navegar.
import {
  ActivatedRoute,
  Router
} from '@angular/router';

// Importa las herramientas necesarias para consultar la API.
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

// Representa cada producto incluido dentro del pedido.
interface DetalleProducto {
  idDetallePedido: number;
  idProducto: number;
  nombreProducto: string;
  cantidad: number;
  precioUnitario: number;
  descuento: number;
  impuesto: number;
  subtotal: number;
}

// Representa el detalle completo recibido desde la API.
interface PedidoDetalle {
  idPedido: number;
  idUsuario: number;
  fechaPedido: string;
  estado: string;
  subtotal: number;
  impuesto: number;
  descuento: number;
  total: number;
  direccionEntrega: string | null;
  idEstadoPedido: number | null;
  estadoPago: string;
  metodoPago: string | null;
  fechaPago: string | null;
  puedePagar: boolean;
  puedeCancelar?: boolean;
  detalles: DetalleProducto[];
}

// Configura la pantalla Detalle de Pedido.
@Component({
  selector: 'app-detalle-pedido',

  // Indica que el componente funciona de forma independiente.
  standalone: true,

  // Registra los módulos utilizados en el HTML.
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    ButtonModule
  ],

  // Define los archivos visuales del componente.
  templateUrl: './detalle-pedido.html',
  styleUrl: './detalle-pedido.css'
})
export class DetallePedido {

  // Guarda la información completa del pedido.
  pedido: PedidoDetalle | null = null;

  // Guarda el identificador recibido desde la URL.
  idPedido = 0;

  // Controla el estado de carga.
  cargando = true;

  // Guarda los mensajes de error.
  mensajeError = '';

  // Guarda la dirección principal del controlador Pedidos.
  private readonly apiUrl =
    'https://localhost:7196/api/Pedidos';

  // Inyecta los servicios utilizados por la pantalla.
  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private router: Router,
    private changeDetector: ChangeDetectorRef
  ) {
    // Obtiene el pedido apenas se abre la pantalla.
    this.obtenerIdPedido();
  }

  // Obtiene el identificador del pedido desde la URL.
  obtenerIdPedido(): void {
    // Lee el parámetro llamado id.
    const idTexto =
      this.route.snapshot.paramMap.get(
        'id'
      );

    // Convierte el parámetro recibido a número.
    this.idPedido =
      Number(idTexto);

    // Comprueba que el identificador sea válido.
    if (
      !this.idPedido ||
      this.idPedido <= 0
    ) {
      // Muestra un mensaje si el id es incorrecto.
      this.mensajeError =
        'El pedido seleccionado no es válido.';

      // Finaliza la carga.
      this.cargando =
        false;

      // Actualiza la pantalla.
      this.changeDetector.detectChanges();

      return;
    }

    // Consulta el pedido si el identificador es válido.
    this.cargarPedido();
  }

  // Consulta el detalle completo del pedido.
  cargarPedido(): void {
    // Activa el estado de carga.
    this.cargando =
      true;

    // Limpia errores anteriores.
    this.mensajeError =
      '';

    // Obtiene el token del usuario conectado.
    const token =
      localStorage.getItem(
        'token'
      );

    // Evita consultar la API si no existe una sesión.
    if (!token) {
      // Muestra el mensaje correspondiente.
      this.mensajeError =
        'No existe una sesión activa.';

      // Finaliza la carga.
      this.cargando =
        false;

      // Actualiza la pantalla.
      this.changeDetector.detectChanges();

      return;
    }

    // Crea el encabezado con el token JWT.
    const headers =
      new HttpHeaders({
        Authorization:
          `Bearer ${token}`
      });

    // Consulta el pedido seleccionado.
    this.http.get<PedidoDetalle>(
      `${this.apiUrl}/${this.idPedido}`,
      {
        headers
      }
    ).subscribe({

      // Se ejecuta cuando la API responde correctamente.
      next: (
        respuesta: PedidoDetalle
      ) => {
        // Guarda la información recibida.
        this.pedido =
          respuesta;

        // Finaliza la carga.
        this.cargando =
          false;

        // Actualiza inmediatamente la pantalla.
        this.changeDetector.detectChanges();
      },

      // Se ejecuta cuando ocurre un error.
      error: (
        error: HttpErrorResponse
      ) => {
        // Muestra el error durante las pruebas.
        console.error(
          'Error al cargar el detalle del pedido:',
          error
        );

        // Comprueba si la sesión no es válida.
        if (error.status === 401) {
          this.mensajeError =
            'La sesión no es válida. Inicia sesión nuevamente.';
        }

        // Comprueba si el pedido pertenece a otro usuario.
        else if (error.status === 403) {
          this.mensajeError =
            'No tienes permiso para consultar este pedido.';
        }

        // Comprueba si el pedido no existe.
        else if (error.status === 404) {
          this.mensajeError =
            'El pedido no existe.';
        }

        // Comprueba si no existe conexión con la API.
        else if (error.status === 0) {
          this.mensajeError =
            'No se pudo conectar con el servidor.';
        }

        // Maneja cualquier otro error.
        else {
          this.mensajeError =
            'No se pudo cargar el detalle del pedido.';
        }

        // Finaliza la carga.
        this.cargando =
          false;

        // Actualiza la pantalla.
        this.changeDetector.detectChanges();
      }
    });
  }

  // Indica si el pedido todavía puede pagarse.
  get puedePagarPedido(): boolean {
    // Comprueba que exista el pedido y que la API permita pagarlo.
    return (
      this.pedido !== null &&
      this.pedido.puedePagar
    );
  }

  // Indica si el pedido ya está pagado.
  get pedidoPagado(): boolean {
    // Comprueba el estado de pago recibido desde la API.
    return (
      this.pedido?.estadoPago
        ?.toLowerCase() ===
      'pagado'
    );
  }

  // Indica si el pedido está cancelado.
  get pedidoCancelado(): boolean {
    // Comprueba el estado general y el estado de pago.
    return (
      this.pedido?.estado
        ?.toLowerCase() ===
      'cancelado' ||
      this.pedido?.estadoPago
        ?.toLowerCase() ===
      'cancelado'
    );
  }

  // Calcula la base utilizada antes de aplicar el impuesto.
  get baseImponible(): number {
    // Devuelve cero si todavía no existe pedido.
    if (!this.pedido) {
      return 0;
    }

    // Resta el descuento al subtotal.
    return (
      Number(this.pedido.subtotal) -
      Number(this.pedido.descuento)
    );
  }

  // Abre la pantalla para pagar el pedido.
  pagarPedido(): void {
    // Comprueba que exista un pedido.
    if (!this.pedido) {
      return;
    }

    // Evita pagar si el backend no lo permite.
    if (!this.pedido.puedePagar) {
      return;
    }

    // Navega hacia la pantalla de pago.
    this.router.navigate([
      '/pago-pedido',
      this.pedido.idPedido
    ]);
  }

  // Regresa al historial de pedidos.
  volverMisPedidos(): void {
    // Navega hacia Mis Pedidos.
    this.router.navigate([
      '/mis-pedidos'
    ]);
  }

  // Regresa al Dashboard del cliente.
  volverInicio(): void {
    // Navega hacia el Dashboard.
    this.router.navigate([
      '/dashboard'
    ]);
  }
}
