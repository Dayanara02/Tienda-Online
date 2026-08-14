// Permite usar directivas comunes de Angular.
import {
  CommonModule
} from '@angular/common';

// Importa herramientas principales del componente.
import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

// Permite leer parámetros de la URL y navegar.
import {
  ActivatedRoute,
  Router
} from '@angular/router';

// Permite realizar peticiones HTTP.
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders
} from '@angular/common/http';

// Permite utilizar formularios.
import {
  FormsModule
} from '@angular/forms';

// Permite usar iconos de Angular Material.
import {
  MatIconModule
} from '@angular/material/icon';

// Permite usar botones de Angular Material.
import {
  MatButtonModule
} from '@angular/material/button';

// Permite usar campos de texto.
import {
  MatInputModule
} from '@angular/material/input';

// Permite utilizar mat-form-field.
import {
  MatFormFieldModule
} from '@angular/material/form-field';

// Permite usar botones de PrimeNG.
import {
  ButtonModule
} from 'primeng/button';


// Representa un método de pago.
interface MetodoPago {

  // Identificador.
  idMetodoPago: number;

  // Nombre.
  nombre: string;

  // Descripción.
  descripcion: string | null;
}


// Representa los datos del pedido.
interface PedidoPago {

  // Identificador.
  idPedido: number;

  // Estado.
  estado: string;

  // Estado del pago.
  estadoPago: string;

  // Total.
  total: number;

  // Indica si puede pagarse.
  puedePagar: boolean;
}


// Representa la respuesta del pago.
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


// Configura la pantalla.
@Component({
  selector: 'app-pago-pedido',

  standalone: true,

  imports: [
    CommonModule,
    FormsModule,
    MatIconModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    ButtonModule
  ],

  templateUrl: './pago-pedido.html',

  styleUrl: './pago-pedido.css'
})
export class PagoPedido {

  // Guarda el identificador del pedido.
  idPedido = 0;

  // Guarda el pedido.
  pedido: PedidoPago | null = null;

  // Guarda los métodos de pago.
  metodosPago: MetodoPago[] = [];

  // Guarda el método seleccionado.
  idMetodoPagoSeleccionado = 0;

  // Guarda el nombre del método.
  nombreMetodoSeleccionado = '';

  // Guarda el dinero disponible.
  montoDisponible = 0;

  // Guarda cuánto dinero falta.
  montoFaltante = 0;

  // Guarda cuánto dinero sobra.
  saldoRestante = 0;

  // Indica si el saldo es insuficiente.
  saldoInsuficiente = false;


  // Número SINPE Móvil de ESENCIA.
  readonly numeroSinpeEsencia =
    '61772321';


  // Cuenta para transferencia.
  readonly cuentaTransferenciaEsencia =
    'CR00 0000 0000 0000 0000 00';


  // Guarda la referencia bancaria.
  referenciaTransferencia = '';

  // Guarda el titular de la tarjeta.
  nombreTarjeta = '';

  // Guarda el número de tarjeta.
  numeroTarjeta = '';

  // Guarda la fecha de vencimiento.
  vencimientoTarjeta = '';

  // Guarda el CVV.
  cvvTarjeta = '';

  // Indica si está cargando.
  cargando = true;

  // Indica si está procesando.
  procesandoPago = false;

  // Guarda errores.
  mensajeError = '';

  // Guarda mensajes exitosos.
  mensajeExito = '';


  // Endpoint de pedidos.
  private readonly apiPedidos =
    'https://localhost:7196/api/Pedidos';

  // Endpoint de métodos de pago.
  private readonly apiMetodoPagos =
    'https://localhost:7196/api/MetodoPagos';

  // Endpoint de pagos.
  private readonly apiPagos =
    'https://localhost:7196/api/Pagos';


  // Constructor.
  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private router: Router,
    private changeDetector: ChangeDetectorRef
  ) {

    // Obtiene el pedido.
    this.obtenerIdPedido();
  }


  // Obtiene el ID desde la URL.
  obtenerIdPedido(): void {

    // Obtiene el parámetro.
    const idTexto =
      this.route.snapshot.paramMap.get(
        'id'
      );

    // Convierte a número.
    this.idPedido =
      Number(
        idTexto
      );

    // Valida el ID.
    if (
      !this.idPedido ||
      this.idPedido <= 0
    ) {

      this.mensajeError =
        'El pedido seleccionado no es válido.';

      this.cargando =
        false;

      this.changeDetector
        .detectChanges();

      return;
    }

    // Carga el pedido.
    this.cargarPedido();
  }


  // Crea los headers con JWT.
  private obtenerHeaders():
    HttpHeaders | null {

    // Obtiene el token.
    const token =
      localStorage.getItem(
        'token'
      );

    // Verifica que exista.
    if (!token) {
      return null;
    }

    // Devuelve el header.
    return new HttpHeaders({
      Authorization:
        `Bearer ${token}`
    });
  }


  // Carga el pedido.
  cargarPedido(): void {

    // Activa carga.
    this.cargando =
      true;

    // Limpia errores.
    this.mensajeError =
      '';

    // Obtiene headers.
    const headers =
      this.obtenerHeaders();

    // Valida sesión.
    if (!headers) {

      this.mensajeError =
        'No existe una sesión activa.';

      this.cargando =
        false;

      this.changeDetector
        .detectChanges();

      return;
    }

    // Consulta el pedido.
    this.http
      .get<PedidoPago>(
        `${this.apiPedidos}/${this.idPedido}`,
        {
          headers
        }
      )
      .subscribe({

        // Si funciona.
        next: respuesta => {

          // Guarda el pedido.
          this.pedido =
            respuesta;

          // Verifica si puede pagarse.
          if (
            !respuesta.puedePagar
          ) {

            this.mensajeError =
              'Este pedido ya no se encuentra disponible para pago.';

            this.cargando =
              false;

            this.changeDetector
              .detectChanges();

            return;
          }

          // Carga los métodos.
          this.cargarMetodosPago();
        },


        // Si falla.
        error: (
          error: HttpErrorResponse
        ) => {

          console.error(
            'Error al cargar el pedido:',
            error
          );

          if (
            error.status === 401
          ) {

            this.mensajeError =
              'La sesión no es válida. Inicia sesión nuevamente.';
          }

          else if (
            error.status === 403
          ) {

            this.mensajeError =
              'No tienes permiso para pagar este pedido.';
          }

          else if (
            error.status === 404
          ) {

            this.mensajeError =
              'El pedido no existe.';
          }

          else {

            this.mensajeError =
              'No se pudo cargar la información del pedido.';
          }

          this.cargando =
            false;

          this.changeDetector
            .detectChanges();
        }
      });
  }


  // Carga los métodos de pago.
  cargarMetodosPago(): void {

    // Obtiene los headers.
    const headers =
      this.obtenerHeaders();

    // Valida sesión.
    if (!headers) {

      this.mensajeError =
        'No existe una sesión activa.';

      this.cargando =
        false;

      this.changeDetector
        .detectChanges();

      return;
    }

    // Consulta los métodos disponibles.
    this.http
      .get<MetodoPago[]>(
        `${this.apiMetodoPagos}/disponibles`,
        {
          headers
        }
      )
      .subscribe({

        // Si funciona.
        next: respuesta => {

          // Guarda los métodos.
          this.metodosPago =
            respuesta;

          // Limpia errores.
          this.mensajeError =
            '';

          // Finaliza carga.
          this.cargando =
            false;

          // Actualiza pantalla.
          this.changeDetector
            .detectChanges();
        },


        // Si falla.
        error: (
          error: HttpErrorResponse
        ) => {

          console.error(
            'Error al cargar métodos de pago:',
            error
          );

          // Limpia métodos.
          this.metodosPago =
            [];

          if (
            error.status === 401
          ) {

            this.mensajeError =
              'La sesión no es válida. Inicia sesión nuevamente.';
          }

          else if (
            error.status === 403
          ) {

            this.mensajeError =
              'Tu usuario no tiene permiso para consultar los métodos de pago.';
          }

          else {

            this.mensajeError =
              'No se pudieron cargar los métodos de pago disponibles.';
          }

          this.cargando =
            false;

          this.changeDetector
            .detectChanges();
        }
      });
  }


  // Selecciona un método de pago.
  seleccionarMetodo(
    metodo: MetodoPago
  ): void {

    // Guarda el ID.
    this.idMetodoPagoSeleccionado =
      metodo.idMetodoPago;

    // Guarda el nombre.
    this.nombreMetodoSeleccionado =
      metodo.nombre;

    // Limpia errores.
    this.mensajeError =
      '';

    // Reinicia saldo.
    this.saldoInsuficiente =
      false;

    // Reinicia faltante.
    this.montoFaltante =
      0;

    // Reinicia restante.
    this.saldoRestante =
      0;

    // Actualiza inmediatamente.
    this.changeDetector
      .detectChanges();
  }


  // Comprueba si es SINPE.
  esSinpe(): boolean {

    return this.nombreMetodoSeleccionado
      .toLowerCase()
      .includes(
        'sinpe'
      );
  }


  // Comprueba si es transferencia.
  esTransferencia(): boolean {

    return this.nombreMetodoSeleccionado
      .toLowerCase()
      .includes(
        'transferencia'
      );
  }


  // Comprueba si es tarjeta.
  esTarjeta(): boolean {

    return this.nombreMetodoSeleccionado
      .toLowerCase()
      .includes(
        'tarjeta'
      );
  }


  // Cambia el monto disponible.
  cambiarMontoDisponible(
    event: Event
  ): void {

    // Obtiene el input.
    const input =
      event.target as HTMLInputElement;

    // Convierte el valor.
    const valor =
      Number(
        input.value
      );

    // Guarda el monto.
    this.montoDisponible =
      isNaN(valor)
        ? 0
        : valor;

    // Valida el saldo.
    this.validarFondos(
      false
    );
  }


  // Cambia referencia de transferencia.
  cambiarReferenciaTransferencia(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    this.referenciaTransferencia =
      input.value;
  }


  // Cambia nombre de tarjeta.
  cambiarNombreTarjeta(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    this.nombreTarjeta =
      input.value;
  }


  // Cambia número de tarjeta.
  cambiarNumeroTarjeta(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    this.numeroTarjeta =
      input.value;
  }


  // Cambia vencimiento.
  cambiarVencimientoTarjeta(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    this.vencimientoTarjeta =
      input.value;
  }


  // Cambia CVV.
  cambiarCvvTarjeta(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    this.cvvTarjeta =
      input.value;
  }


  // Obtiene el total.
  obtenerTotalPedido(): number {

    return (
      this.pedido?.total ??
      0
    );
  }


  // Valida fondos disponibles.
  validarFondos(
    mostrarMensaje:
      boolean = true
  ): boolean {

    // Obtiene total.
    const total =
      this.obtenerTotalPedido();

    // Valida total.
    if (
      total <= 0
    ) {
      return false;
    }

    // Reinicia datos.
    this.montoFaltante =
      0;

    this.saldoRestante =
      0;

    this.saldoInsuficiente =
      false;


    // Valida dinero disponible.
    if (
      this.montoDisponible <= 0
    ) {

      if (
        mostrarMensaje
      ) {

        this.mensajeError =
          'Indica cuánto dinero tienes disponible.';
      }

      return false;
    }


    // Si no alcanza.
    if (
      this.montoDisponible <
      total
    ) {

      // Calcula faltante.
      this.montoFaltante =
        total -
        this.montoDisponible;

      // Marca insuficiente.
      this.saldoInsuficiente =
        true;

      if (
        mostrarMensaje
      ) {

        this.mensajeError =
          'Pago no completado. No tienes suficiente saldo para realizar esta compra.';
      }

      return false;
    }


    // Calcula sobrante.
    this.saldoRestante =
      this.montoDisponible -
      total;

    // Limpia error anterior.
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


  // Comprueba si alcanza.
  tieneFondosSuficientes():
    boolean {

    const total =
      this.obtenerTotalPedido();

    return (
      total > 0 &&
      this.montoDisponible >=
      total
    );
  }


  // Valida datos del método.
  validarDatosMetodo(): boolean {

    // Valida transferencia.
    if (
      this.esTransferencia()
    ) {

      if (
        !this.referenciaTransferencia
          .trim()
      ) {

        this.mensajeError =
          'Ingresa la referencia de la transferencia bancaria.';

        return false;
      }
    }


    // Valida tarjeta.
    if (
      this.esTarjeta()
    ) {

      // Valida nombre.
      if (
        !this.nombreTarjeta
          .trim()
      ) {

        this.mensajeError =
          'Ingresa el nombre que aparece en la tarjeta.';

        return false;
      }


      // Valida letras.
      if (
        !/^[A-Za-zÁÉÍÓÚáéíóúÑñÜü\s]+$/.test(
          this.nombreTarjeta
            .trim()
        )
      ) {

        this.mensajeError =
          'El nombre de la tarjeta solo puede contener letras.';

        return false;
      }


      // Limpia espacios.
      const numeroLimpio =
        this.numeroTarjeta
          .replace(
            /\s/g,
            ''
          );


      // Valida número.
      if (
        !/^\d{16}$/.test(
          numeroLimpio
        )
      ) {

        this.mensajeError =
          'El número de tarjeta debe contener 16 dígitos.';

        return false;
      }


      // Valida vencimiento.
      if (
        !/^\d{2}\/\d{2}$/.test(
          this.vencimientoTarjeta
        )
      ) {

        this.mensajeError =
          'La fecha de vencimiento debe utilizar el formato MM/AA.';

        return false;
      }


      // Obtiene mes.
      const mes =
        Number(
          this.vencimientoTarjeta
            .substring(
              0,
              2
            )
        );


      // Obtiene año.
      const anioCorto =
        Number(
          this.vencimientoTarjeta
            .substring(
              3,
              5
            )
        );

      const anio =
        2000 +
        anioCorto;


      // Valida mes.
      if (
        mes < 1 ||
        mes > 12
      ) {

        this.mensajeError =
          'El mes de vencimiento de la tarjeta no es válido.';

        return false;
      }


      // Fecha actual.
      const fechaActual =
        new Date();

      const mesActual =
        fechaActual.getMonth() +
        1;

      const anioActual =
        fechaActual.getFullYear();


      // Valida año.
      if (
        anio <
        anioActual
      ) {

        this.mensajeError =
          'La tarjeta está vencida.';

        return false;
      }


      // Valida mes actual.
      if (
        anio === anioActual &&
        mes < mesActual
      ) {

        this.mensajeError =
          'La tarjeta está vencida.';

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

    // SINPE no necesita
    // información adicional.
    return true;
  }


  // Realiza el pago.
  pagar(): void {

    // Verifica pedido.
    if (
      !this.pedido
    ) {
      return;
    }

    // Verifica método.
    if (
      this.idMetodoPagoSeleccionado <=
      0
    ) {

      this.mensajeError =
        'Selecciona un método de pago antes de continuar.';

      return;
    }

    // Verifica fondos.
    if (
      !this.validarFondos(
        true
      )
    ) {
      return;
    }

    // Verifica datos.
    if (
      !this.validarDatosMetodo()
    ) {
      return;
    }

    // Evita doble pago.
    if (
      this.procesandoPago
    ) {
      return;
    }

    // Obtiene headers.
    const headers =
      this.obtenerHeaders();

    // Valida sesión.
    if (!headers) {

      this.mensajeError =
        'No existe una sesión activa.';

      return;
    }

    // Activa proceso.
    this.procesandoPago =
      true;

    // Limpia mensajes.
    this.mensajeError =
      '';

    this.mensajeExito =
      '';


    // Datos enviados.
    const datosPago = {

      idPedido:
        this.pedido.idPedido,

      idMetodoPago:
        this.idMetodoPagoSeleccionado
    };


    // Envía pago.
    this.http
      .post<RespuestaPago>(
        `${this.apiPagos}/pagar`,
        datosPago,
        {
          headers
        }
      )
      .subscribe({

        // Pago correcto.
        next: respuesta => {

          console.log(
            'Pago realizado:',
            respuesta
          );

          this.mensajeExito =
            'Pago realizado correctamente.';

          this.procesandoPago =
            false;

          this.changeDetector
            .detectChanges();


          // Regresa al detalle.
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


        // Error.
        error: (
          error: HttpErrorResponse
        ) => {

          console.error(
            'Error al realizar el pago:',
            error
          );


          if (
            typeof error.error ===
            'string' &&
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


          this.procesandoPago =
            false;

          this.changeDetector
            .detectChanges();
        }
      });
  }


  // Cancela el pago.
  cancelar(): void {

    this.router.navigate([
      '/detalle-pedido',
      this.idPedido
    ]);
  }
}
