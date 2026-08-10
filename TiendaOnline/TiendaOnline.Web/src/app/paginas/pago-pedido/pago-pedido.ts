// Importa CommonModule para utilizar directivas
// como *ngIf y *ngFor dentro del HTML.
import { CommonModule } from '@angular/common';

// Importa las herramientas necesarias
// para crear el componente y actualizar la pantalla.
import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

// ActivatedRoute permite obtener el id del pedido
// que viene dentro de la dirección URL.
//
// Router permite navegar hacia otras páginas.
import {
  ActivatedRoute,
  Router
} from '@angular/router';

// HttpClient permite comunicarse con la API.
//
// HttpHeaders permite enviar el token JWT.
//
// HttpErrorResponse permite manejar errores HTTP.
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders
} from '@angular/common/http';


// =========================================================
// INTERFAZ DEL MÉTODO DE PAGO
// =========================================================

// Representa cada método de pago
// recibido desde la base de datos.
interface MetodoPago {

  // Identificador del método.
  idMetodoPago: number;

  // Nombre del método.
  nombre: string;

  // Descripción opcional.
  descripcion: string | null;
}


// =========================================================
// INTERFAZ DEL PEDIDO
// =========================================================

// Representa la información necesaria
// para realizar el pago.
interface PedidoPago {

  // Identificador del pedido.
  idPedido: number;

  // Estado general del pedido.
  estado: string;

  // Estado actual del pago.
  estadoPago: string;

  // Total exacto del pedido.
  total: number;

  // Indica si todavía puede pagarse.
  puedePagar: boolean;
}


// =========================================================
// INTERFAZ DE RESPUESTA DEL PAGO
// =========================================================

// Representa la información que devuelve
// el backend después de pagar.
interface RespuestaPago {

  // Mensaje de confirmación.
  mensaje: string;

  // Pedido pagado.
  idPedido: number;

  // Pago creado.
  idPago: number;

  // Estado del pago.
  estadoPago: string;

  // Estado general del pedido.
  estadoPedido: string;

  // Método utilizado.
  metodoPago: string;

  // Monto pagado.
  monto: number;

  // Referencia generada.
  referencia: string;

  // Fecha del pago.
  fechaPago: string;
}


// =========================================================
// COMPONENTE
// =========================================================

@Component({

  // Nombre interno del componente.
  selector: 'app-pago-pedido',

  // Indica que funciona como componente independiente.
  standalone: true,

  // Permite utilizar directivas comunes de Angular.
  imports: [
    CommonModule
  ],

  // Archivo HTML relacionado.
  templateUrl: './pago-pedido.html',

  // Archivo CSS relacionado.
  styleUrl: './pago-pedido.css'
})


// Clase principal de la pantalla de pago.
export class PagoPedido {

  // =======================================================
  // PEDIDO
  // =======================================================

  // Identificador obtenido desde la URL.
  idPedido: number = 0;

  // Guarda la información del pedido.
  pedido: PedidoPago | null = null;


  // =======================================================
  // MÉTODOS DE PAGO
  // =======================================================

  // Lista de métodos disponibles.
  metodosPago: MetodoPago[] = [];

  // Identificador del método seleccionado.
  idMetodoPagoSeleccionado: number = 0;

  // Nombre del método seleccionado.
  nombreMetodoSeleccionado: string = '';


  // =======================================================
  // DINERO DISPONIBLE
  // =======================================================

  // Dinero que el Cliente indica tener disponible.
  montoDisponible: number = 0;

  // Dinero que hace falta si no alcanza.
  montoFaltante: number = 0;

  // Dinero que quedaría después del pago.
  saldoRestante: number = 0;

  // Indica si actualmente el dinero no alcanza.
  saldoInsuficiente: boolean = false;


  // =======================================================
  // SINPE MÓVIL
  // =======================================================

  // Número SINPE del negocio.
  readonly numeroSinpeEsencia: string =
    '6177-2331';

  // Número desde donde el Cliente realiza el SINPE.
  numeroOrigenSinpe: string = '';

  // Referencia o comprobante del SINPE.
  referenciaSinpe: string = '';


  // =======================================================
  // TRANSFERENCIA
  // =======================================================

  // Cuenta utilizada para la simulación
  // de transferencia bancaria.
  readonly cuentaTransferenciaEsencia: string =
    'CR00 0000 0000 0000 0000 00';

  // Referencia de la transferencia.
  referenciaTransferencia: string = '';


  // =======================================================
  // TARJETA
  // =======================================================

  // Nombre del titular.
  nombreTarjeta: string = '';

  // Número de tarjeta.
  numeroTarjeta: string = '';

  // Fecha de vencimiento.
  vencimientoTarjeta: string = '';

  // Código de seguridad.
  cvvTarjeta: string = '';


  // =======================================================
  // ESTADOS DE LA PANTALLA
  // =======================================================

  // Indica si todavía se están cargando datos.
  cargando: boolean = true;

  // Indica si el pago está siendo procesado.
  procesandoPago: boolean = false;

  // Guarda errores.
  mensajeError: string = '';

  // Guarda mensajes exitosos.
  mensajeExito: string = '';


  // =======================================================
  // URL DE LA API
  // =======================================================

  // API de pedidos.
  private readonly apiPedidos =
    'https://localhost:7196/api/Pedidos';

  // API de métodos de pago.
  private readonly apiMetodoPagos =
    'https://localhost:7196/api/MetodoPagos';

  // API de pagos.
  private readonly apiPagos =
    'https://localhost:7196/api/Pagos';


  // =======================================================
  // CONSTRUCTOR
  // =======================================================

  constructor(

    // Permite obtener el id desde la URL.
    private route: ActivatedRoute,

    // Permite realizar peticiones HTTP.
    private http: HttpClient,

    // Permite navegar entre páginas.
    private router: Router,

    // Permite actualizar manualmente la pantalla.
    private changeDetector: ChangeDetectorRef
  ) {

    // Obtiene el pedido cuando abre la página.
    this.obtenerIdPedido();
  }


  // =========================================================
  // OBTENER ID DEL PEDIDO
  // =========================================================

  obtenerIdPedido(): void {

    // Obtiene el parámetro id.
    const idTexto =
      this.route.snapshot.paramMap.get('id');

    // Lo convierte a número.
    this.idPedido =
      Number(idTexto);


    // Comprueba que sea válido.
    if (
      !this.idPedido ||
      this.idPedido <= 0
    ) {

      this.mensajeError =
        'El pedido seleccionado no es válido.';

      this.cargando = false;

      this.changeDetector.detectChanges();

      return;
    }


    // Carga el pedido.
    this.cargarPedido();
  }


  // =========================================================
  // OBTENER HEADERS
  // =========================================================

  private obtenerHeaders(): HttpHeaders | null {

    // Obtiene el token JWT.
    const token =
      localStorage.getItem('token');


    // Si no existe sesión, devuelve null.
    if (!token) {

      return null;
    }


    // Crea el header Authorization.
    return new HttpHeaders({

      Authorization:
        `Bearer ${token}`
    });
  }


  // =========================================================
  // CARGAR PEDIDO
  // =========================================================

  cargarPedido(): void {

    // Activa la carga.
    this.cargando = true;

    // Limpia errores.
    this.mensajeError = '';


    // Obtiene los headers.
    const headers =
      this.obtenerHeaders();


    // Comprueba que exista sesión.
    if (!headers) {

      this.mensajeError =
        'No existe una sesión activa.';

      this.cargando = false;

      this.changeDetector.detectChanges();

      return;
    }


    // Consulta el pedido.
    this.http.get<PedidoPago>(
      `${this.apiPedidos}/${this.idPedido}`,
      {
        headers: headers
      }
    ).subscribe({

      // Cuando funciona correctamente.
      next: (respuesta: PedidoPago) => {

        // Guarda el pedido.
        this.pedido =
          respuesta;


        // Comprueba que todavía pueda pagarse.
        if (!respuesta.puedePagar) {

          this.mensajeError =
            'Este pedido ya no se encuentra disponible para pago.';

          this.cargando = false;

          this.changeDetector.detectChanges();

          return;
        }


        // Carga los métodos disponibles.
        this.cargarMetodosPago();
      },


      // Si ocurre un error.
      error: (error: HttpErrorResponse) => {

        console.error(
          'Error al cargar el pedido:',
          error
        );


        if (error.status === 401) {

          this.mensajeError =
            'La sesión no es válida. Inicia sesión nuevamente.';
        }
        else if (error.status === 403) {

          this.mensajeError =
            'No tienes permiso para pagar este pedido.';
        }
        else if (error.status === 404) {

          this.mensajeError =
            'El pedido no existe.';
        }
        else {

          this.mensajeError =
            'No se pudo cargar la información del pedido.';
        }


        this.cargando = false;

        this.changeDetector.detectChanges();
      }
    });
  }


  // =========================================================
  // CARGAR MÉTODOS
  // =========================================================

  cargarMetodosPago(): void {

    // Obtiene el token.
    const headers =
      this.obtenerHeaders();


    // Comprueba que exista sesión.
    if (!headers) {

      this.mensajeError =
        'No existe una sesión activa.';

      this.cargando = false;

      this.changeDetector.detectChanges();

      return;
    }


    // Consulta los métodos disponibles.
    this.http.get<MetodoPago[]>(
      `${this.apiMetodoPagos}/disponibles`,
      {
        headers: headers
      }
    ).subscribe({

      // Guarda la respuesta.
      next: (respuesta: MetodoPago[]) => {

        this.metodosPago =
          respuesta ?? [];

        this.cargando = false;

        this.changeDetector.detectChanges();
      },


      // Maneja errores.
      error: (error: HttpErrorResponse) => {

        console.error(
          'Error al cargar métodos:',
          error
        );

        this.mensajeError =
          'No se pudieron cargar los métodos de pago disponibles.';

        this.cargando = false;

        this.changeDetector.detectChanges();
      }
    });
  }


  // =========================================================
  // SELECCIONAR MÉTODO
  // =========================================================

  seleccionarMetodo(
    metodo: MetodoPago
  ): void {

    // Guarda el id seleccionado.
    this.idMetodoPagoSeleccionado =
      metodo.idMetodoPago;

    // Guarda el nombre.
    this.nombreMetodoSeleccionado =
      metodo.nombre;

    // Limpia errores.
    this.mensajeError = '';

    // Reinicia la validación de dinero.
    this.saldoInsuficiente = false;

    this.montoFaltante = 0;

    this.saldoRestante = 0;
  }


  // =========================================================
  // IDENTIFICAR EL MÉTODO
  // =========================================================

  // Comprueba si se seleccionó SINPE.
  esSinpe(): boolean {

    return this.nombreMetodoSeleccionado
      .toLowerCase()
      .includes('sinpe');
  }


  // Comprueba si se seleccionó transferencia.
  esTransferencia(): boolean {

    return this.nombreMetodoSeleccionado
      .toLowerCase()
      .includes('transferencia');
  }


  // Comprueba si se seleccionó tarjeta.
  esTarjeta(): boolean {

    return this.nombreMetodoSeleccionado
      .toLowerCase()
      .includes('tarjeta');
  }


  // =========================================================
  // MÉTODOS PARA LOS INPUT DEL HTML
  // =========================================================

  // Recibe el valor escrito en Dinero disponible.
  cambiarMontoDisponible(
    event: Event
  ): void {

    // Obtiene el input que generó el evento.
    const input =
      event.target as HTMLInputElement;

    // Convierte el texto a número.
    const valor =
      Number(input.value);


    // Si no es válido, utiliza cero.
    this.montoDisponible =
      isNaN(valor)
        ? 0
        : valor;


    // Recalcula inmediatamente.
    this.validarFondos(false);
  }


  // Guarda el número desde donde se hizo el SINPE.
  cambiarNumeroSinpe(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    this.numeroOrigenSinpe =
      input.value;
  }


  // Guarda la referencia del SINPE.
  cambiarReferenciaSinpe(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    this.referenciaSinpe =
      input.value;
  }


  // Guarda la referencia de transferencia.
  cambiarReferenciaTransferencia(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    this.referenciaTransferencia =
      input.value;
  }


  // Guarda el nombre del titular de la tarjeta.
  cambiarNombreTarjeta(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    this.nombreTarjeta =
      input.value;
  }


  // Guarda el número de tarjeta.
  cambiarNumeroTarjeta(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    this.numeroTarjeta =
      input.value;
  }


  // Guarda el vencimiento.
  cambiarVencimientoTarjeta(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    this.vencimientoTarjeta =
      input.value;
  }


  // Guarda el CVV.
  cambiarCvvTarjeta(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    this.cvvTarjeta =
      input.value;
  }


  // =========================================================
  // OBTENER TOTAL DEL PEDIDO
  // =========================================================

  // Devuelve el total de forma segura.
  //
  // Si por alguna razón el pedido todavía
  // no existe, devuelve cero.
  obtenerTotalPedido(): number {

    return this.pedido?.total ?? 0;
  }


  // =========================================================
  // VALIDAR FONDOS
  // =========================================================

  validarFondos(
    mostrarMensaje: boolean = true
  ): boolean {

    // Obtiene el total de forma segura.
    const total =
      this.obtenerTotalPedido();


    // Si no hay pedido válido,
    // no permite continuar.
    if (total <= 0) {

      return false;
    }


    // Reinicia los resultados.
    this.montoFaltante = 0;

    this.saldoRestante = 0;

    this.saldoInsuficiente = false;


    // Comprueba que haya indicado dinero.
    if (this.montoDisponible <= 0) {

      if (mostrarMensaje) {

        this.mensajeError =
          'Indica cuánto dinero tienes disponible.';
      }

      return false;
    }


    // Comprueba si el dinero es insuficiente.
    if (
      this.montoDisponible <
      total
    ) {

      // Calcula cuánto falta.
      this.montoFaltante =
        total -
        this.montoDisponible;

      // Activa el estado de error.
      this.saldoInsuficiente = true;


      if (mostrarMensaje) {

        this.mensajeError =
          'Pago no completado. No tienes suficiente saldo para realizar esta compra.';
      }


      return false;
    }


    // Calcula el saldo restante.
    this.saldoRestante =
      this.montoDisponible -
      total;


    // Limpia el mensaje anterior
    // relacionado con saldo.
    if (
      this.mensajeError.includes(
        'suficiente saldo'
      )
    ) {

      this.mensajeError = '';
    }


    return true;
  }


  // =========================================================
  // SABER SI HAY FONDOS SUFICIENTES
  // =========================================================

  // Este método será utilizado por el HTML
  // para mostrar el mensaje verde.
  tieneFondosSuficientes(): boolean {

    const total =
      this.obtenerTotalPedido();


    return (
      total > 0 &&
      this.montoDisponible >= total
    );
  }


  // =========================================================
  // VALIDAR DATOS DEL MÉTODO
  // =========================================================

  validarDatosMetodo(): boolean {

    // ---------------------------
    // SINPE
    // ---------------------------

    if (this.esSinpe()) {

      if (
        !this.numeroOrigenSinpe.trim()
      ) {

        this.mensajeError =
          'Indica el número desde el que realizaste el SINPE.';

        return false;
      }


      if (
        !this.referenciaSinpe.trim()
      ) {

        this.mensajeError =
          'Ingresa la referencia o comprobante del SINPE.';

        return false;
      }
    }


    // ---------------------------
    // TRANSFERENCIA
    // ---------------------------

    if (this.esTransferencia()) {

      if (
        !this.referenciaTransferencia.trim()
      ) {

        this.mensajeError =
          'Ingresa la referencia de la transferencia bancaria.';

        return false;
      }
    }


    // ---------------------------
    // TARJETA
    // ---------------------------

    if (this.esTarjeta()) {

      // Comprueba nombre.
      if (
        !this.nombreTarjeta.trim()
      ) {

        this.mensajeError =
          'Ingresa el nombre que aparece en la tarjeta.';

        return false;
      }


      // Elimina espacios.
      const numeroLimpio =
        this.numeroTarjeta
          .replace(/\s/g, '');


      // Valida 16 dígitos.
      if (
        !/^\d{16}$/.test(
          numeroLimpio
        )
      ) {

        this.mensajeError =
          'El número de tarjeta debe contener 16 dígitos.';

        return false;
      }


      // Valida MM/AA.
      if (
        !/^\d{2}\/\d{2}$/.test(
          this.vencimientoTarjeta
        )
      ) {

        this.mensajeError =
          'La fecha de vencimiento debe utilizar el formato MM/AA.';

        return false;
      }


      // Valida CVV.
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


  // =========================================================
  // PAGAR
  // =========================================================

  pagar(): void {

    // Comprueba que exista pedido.
    if (!this.pedido) {

      return;
    }


    // Comprueba que seleccionó método.
    if (
      this.idMetodoPagoSeleccionado <= 0
    ) {

      this.mensajeError =
        'Selecciona un método de pago antes de continuar.';

      return;
    }


    // Comprueba el saldo.
    if (!this.validarFondos(true)) {

      return;
    }


    // Comprueba los datos específicos.
    if (!this.validarDatosMetodo()) {

      return;
    }


    // Evita doble pago.
    if (this.procesandoPago) {

      return;
    }


    // Obtiene autorización.
    const headers =
      this.obtenerHeaders();


    if (!headers) {

      this.mensajeError =
        'No existe una sesión activa.';

      return;
    }


    // Activa procesamiento.
    this.procesandoPago = true;

    // Limpia mensajes anteriores.
    this.mensajeError = '';

    this.mensajeExito = '';


    // Crea los datos que necesita el backend.
    const datosPago = {

      // Pedido que se pagará.
      idPedido:
        this.pedido.idPedido,

      // Método seleccionado.
      idMetodoPago:
        this.idMetodoPagoSeleccionado
    };


    // Envía el pago al backend.
    this.http.post<RespuestaPago>(
      `${this.apiPagos}/pagar`,
      datosPago,
      {
        headers: headers
      }
    ).subscribe({

      // Pago correcto.
      next: (respuesta: RespuestaPago) => {

        console.log(
          'Pago realizado:',
          respuesta
        );


        this.mensajeExito =
          'Pago realizado correctamente.';

        this.procesandoPago = false;

        this.changeDetector.detectChanges();


        // Por ahora regresa al detalle.
        //
        // Después cambiaremos esta navegación
        // por la pantalla de envío.
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


      // Error en el pago.
      error: (error: HttpErrorResponse) => {

        console.error(
          'Error al realizar el pago:',
          error
        );


        if (
          typeof error.error === 'string' &&
          error.error
        ) {

          this.mensajeError =
            error.error;
        }
        else if (
          error.error?.mensaje
        ) {

          this.mensajeError =
            error.error.mensaje;
        }
        else {

          this.mensajeError =
            'No se pudo completar el pago.';
        }


        this.procesandoPago = false;

        this.changeDetector.detectChanges();
      }
    });
  }


  // =========================================================
  // CANCELAR
  // =========================================================

  // Regresa al detalle sin pagar.
  cancelar(): void {

    this.router.navigate([
      '/detalle-pedido',
      this.idPedido
    ]);
  }
}
