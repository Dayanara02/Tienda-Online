// Importa herramientas comunes de Angular.
import { CommonModule } from '@angular/common';

// Importa herramientas HTTP para consultar la API y enviar el token.
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders
} from '@angular/common/http';

// Importa Component y ChangeDetectorRef para crear y actualizar la vista.
import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

// Importa Router para navegar entre pantallas.
import { Router } from '@angular/router';

// Importa botones de PrimeNG.
import { ButtonModule } from 'primeng/button';

// Importa iconos de Angular Material.
import { MatIconModule } from '@angular/material/icon';

// Importa botones de Angular Material.
import { MatButtonModule } from '@angular/material/button';

// Importa tooltips de Angular Material.
import { MatTooltipModule } from '@angular/material/tooltip';

// Representa la estructura de un pedido recibido desde la API.
interface Pedido {
  // Identificador del pedido.
  idPedido: number;

  // Fecha del pedido.
  fechaPedido: string;

  // Estado general del pedido.
  estado: string;

  // Subtotal de la compra.
  subtotal: number;

  // Impuesto aplicado.
  impuesto: number;

  // Descuento aplicado.
  descuento: number;

  // Total final.
  total: number;

  // Dirección de entrega.
  direccionEntrega: string | null;

  // Estado del pago.
  estadoPago?: string | null;
}

// Configura el componente Mis Pedidos.
@Component({
  selector: 'app-mis-pedidos',
  standalone: true,

  // Registra los módulos utilizados en el HTML.
  imports: [
    CommonModule,
    ButtonModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule
  ],

  // Define los archivos de vista y estilos.
  templateUrl: './mis-pedidos.html',
  styleUrl: './mis-pedidos.css'
})
export class MisPedidos {

  // Guarda los pedidos del cliente.
  pedidos: Pedido[] = [];

  // Indica si se están cargando los pedidos.
  cargando = true;

  // Indica si se está cancelando un pedido.
  cancelandoPedido = false;

  // Guarda el pedido que se está cancelando.
  idPedidoCancelando: number | null = null;

  // Guarda mensajes de error.
  mensajeError = '';

  // Guarda mensajes de éxito.
  mensajeExito = '';

  // Dirección base del controlador Pedidos.
  private readonly apiUrl =
    'https://localhost:7196/api/Pedidos';

  // Inyecta HttpClient, Router y ChangeDetectorRef.
  constructor(
    private http: HttpClient,
    private router: Router,
    private changeDetector: ChangeDetectorRef
  ) {
    // Carga los pedidos al abrir la pantalla.
    this.cargarPedidos();
  }

  // Crea los encabezados con el token JWT.
  private obtenerHeaders(): HttpHeaders | null {

    // Obtiene el token guardado.
    const token =
      localStorage.getItem('token');

    // Devuelve null si no existe token.
    if (!token) {
      return null;
    }

    // Devuelve los encabezados de autorización.
    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }

  // Consulta los pedidos del cliente autenticado.
  cargarPedidos(): void {

    // Activa el estado de carga.
    this.cargando = true;

    // Limpia errores anteriores.
    this.mensajeError = '';

    // Obtiene los encabezados.
    const headers =
      this.obtenerHeaders();

    // Verifica que exista una sesión activa.
    if (!headers) {
      this.mensajeError =
        'No existe una sesión activa. Inicia sesión nuevamente.';

      this.cargando = false;
      this.changeDetector.detectChanges();
      return;
    }

    // Consulta los pedidos desde la API.
    this.http
      .get<Pedido[]>(
        `${this.apiUrl}/mis-pedidos`,
        {
          headers,
          timeout: 10000
        }
      )
      .subscribe({

        // Guarda los pedidos recibidos.
        next: (respuesta: Pedido[]) => {
          this.pedidos =
            respuesta ?? [];

          this.cargando = false;
          this.changeDetector.detectChanges();
        },

        // Maneja los errores de la consulta.
        error: (error: HttpErrorResponse) => {
          console.error(
            'Error al cargar pedidos:',
            error
          );

          if (error.status === 401) {
            this.mensajeError =
              'La sesión no es válida. Inicia sesión nuevamente.';
          } else if (error.status === 403) {
            this.mensajeError =
              'Tu usuario no tiene permiso para consultar estos pedidos.';
          } else if (error.status === 0) {
            this.mensajeError =
              'No se pudo completar la conexión con la API.';
          } else {
            this.mensajeError =
              'No se pudieron cargar los pedidos.';
          }

          this.cargando = false;
          this.changeDetector.detectChanges();
        }
      });
  }

  // Indica si un pedido cumple las reglas para cancelarse.
  puedeCancelar(
    pedido: Pedido
  ): boolean {

    // Normaliza el estado del pedido.
    const estado =
      pedido.estado
        ?.trim()
        .toLowerCase();

    // Normaliza el estado del pago.
    const estadoPago =
      pedido.estadoPago
        ?.trim()
        .toLowerCase();

    // Solo permite cancelar Pendiente o Confirmado.
    const estadoPermitido =
      estado === 'pendiente' ||
      estado === 'confirmado';

    // No permite cancelar pedidos pagados.
    const noEstaPagado =
      estadoPago !== 'pagado';

    // Devuelve true si cumple ambas condiciones.
    return (
      estadoPermitido &&
      noEstaPagado
    );
  }

  // Cancela un pedido directamente desde Mis Pedidos.
  cancelarPedido(
    pedido: Pedido
  ): void {

    // Verifica nuevamente que pueda cancelarse.
    if (!this.puedeCancelar(pedido)) {
      this.mensajeError =
        'Este pedido ya no puede cancelarse.';
      return;
    }

    // Solicita confirmación antes de cancelar.
    const confirmar =
      window.confirm(
        `¿Deseas cancelar el pedido #${pedido.idPedido}?`
      );

    // Detiene el proceso si el cliente no confirma.
    if (!confirmar) {
      return;
    }

    // Obtiene los encabezados con el token.
    const headers =
      this.obtenerHeaders();

    // Verifica que exista una sesión activa.
    if (!headers) {
      this.mensajeError =
        'No existe una sesión activa.';
      return;
    }

    // Limpia mensajes anteriores.
    this.mensajeError = '';
    this.mensajeExito = '';

    // Indica que comenzó la cancelación.
    this.cancelandoPedido = true;
    this.idPedidoCancelando =
      pedido.idPedido;

    // Envía la solicitud de cancelación a la API.
    this.http
      .put<any>(
        `${this.apiUrl}/${pedido.idPedido}/cancelar`,
        {},
        { headers }
      )
      .subscribe({

        // Actualiza la lista si la cancelación funciona.
        next: (respuesta) => {
          this.mensajeExito =
            respuesta?.mensaje ||
            'Pedido cancelado correctamente.';

          this.cancelandoPedido = false;
          this.idPedidoCancelando = null;

          this.cargarPedidos();
          this.changeDetector.detectChanges();
        },

        // Muestra el error si la cancelación falla.
        error: (error: HttpErrorResponse) => {
          console.error(
            'Error al cancelar pedido:',
            error
          );

          if (
            typeof error.error ===
            'string'
          ) {
            this.mensajeError =
              error.error;
          } else if (
            error.error?.mensaje
          ) {
            this.mensajeError =
              error.error.mensaje;
          } else {
            this.mensajeError =
              'No se pudo cancelar el pedido.';
          }

          this.cancelandoPedido = false;
          this.idPedidoCancelando = null;

          this.changeDetector.detectChanges();
        }
      });
  }

  // Indica si un pedido específico está siendo cancelado.
  estaCancelando(
    idPedido: number
  ): boolean {
    return (
      this.cancelandoPedido &&
      this.idPedidoCancelando === idPedido
    );
  }

  // Abre el detalle de un pedido.
  verDetalle(
    idPedido: number
  ): void {
    this.router.navigate([
      '/detalle-pedido',
      idPedido
    ]);
  }

  // Devuelve una clase CSS según el estado.
  obtenerClaseEstado(
    estado: string
  ): string {

    // Normaliza el estado.
    const estadoNormalizado =
      estado
        ?.trim()
        .toLowerCase();

    if (estadoNormalizado === 'pendiente') {
      return 'estado-pendiente';
    }

    if (estadoNormalizado === 'confirmado') {
      return 'estado-confirmado';
    }

    if (estadoNormalizado === 'pagado') {
      return 'estado-pagado';
    }

    if (estadoNormalizado === 'enviado') {
      return 'estado-enviado';
    }

    if (estadoNormalizado === 'entregado') {
      return 'estado-entregado';
    }

    if (estadoNormalizado === 'cancelado') {
      return 'estado-cancelado';
    }

    // Devuelve una clase general para otros estados.
    return 'estado-general';
  }

  // Devuelve un icono según el estado.
  obtenerIconoEstado(
    estado: string
  ): string {

    // Normaliza el estado.
    const estadoNormalizado =
      estado
        ?.trim()
        .toLowerCase();

    if (estadoNormalizado === 'pendiente') {
      return 'schedule';
    }

    if (estadoNormalizado === 'confirmado') {
      return 'task_alt';
    }

    if (estadoNormalizado === 'pagado') {
      return 'payments';
    }

    if (estadoNormalizado === 'enviado') {
      return 'local_shipping';
    }

    if (estadoNormalizado === 'entregado') {
      return 'check_circle';
    }

    if (estadoNormalizado === 'cancelado') {
      return 'cancel';
    }

    // Utiliza un icono general para otros estados.
    return 'info';
  }

  // Devuelve la cantidad total de pedidos.
  get cantidadPedidos(): number {
    return this.pedidos.length;
  }

  // Calcula la suma de los pedidos mostrados.
  get totalPedidos(): number {
    return this.pedidos.reduce(
      (total, pedido) =>
        total + pedido.total,
      0
    );
  }

  // Vuelve a consultar los pedidos.
  actualizarPedidos(): void {
    this.cargarPedidos();
  }

  // Abre la pantalla Productos.
  irAProductos(): void {
    this.router.navigate([
      '/productos'
    ]);
  }

  // Regresa al Dashboard del cliente.
  volverDashboard(): void {
    this.router.navigate([
      '/dashboard'
    ]);
  }
}
