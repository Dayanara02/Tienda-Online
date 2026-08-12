// Importa CommonModule para usar *ngIf, *ngFor y pipes.
import { CommonModule } from '@angular/common';

// Importa las herramientas principales del componente.
import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

// Importa las herramientas para leer el id de la URL y navegar.
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

// Importa campos de texto de Angular Material.
import { MatInputModule } from '@angular/material/input';

// Importa formularios de Angular para utilizar ngModel.
import { FormsModule } from '@angular/forms';

// Importa botones de PrimeNG.
import { ButtonModule } from 'primeng/button';

// Representa cada método de pago recibido desde la API.
interface MetodoPago {
  idMetodoPago: number;
  nombre: string;
  descripcion: string | null;
}

// Representa la información necesaria del pedido.
interface PedidoPago {
  idPedido: number;
  estado: string;
  estadoPago: string;
  total: number;
  puedePagar: boolean;
}

// Representa la respuesta enviada por el backend después del pago.
interface RespuestaPago {
  mensaje: string;
  idPedido: number;
  idPago: number;
  estadoPago: string;
  estadoPedido: string;
  metodoPago: string;
  monto: number;
  referencia: string;
  fechaPago: string;
}

// Configura la pantalla de Pago de Pedido.
@Component({
  selector: 'app-pago-pedido',

  // Indica que funciona como componente independiente.
  standalone: true,

  // Registra los módulos utilizados por el HTML.
  imports: [
    CommonModule,
    FormsModule,
    MatIconModule,
    MatButtonModule,
    MatInputModule,
    ButtonModule
  ],

  // Define los archivos visuales del componente.
  templateUrl: './pago-pedido.html',
  styleUrl: './pago-pedido.css'
})
export class PagoPedido {

  // Guarda el identificador recibido desde la URL.
  idPedido = 0;

  // Guarda la información del pedido.
  pedido: PedidoPago | null = null;

  // Guarda los métodos de pago permitidos.
  metodosPago: MetodoPago[] = [];

  // Guarda el identificador del método seleccionado.
  idMetodoPagoSeleccionado = 0;

  // Guarda el nombre del método seleccionado.
  nombreMetodoSeleccionado = '';

  // Guarda el dinero que el cliente indica tener disponible.
  montoDisponible = 0;

  // Guarda cuánto dinero hace falta.
  montoFaltante = 0;

  // Guarda cuánto dinero quedaría después del pago.
  saldoRestante = 0;

  // Indica si el saldo no alcanza.
  saldoInsuficiente = false;

  // Guarda la cuenta utilizada para transferencia.
  readonly cuentaTransferenciaEsencia =
    'CR00 0000 0000 0000 0000 00';

  // Guarda la referencia de la transferencia.
  referenciaTransferencia = '';

  // Guarda el nombre del titular de la tarjeta.
  nombreTarjeta = '';

  // Guarda el número de la tarjeta.
  numeroTarjeta = '';

  // Guarda la fecha de vencimiento.
  vencimientoTarjeta = '';

  // Guarda el código de seguridad.
  cvvTarjeta = '';

  // Indica si todavía se están cargando datos.
  cargando = true;

  // Indica si el pago está siendo procesado.
  procesandoPago = false;

  // Guarda mensajes de error.
  mensajeError = '';

  // Guarda mensajes exitosos.
  mensajeExito = '';

  // Guarda la dirección del controlador de Pedidos.
  private readonly apiPedidos =
    'https://localhost:7196/api/Pedidos';

  // Guarda la dirección del controlador de métodos de pago.
  private readonly apiMetodoPagos =
    'https://localhost:7196/api/MetodoPagos';

  // Guarda la dirección del controlador de Pagos.
  private readonly apiPagos =
    'https://localhost:7196/api/Pagos';

  // Inyecta los servicios utilizados en la pantalla.
  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private router: Router,
    private changeDetector: ChangeDetectorRef
  ) {
    // Obtiene el pedido al abrir la pantalla.
    this.obtenerIdPedido();
  }

  // Obtiene el identificador del pedido desde la URL.
  obtenerIdPedido(): void {
    // Lee el parámetro llamado id.
    const idTexto =
      this.route.snapshot.paramMap.get(
        'id'
      );

    // Convierte el valor recibido a número.
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

    // Consulta el pedido.
    this.cargarPedido();
  }

  // Obtiene los encabezados necesarios para la API.
  private obtenerHeaders():
    HttpHeaders | null {

    // Obtiene el token JWT.
    const token =
      localStorage.getItem(
        'token'
      );

    // Devuelve null si no existe sesión.
    if (!token) {
      return null;
    }

    // Crea el encabezado Authorization.
    return new HttpHeaders({
      Authorization:
        `Bearer ${token}`
    });
  }

  // Consulta el pedido que se desea pagar.
  cargarPedido(): void {
    // Activa el estado de carga.
    this.cargando =
      true;

    // Limpia errores anteriores.
    this.mensajeError =
      '';

    // Obtiene los encabezados.
    const headers =
      this.obtenerHeaders();

    // Comprueba que exista sesión.
    if (!headers) {
      this.mensajeError =
        'No existe una sesión activa.';

      this.cargando =
        false;

      this.changeDetector.detectChanges();

      return;
    }

    // Consulta el pedido seleccionado.
    this.http.get<PedidoPago>(
      `${this.apiPedidos}/${this.idPedido}`,
      {
        headers
      }
    ).subscribe({

      // Se ejecuta si la API responde correctamente.
      next: (
        respuesta: PedidoPago
      ) => {
        // Guarda la información del pedido.
        this.pedido =
          respuesta;

        // Comprueba que todavía pueda pagarse.
        if (!respuesta.puedePagar) {
          this.mensajeError =
            'Este pedido ya no se encuentra disponible para pago.';

          this.cargando =
            false;

          this.changeDetector.detectChanges();

          return;
        }

        // Carga los métodos permitidos.
        this.cargarMetodosPago();
      },

      // Se ejecuta si ocurre un error.
      error: (
        error: HttpErrorResponse
      ) => {
        // Muestra el error durante las pruebas.
        console.error(
          'Error al cargar el pedido:',
          error
        );

        // Comprueba si la sesión no es válida.
        if (error.status === 401) {
          this.mensajeError =
            'La sesión no es válida. Inicia sesión nuevamente.';
        }

        // Comprueba si no tiene permiso.
        else if (error.status === 403) {
          this.mensajeError =
            'No tienes permiso para pagar este pedido.';
        }

        // Comprueba si el pedido no existe.
        else if (error.status === 404) {
          this.mensajeError =
            'El pedido no existe.';
        }

        // Maneja cualquier otro error.
        else {
          this.mensajeError =
            'No se pudo cargar la información del pedido.';
        }

        // Finaliza la carga.
        this.cargando =
          false;

        // Actualiza la pantalla.
        this.changeDetector.detectChanges();
      }
    });
  }

  // Consulta los métodos de pago disponibles.
  cargarMetodosPago(): void {
    // Obtiene los encabezados.
    const headers =
      this.obtenerHeaders();

    // Comprueba que exista sesión.
    if (!headers) {
      this.mensajeError =
        'No existe una sesión activa.';

      this.cargando =
        false;

      this.changeDetector.detectChanges();

      return;
    }

    // Consulta los métodos disponibles.
    this.http.get<MetodoPago[]>(
      `${this.apiMetodoPagos}/disponibles`,
      {
        headers
      }
    ).subscribe({

      // Guarda únicamente los métodos permitidos.
      next: (
        respuesta: MetodoPago[]
      ) => {
        // Elimina SINPE de la lista.
        this.metodosPago =
          (respuesta ?? [])
            .filter(
              metodo =>
                !metodo.nombre
                  .toLowerCase()
                  .includes('sinpe')
            );

        // Finaliza la carga.
        this.cargando =
          false;

        // Actualiza la pantalla.
        this.changeDetector.detectChanges();
      },

      // Maneja errores de la consulta.
      error: (
        error: HttpErrorResponse
      ) => {
        // Muestra el error durante las pruebas.
        console.error(
          'Error al cargar métodos:',
          error
        );

        // Muestra un mensaje.
        this.mensajeError =
          'No se pudieron cargar los métodos de pago disponibles.';

        // Finaliza la carga.
        this.cargando =
          false;

        // Actualiza la pantalla.
        this.changeDetector.detectChanges();
      }
    });
  }

  // Guarda el método seleccionado.
  seleccionarMetodo(
    metodo: MetodoPago
  ): void {
    // Guarda el identificador.
    this.idMetodoPagoSeleccionado =
      metodo.idMetodoPago;

    // Guarda el nombre.
    this.nombreMetodoSeleccionado =
      metodo.nombre;

    // Limpia mensajes anteriores.
    this.mensajeError =
      '';

    // Reinicia la validación del saldo.
    this.saldoInsuficiente =
      false;

    this.montoFaltante =
      0;

    this.saldoRestante =
      0;
  }

  // Indica si el método seleccionado es transferencia.
  esTransferencia(): boolean {
    // Busca la palabra transferencia en el nombre.
    return this.nombreMetodoSeleccionado
      .toLowerCase()
      .includes(
        'transferencia'
      );
  }

  // Indica si el método seleccionado es tarjeta.
  esTarjeta(): boolean {
    // Busca la palabra tarjeta en el nombre.
    return this.nombreMetodoSeleccionado
      .toLowerCase()
      .includes(
        'tarjeta'
      );
  }

  // Recibe el monto disponible escrito por el cliente.
  cambiarMontoDisponible(
    event: Event
  ): void {
    // Obtiene el campo que generó el evento.
    const input =
      event.target as HTMLInputElement;

    // Convierte el texto recibido a número.
    const valor =
      Number(input.value);

    // Guarda cero si el valor no es válido.
    this.montoDisponible =
      isNaN(valor)
        ? 0
        : valor;

    // Recalcula el saldo.
    this.validarFondos(
      false
    );
  }

  // Guarda la referencia de la transferencia.
  cambiarReferenciaTransferencia(
    event: Event
  ): void {
    // Obtiene el input.
    const input =
      event.target as HTMLInputElement;

    // Guarda el valor escrito.
    this.referenciaTransferencia =
      input.value;
  }

  // Guarda el nombre del titular.
  cambiarNombreTarjeta(
    event: Event
  ): void {
    // Obtiene el input.
    const input =
      event.target as HTMLInputElement;

    // Guarda el valor escrito.
    this.nombreTarjeta =
      input.value;
  }

  // Guarda el número de tarjeta.
  cambiarNumeroTarjeta(
    event: Event
  ): void {
    // Obtiene el input.
    const input =
      event.target as HTMLInputElement;

    // Guarda el valor escrito.
    this.numeroTarjeta =
      input.value;
  }

  // Guarda la fecha de vencimiento.
  cambiarVencimientoTarjeta(
    event: Event
  ): void {
    // Obtiene el input.
    const input =
      event.target as HTMLInputElement;

    // Guarda el valor escrito.
    this.vencimientoTarjeta =
      input.value;
  }

  // Guarda el código CVV.
  cambiarCvvTarjeta(
    event: Event
  ): void {
    // Obtiene el input.
    const input =
      event.target as HTMLInputElement;

    // Guarda el valor escrito.
    this.cvvTarjeta =
      input.value;
  }

  // Devuelve el total del pedido de forma segura.
  obtenerTotalPedido(): number {
    // Devuelve cero si todavía no existe pedido.
    return this.pedido?.total ?? 0;
  }

  // Comprueba que el cliente indique saldo suficiente.
  validarFondos(
    mostrarMensaje: boolean = true
  ): boolean {
    // Obtiene el total real del pedido.
    const total =
      this.obtenerTotalPedido();

    // No permite continuar si no existe un total válido.
    if (total <= 0) {
      return false;
    }

    // Reinicia los resultados anteriores.
    this.montoFaltante =
      0;

    this.saldoRestante =
      0;

    this.saldoInsuficiente =
      false;

    // Comprueba que haya indicado un monto.
    if (
      this.montoDisponible <= 0
    ) {
      if (mostrarMensaje) {
        this.mensajeError =
          'Indica cuánto dinero tienes disponible.';
      }

      return false;
    }

    // Comprueba si el dinero no alcanza.
    if (
      this.montoDisponible <
      total
    ) {
      // Calcula cuánto dinero hace falta.
      this.montoFaltante =
        total -
        this.montoDisponible;

      // Activa el indicador de saldo insuficiente.
      this.saldoInsuficiente =
        true;

      // Muestra el mensaje si corresponde.
      if (mostrarMensaje) {
        this.mensajeError =
          'Pago no completado. No tienes suficiente saldo para realizar esta compra.';
      }

      return false;
    }

    // Calcula cuánto dinero quedaría disponible.
    this.saldoRestante =
      this.montoDisponible -
      total;

    // Limpia el mensaje de saldo anterior.
    if (
      this.mensajeError.includes(
        'suficiente saldo'
      )
    ) {
      this.mensajeError =
        '';
    }

    return true;
  }

  // Indica si el saldo disponible alcanza.
  tieneFondosSuficientes(): boolean {
    // Obtiene el total.
    const total =
      this.obtenerTotalPedido();

    // Comprueba el saldo.
    return (
      total > 0 &&
      this.montoDisponible >= total
    );
  }

  // Valida los datos según el método seleccionado.
  validarDatosMetodo(): boolean {
    // Valida la transferencia bancaria.
    if (this.esTransferencia()) {
      // Comprueba que exista una referencia.
      if (
        !this.referenciaTransferencia
          .trim()
      ) {
        this.mensajeError =
          'Ingresa la referencia de la transferencia bancaria.';

        return false;
      }
    }

    // Valida los datos de tarjeta.
    if (this.esTarjeta()) {
      // Comprueba el nombre del titular.
      if (
        !this.nombreTarjeta
          .trim()
      ) {
        this.mensajeError =
          'Ingresa el nombre que aparece en la tarjeta.';

        return false;
      }

      // Elimina espacios del número de tarjeta.
      const numeroLimpio =
        this.numeroTarjeta
          .replace(
            /\s/g,
            ''
          );

      // Comprueba que tenga 16 dígitos.
      if (
        !/^\d{16}$/.test(
          numeroLimpio
        )
      ) {
        this.mensajeError =
          'El número de tarjeta debe contener 16 dígitos.';

        return false;
      }

      // Comprueba el formato MM/AA.
      if (
        !/^\d{2}\/\d{2}$/.test(
          this.vencimientoTarjeta
        )
      ) {
        this.mensajeError =
          'La fecha de vencimiento debe utilizar el formato MM/AA.';

        return false;
      }

      // Comprueba que el mes sea válido.
      const mes =
        Number(
          this.vencimientoTarjeta
            .substring(
              0,
              2
            )
        );

      // Evita meses fuera del rango permitido.
      if (
        mes < 1 ||
        mes > 12
      ) {
        this.mensajeError =
          'El mes de vencimiento de la tarjeta no es válido.';

        return false;
      }

      // Comprueba el código CVV.
      if (
        !/^\d{3,4}$/.test(
          this.cvvTarjeta
        )
      ) {
        this.mensajeError =
          'El CVV debe contener 3 o 4 dígitos.';

        return false;
      }
    }

    return true;
  }

  // Envía el pago hacia el backend.
  pagar(): void {
    // Comprueba que exista el pedido.
    if (!this.pedido) {
      return;
    }

    // Comprueba que se haya seleccionado un método.
    if (
      this.idMetodoPagoSeleccionado <= 0
    ) {
      this.mensajeError =
        'Selecciona un método de pago antes de continuar.';

      return;
    }

    // Comprueba nuevamente que no se haya seleccionado SINPE.
    if (
      this.nombreMetodoSeleccionado
        .toLowerCase()
        .includes('sinpe')
    ) {
      this.mensajeError =
        'SINPE no se encuentra disponible como método de pago.';

      return;
    }

    // Comprueba el saldo disponible.
    if (
      !this.validarFondos(
        true
      )
    ) {
      return;
    }

    // Comprueba los datos del método seleccionado.
    if (
      !this.validarDatosMetodo()
    ) {
      return;
    }

    // Evita enviar el pago dos veces.
    if (this.procesandoPago) {
      return;
    }

    // Obtiene los encabezados de autorización.
    const headers =
      this.obtenerHeaders();

    // Comprueba que exista sesión.
    if (!headers) {
      this.mensajeError =
        'No existe una sesión activa.';

      return;
    }

    // Activa el procesamiento.
    this.procesandoPago =
      true;

    // Limpia mensajes anteriores.
    this.mensajeError =
      '';

    this.mensajeExito =
      '';

    // Crea los datos que recibe el backend.
    const datosPago = {
      // Envía el pedido seleccionado.
      idPedido:
        this.pedido.idPedido,

      // Envía el método seleccionado.
      idMetodoPago:
        this.idMetodoPagoSeleccionado
    };

    // Envía el pago al backend.
    this.http.post<RespuestaPago>(
      `${this.apiPagos}/pagar`,
      datosPago,
      {
        headers
      }
    ).subscribe({

      // Se ejecuta cuando el pago es correcto.
      next: (
        respuesta: RespuestaPago
      ) => {
        // Muestra la respuesta durante las pruebas.
        console.log(
          'Pago realizado:',
          respuesta
        );

        // Informa que el pago fue realizado.
        this.mensajeExito =
          'Pago realizado correctamente.';

        // Finaliza el procesamiento.
        this.procesandoPago =
          false;

        // Actualiza la pantalla.
        this.changeDetector.detectChanges();

        // Regresa al detalle después de mostrar el mensaje.
        setTimeout(
          () => {
            this.router.navigate([
              '/detalle-pedido',
              this.idPedido
            ]);
          },
          1200
        );
      },

      // Se ejecuta cuando ocurre un error.
      error: (
        error: HttpErrorResponse
      ) => {
        // Muestra el error durante las pruebas.
        console.error(
          'Error al realizar el pago:',
          error
        );

        // Utiliza el mensaje enviado por la API.
        if (
          typeof error.error ===
          'string' &&
          error.error
        ) {
          this.mensajeError =
            error.error;
        }

        // Utiliza el campo mensaje si existe.
        else if (
          error.error?.mensaje
        ) {
          this.mensajeError =
            error.error.mensaje;
        }

        // Utiliza un mensaje general.
        else {
          this.mensajeError =
            'No se pudo completar el pago.';
        }

        // Finaliza el procesamiento.
        this.procesandoPago =
          false;

        // Actualiza la pantalla.
        this.changeDetector.detectChanges();
      }
    });
  }

  // Regresa al detalle del pedido sin realizar el pago.
  cancelar(): void {
    // Navega al detalle del pedido actual.
    this.router.navigate([
      '/detalle-pedido',
      this.idPedido
    ]);
  }
}
