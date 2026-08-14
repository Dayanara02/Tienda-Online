// Permite usar directivas comunes de Angular.
import { CommonModule } from '@angular/common';

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

// Permite usar iconos de Angular Material.
import { MatIconModule } from '@angular/material/icon';

// Permite usar botones de Angular Material.
import { MatButtonModule } from '@angular/material/button';

// Permite usar campos de texto.
import { MatInputModule } from '@angular/material/input';

// Permite utilizar ngModel.
import { FormsModule } from '@angular/forms';

// Permite usar botones de PrimeNG.
import { ButtonModule } from 'primeng/button';

// Representa un método de pago.
interface MetodoPago {
  idMetodoPago: number;
  nombre: string;
  descripcion: string | null;
}

// Representa los datos del pedido.
interface PedidoPago {
  idPedido: number;
  estado: string;
  estadoPago: string;
  total: number;
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

// Configura la pantalla de pago.
@Component({
  selector: 'app-pago-pedido',

  // Indica que es independiente.
  standalone: true,

  // Módulos utilizados.
  imports: [
    CommonModule,
    FormsModule,
    MatIconModule,
    MatButtonModule,
    MatInputModule,
    ButtonModule
  ],

  // Archivo HTML.
  templateUrl: './pago-pedido.html',

  // Archivo CSS.
  styleUrl: './pago-pedido.css'
})
export class PagoPedido {

  // Guarda el id del pedido.
  idPedido = 0;

  // Guarda el pedido actual.
  pedido: PedidoPago | null = null;

  // Guarda los métodos disponibles.
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

  // Indica si el saldo no alcanza.
  saldoInsuficiente = false;

  // Cuenta usada para transferencia.
  readonly cuentaTransferenciaEsencia =
    'CR00 0000 0000 0000 0000 00';

  // Guarda la referencia bancaria.
  referenciaTransferencia = '';

  // Guarda el titular de la tarjeta.
  nombreTarjeta = '';

  // Guarda el número de tarjeta.
  numeroTarjeta = '';

  // Guarda la fecha MM/AA.
  vencimientoTarjeta = '';

  // Guarda el CVV.
  cvvTarjeta = '';

  // Indica si está cargando.
  cargando = true;

  // Indica si está pagando.
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

  // Recibe los servicios necesarios.
  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private router: Router,
    private changeDetector: ChangeDetectorRef
  ) {
    // Obtiene el pedido al abrir.
    this.obtenerIdPedido();
  }

  // Obtiene el id del pedido.
  obtenerIdPedido(): void {

    // Lee el parámetro id.
    const idTexto =
      this.route.snapshot.paramMap.get(
        'id'
      );

    // Convierte el id a número.
    this.idPedido =
      Number(idTexto);

    // Valida el identificador.
    if (
      !this.idPedido ||
      this.idPedido <= 0
    ) {
      // Muestra el error.
      this.mensajeError =
        'El pedido seleccionado no es válido.';

      // Finaliza la carga.
      this.cargando = false;

      // Actualiza la pantalla.
      this.changeDetector.detectChanges();

      return;
    }

    // Consulta el pedido.
    this.cargarPedido();
  }

  // Crea los encabezados JWT.
  private obtenerHeaders():
    HttpHeaders | null {

    // Obtiene el token guardado.
    const token =
      localStorage.getItem(
        'token'
      );

    // Comprueba que exista.
    if (!token) {
      return null;
    }

    // Devuelve Authorization.
    return new HttpHeaders({
      Authorization:
        `Bearer ${token}`
    });
  }

  // Consulta el pedido.
  cargarPedido(): void {

    // Activa la carga.
    this.cargando = true;

    // Limpia errores.
    this.mensajeError = '';

    // Obtiene los headers.
    const headers =
      this.obtenerHeaders();

    // Comprueba la sesión.
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
        headers
      }
    ).subscribe({

      // Se ejecuta si responde bien.
      next: (
        respuesta: PedidoPago
      ) => {

        // Guarda el pedido.
        this.pedido =
          respuesta;

        // Comprueba que pueda pagarse.
        if (!respuesta.puedePagar) {
          this.mensajeError =
            'Este pedido ya no se encuentra disponible para pago.';

          this.cargando =
            false;

          this.changeDetector.detectChanges();

          return;
        }

        // Carga los métodos.
        this.cargarMetodosPago();
      },

      // Se ejecuta si falla.
      error: (
        error: HttpErrorResponse
      ) => {

        // Muestra el error en consola.
        console.error(
          'Error al cargar el pedido:',
          error
        );

        // Sesión inválida.
        if (error.status === 401) {
          this.mensajeError =
            'La sesión no es válida. Inicia sesión nuevamente.';
        }

        // Sin permiso.
        else if (error.status === 403) {
          this.mensajeError =
            'No tienes permiso para pagar este pedido.';
        }

        // Pedido inexistente.
        else if (error.status === 404) {
          this.mensajeError =
            'El pedido no existe.';
        }

        // Cualquier otro error.
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

  // Consulta los métodos de pago.
  cargarMetodosPago(): void {

    // Obtiene los headers.
    const headers =
      this.obtenerHeaders();

    // Comprueba la sesión.
    if (!headers) {
      this.mensajeError =
        'No existe una sesión activa.';

      this.cargando = false;

      this.changeDetector.detectChanges();

      return;
    }

    // Consulta los métodos.
    this.http.get<MetodoPago[]>(
      `${this.apiMetodoPagos}/disponibles`,
      {
        headers
      }
    ).subscribe({

      // Guarda los métodos recibidos.
      next: (
        respuesta: MetodoPago[]
      ) => {

        // Finaliza la carga.
        this.cargando =
          false;

        // Actualiza la pantalla.
        this.changeDetector.detectChanges();
      },

      // Maneja errores.
      error: (
        error: HttpErrorResponse
      ) => {

        // Muestra el error en consola.
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

    // Guarda el id.
    this.idMetodoPagoSeleccionado =
      metodo.idMetodoPago;

    // Guarda el nombre.
    this.nombreMetodoSeleccionado =
      metodo.nombre;

    // Limpia errores.
    this.mensajeError =
      '';

    // Reinicia el saldo.
    this.saldoInsuficiente =
      false;

    // Reinicia el faltante.
    this.montoFaltante =
      0;

    // Reinicia el sobrante.
    this.saldoRestante =
      0;
  }

  // Comprueba si es transferencia.
  esTransferencia(): boolean {

    // Busca la palabra transferencia.
    return this.nombreMetodoSeleccionado
      .toLowerCase()
      .includes(
        'transferencia'
      );
  }

  // Comprueba si es tarjeta.
  esTarjeta(): boolean {

    // Busca la palabra tarjeta.
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
      Number(input.value);

    // Guarda cero si falla.
    this.montoDisponible =
      isNaN(valor)
        ? 0
        : valor;

    // Valida el saldo.
    this.validarFondos(
      false
    );
  }

  // Guarda la referencia.
  cambiarReferenciaTransferencia(
    event: Event
  ): void {

    // Obtiene el input.
    const input =
      event.target as HTMLInputElement;

    // Guarda el valor.
    this.referenciaTransferencia =
      input.value;
  }

  // Guarda el titular.
  cambiarNombreTarjeta(
    event: Event
  ): void {

    // Obtiene el input.
    const input =
      event.target as HTMLInputElement;

    // Guarda el valor.
    this.nombreTarjeta =
      input.value;
  }

  // Guarda el número.
  cambiarNumeroTarjeta(
    event: Event
  ): void {

    // Obtiene el input.
    const input =
      event.target as HTMLInputElement;

    // Guarda el valor.
    this.numeroTarjeta =
      input.value;
  }

  // Guarda el vencimiento.
  cambiarVencimientoTarjeta(
    event: Event
  ): void {

    // Obtiene el input.
    const input =
      event.target as HTMLInputElement;

    // Guarda el valor.
    this.vencimientoTarjeta =
      input.value;
  }

  // Guarda el CVV.
  cambiarCvvTarjeta(
    event: Event
  ): void {

    // Obtiene el input.
    const input =
      event.target as HTMLInputElement;

    // Guarda el valor.
    this.cvvTarjeta =
      input.value;
  }

  // Obtiene el total del pedido.
  obtenerTotalPedido(): number {

    // Devuelve cero si no existe.
    return this.pedido?.total ?? 0;
  }

  // Comprueba si el saldo alcanza.
  validarFondos(
    mostrarMensaje: boolean = true
  ): boolean {

    // Obtiene el total.
    const total =
      this.obtenerTotalPedido();

    // Comprueba el total.
    if (total <= 0) {
      return false;
    }

    // Reinicia cálculos.
    this.montoFaltante =
      0;

    // Reinicia sobrante.
    this.saldoRestante =
      0;

    // Reinicia indicador.
    this.saldoInsuficiente =
      false;

    // Comprueba el monto.
    if (
      this.montoDisponible <= 0
    ) {
      if (mostrarMensaje) {
        this.mensajeError =
          'Indica cuánto dinero tienes disponible.';
      }

      return false;
    }

    // Comprueba si falta dinero.
    if (
      this.montoDisponible <
      total
    ) {

      // Calcula el faltante.
      this.montoFaltante =
        total -
        this.montoDisponible;

      // Activa el indicador.
      this.saldoInsuficiente =
        true;

      // Muestra el mensaje.
      if (mostrarMensaje) {
        this.mensajeError =
          'Pago no completado. No tienes suficiente saldo para realizar esta compra.';
      }

      return false;
    }

    // Calcula el sobrante.
    this.saldoRestante =
      this.montoDisponible -
      total;

    // Limpia errores anteriores.
    if (
      this.mensajeError.includes(
        'suficiente saldo'
      )
    ) {
      this.mensajeError =
        '';
    }

    // Indica que alcanza.
    return true;
  }

  // Comprueba si el saldo alcanza.
  tieneFondosSuficientes(): boolean {

    // Obtiene el total.
    const total =
      this.obtenerTotalPedido();

    // Compara el saldo.
    return (
      total > 0 &&
      this.montoDisponible >= total
    );
  }

  // Valida los datos del método.
  validarDatosMetodo(): boolean {

    // Valida transferencia.
    if (this.esTransferencia()) {

      // Comprueba la referencia.
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
    if (this.esTarjeta()) {

      // Comprueba el titular.
      if (
        !this.nombreTarjeta
          .trim()
      ) {
        this.mensajeError =
          'Ingresa el nombre que aparece en la tarjeta.';

        return false;
      }

      // Comprueba que el nombre tenga solo letras y espacios.
      if (
        !/^[A-Za-zÁÉÍÓÚáéíóúÑñÜü\s]+$/.test(
          this.nombreTarjeta.trim()
        )
      ) {
        this.mensajeError =
          'El nombre de la tarjeta solo puede contener letras.';

        return false;
      }

      // Elimina espacios del número.
      const numeroLimpio =
        this.numeroTarjeta
          .replace(
            /\s/g,
            ''
          );

      // Comprueba los 16 dígitos.
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

      // Obtiene el mes.
      const mes =
        Number(
          this.vencimientoTarjeta
            .substring(
              0,
              2
            )
        );

      // Obtiene el año corto.
      const anioCorto =
        Number(
          this.vencimientoTarjeta
            .substring(
              3,
              5
            )
        );

      // Convierte 26 en 2026.
      const anio =
        2000 +
        anioCorto;

      // Comprueba el rango del mes.
      if (
        mes < 1 ||
        mes > 12
      ) {
        this.mensajeError =
          'El mes de vencimiento de la tarjeta no es válido.';

        return false;
      }

      // Obtiene la fecha actual.
      const fechaActual =
        new Date();

      // Obtiene el mes actual.
      const mesActual =
        fechaActual.getMonth() + 1;

      // Obtiene el año actual.
      const anioActual =
        fechaActual.getFullYear();

      // Comprueba si el año ya pasó.
      if (
        anio <
        anioActual
      ) {
        this.mensajeError =
          'La tarjeta está vencida.';

        return false;
      }

      // Comprueba si el mes ya pasó.
      if (
        anio === anioActual &&
        mes < mesActual
      ) {
        this.mensajeError =
          'La tarjeta está vencida.';

        return false;
      }

      // Comprueba el CVV.
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

    // Indica que los datos son válidos.
    return true;
  }

  // Envía el pago al backend.
  pagar(): void {

    // Comprueba que exista pedido.
    if (!this.pedido) {
      return;
    }

    // Comprueba el método.
    if (
      this.idMetodoPagoSeleccionado <= 0
    ) {
      this.mensajeError =
        'Selecciona un método de pago antes de continuar.';

      return;
    }

    // Comprueba el saldo.
    if (
      !this.validarFondos(
        true
      )
    ) {
      return;
    }

    // Comprueba los datos.
    if (
      !this.validarDatosMetodo()
    ) {
      return;
    }

    // Evita pagos dobles.
    if (this.procesandoPago) {
      return;
    }

    // Obtiene autorización.
    const headers =
      this.obtenerHeaders();

    // Comprueba la sesión.
    if (!headers) {
      this.mensajeError =
        'No existe una sesión activa.';

      return;
    }

    // Activa el procesamiento.
    this.procesandoPago =
      true;

    // Limpia errores.
    this.mensajeError =
      '';

    // Limpia mensajes exitosos.
    this.mensajeExito =
      '';

    // Prepara el pago.
    const datosPago = {

      // Envía el pedido.
      idPedido:
        this.pedido.idPedido,

      // Envía el método.
      idMetodoPago:
        this.idMetodoPagoSeleccionado
    };

    // Envía el pago.
    this.http.post<RespuestaPago>(
      `${this.apiPagos}/pagar`,
      datosPago,
      {
        headers
      }
    ).subscribe({

      // Se ejecuta si funciona.
      next: (
        respuesta: RespuestaPago
      ) => {

        // Muestra la respuesta.
        console.log(
          'Pago realizado:',
          respuesta
        );

        // Muestra éxito.
        this.mensajeExito =
          'Pago realizado correctamente.';

        // Finaliza el proceso.
        this.procesandoPago =
          false;

        // Actualiza la pantalla.
        this.changeDetector.detectChanges();

        // Regresa al pedido.
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

      // Se ejecuta si falla.
      error: (
        error: HttpErrorResponse
      ) => {

        // Muestra el error.
        console.error(
          'Error al realizar el pago:',
          error
        );

        // Comprueba texto simple.
        if (
          typeof error.error ===
          'string' &&
          error.error
        ) {
          this.mensajeError =
            error.error;
        }

        // Comprueba campo mensaje.
        else if (
          error.error?.mensaje
        ) {
          this.mensajeError =
            error.error.mensaje;
        }

        // Usa un mensaje general.
        else {
          this.mensajeError =
            'No se pudo completar el pago.';
        }

        // Finaliza el proceso.
        this.procesandoPago =
          false;

        // Actualiza la pantalla.
        this.changeDetector.detectChanges();
      }
    });
  }

  // Cancela y vuelve al pedido.
  cancelar(): void {

    // Regresa al detalle.
    this.router.navigate([
      '/detalle-pedido',
      this.idPedido
    ]);
  }
}
