// Importa CommonModule para utilizar directivas como
// *ngIf, *ngFor y pipes dentro del HTML.
import { CommonModule } from '@angular/common';

// Importa las herramientas necesarias para crear
// el componente y actualizar la pantalla manualmente.
import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

// ActivatedRoute permite obtener el id del pedido
// directamente desde la dirección URL.
//
// Router permite navegar hacia otras páginas.
import {
  ActivatedRoute,
  Router
} from '@angular/router';

// HttpClient permite consultar la API.
//
// HttpErrorResponse permite manejar los errores HTTP.
//
// HttpHeaders permite enviar el token JWT.
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders
} from '@angular/common/http';


// =========================================================
// INTERFAZ DEL DETALLE DE CADA PRODUCTO
// =========================================================

// Esta interfaz representa cada producto
// que pertenece al pedido.
interface DetalleProducto {

  // Identificador del detalle.
  idDetallePedido: number;

  // Identificador del producto.
  idProducto: number;

  // Nombre real del producto.
  nombreProducto: string;

  // Cantidad comprada.
  cantidad: number;

  // Precio de una unidad.
  precioUnitario: number;

  // Descuento aplicado al producto.
  descuento: number;

  // Impuesto aplicado al producto.
  impuesto: number;

  // Subtotal correspondiente a ese producto.
  subtotal: number;
}


// =========================================================
// INTERFAZ DEL PEDIDO COMPLETO
// =========================================================

// Esta interfaz representa toda la información
// que devuelve GET /api/Pedidos/{id}.
interface PedidoDetalle {

  // Identificador del pedido.
  idPedido: number;

  // Usuario propietario del pedido.
  idUsuario: number;

  // Fecha en que se realizó la compra.
  fechaPedido: string;

  // Estado general del pedido.
  estado: string;

  // Subtotal general.
  subtotal: number;

  // Impuesto general.
  impuesto: number;

  // Descuento general.
  descuento: number;

  // Total final.
  total: number;

  // Dirección donde se entregará el pedido.
  direccionEntrega: string | null;

  // Identificador del estado general.
  idEstadoPedido: number | null;

  // Estado que verá el Cliente:
  // Pendiente, Pagado o Cancelado.
  estadoPago: string;

  // Método utilizado para pagar.
  // Puede venir null si todavía no se ha pagado.
  metodoPago: string | null;

  // Fecha en que se realizó el pago.
  // Puede venir null si todavía no existe pago.
  fechaPago: string | null;

  // Indica si debe aparecer el botón
  // Pagar pedido.
  puedePagar: boolean;

  // Lista de productos comprados.
  detalles: DetalleProducto[];
}


// =========================================================
// COMPONENTE
// =========================================================

@Component({

  // Nombre interno del componente.
  selector: 'app-detalle-pedido',

  // Indica que funciona como componente independiente.
  standalone: true,

  // Permite utilizar *ngIf, *ngFor y pipes.
  imports: [
    CommonModule
  ],

  // Archivo HTML relacionado.
  templateUrl: './detalle-pedido.html',

  // Archivo CSS relacionado.
  styleUrl: './detalle-pedido.css'
})


// Clase principal de la página.
export class DetallePedido {

  // Guarda toda la información
  // recibida desde la API.
  pedido: PedidoDetalle | null = null;

  // Guarda el identificador del pedido
  // obtenido desde la URL.
  idPedido: number = 0;

  // Controla el mensaje de carga.
  cargando: boolean = true;

  // Guarda los mensajes de error.
  mensajeError: string = '';

  // URL principal del controlador Pedidos.
  private readonly apiUrl =
    'https://localhost:7196/api/Pedidos';


  // Constructor del componente.
  constructor(

    // Permite leer el parámetro id de la URL.
    private route: ActivatedRoute,

    // Permite realizar peticiones HTTP.
    private http: HttpClient,

    // Permite navegar entre páginas.
    private router: Router,

    // Permite actualizar manualmente la interfaz.
    private changeDetector: ChangeDetectorRef
  ) {

    // Apenas se abre la página,
    // obtiene el id del pedido.
    this.obtenerIdPedido();
  }


  // =========================================================
  // OBTENER ID DESDE LA URL
  // =========================================================

  // Este método lee el id recibido
  // en una dirección como:
  //
  // /detalle-pedido/5
  obtenerIdPedido(): void {

    // Obtiene el parámetro llamado id.
    const idTexto =
      this.route.snapshot.paramMap.get('id');


    // Convierte el valor recibido a número.
    this.idPedido =
      Number(idTexto);


    // Comprueba que el identificador sea válido.
    if (
      !this.idPedido ||
      this.idPedido <= 0
    ) {

      // Muestra un mensaje si el id no es correcto.
      this.mensajeError =
        'El pedido seleccionado no es válido.';

      // Termina la carga.
      this.cargando = false;

      // Actualiza la pantalla.
      this.changeDetector.detectChanges();

      // Detiene el método.
      return;
    }


    // Si el identificador es válido,
    // consulta el pedido en la API.
    this.cargarPedido();
  }


  // =========================================================
  // CARGAR EL PEDIDO
  // =========================================================

  // Consulta el detalle completo
  // del pedido seleccionado.
  cargarPedido(): void {

    // Inicia el estado de carga.
    this.cargando = true;

    // Limpia cualquier error anterior.
    this.mensajeError = '';


    // Obtiene el token JWT guardado
    // después de iniciar sesión.
    const token =
      localStorage.getItem('token');


    // Comprueba que exista una sesión activa.
    if (!token) {

      // Informa que no existe token.
      this.mensajeError =
        'No existe una sesión activa.';

      // Termina la carga.
      this.cargando = false;

      // Actualiza la pantalla.
      this.changeDetector.detectChanges();

      // Detiene el método.
      return;
    }


    // Crea los encabezados que se enviarán
    // hacia la API.
    const headers =
      new HttpHeaders({

        // Agrega el token JWT.
        Authorization:
          `Bearer ${token}`
      });


    // Realiza la consulta:
    //
    // GET /api/Pedidos/5
    this.http.get<PedidoDetalle>(
      `${this.apiUrl}/${this.idPedido}`,
      {
        headers: headers
      }
    ).subscribe({

      // Este bloque se ejecuta
      // cuando la API responde correctamente.
      next: (respuesta: PedidoDetalle) => {

        // Muestra la respuesta en consola.
        // Sirve durante las pruebas del proyecto.
        console.log(
          'Detalle del pedido recibido:',
          respuesta
        );

        // Guarda el pedido completo.
        this.pedido =
          respuesta;

        // Termina la carga.
        this.cargando = false;

        // Actualiza la pantalla.
        this.changeDetector.detectChanges();
      },


      // Este bloque se ejecuta
      // cuando ocurre algún error.
      error: (error: HttpErrorResponse) => {

        // Muestra información del error
        // en la consola del navegador.
        console.error(
          'Error al cargar el detalle del pedido:',
          error
        );


        // Si el token no es válido.
        if (error.status === 401) {

          this.mensajeError =
            'La sesión no es válida. Inicia sesión nuevamente.';
        }

        // Si intenta consultar un pedido
        // perteneciente a otro usuario.
        else if (error.status === 403) {

          this.mensajeError =
            'No tienes permiso para consultar este pedido.';
        }

        // Si el pedido no existe.
        else if (error.status === 404) {

          this.mensajeError =
            'El pedido no existe.';
        }

        // Si no puede comunicarse con la API.
        else if (error.status === 0) {

          this.mensajeError =
            'No se pudo conectar con el servidor.';
        }

        // Cualquier otro error.
        else {

          this.mensajeError =
            'No se pudo cargar el detalle del pedido.';
        }


        // Termina la carga.
        this.cargando = false;

        // Actualiza la pantalla.
        this.changeDetector.detectChanges();
      }
    });
  }


  // =========================================================
  // PAGAR PEDIDO
  // =========================================================

  // Este método se ejecuta cuando el Cliente
  // presiona el botón "Pagar pedido".
  pagarPedido(): void {

    // Comprueba que exista información del pedido.
    if (!this.pedido) {

      return;
    }


    // Evita abrir la pantalla de pago
    // si el backend indica que el pedido
    // ya no puede pagarse.
    if (!this.pedido.puedePagar) {

      return;
    }


    // Navega hacia la nueva pantalla de pago
    // enviando el identificador del pedido.
    //
    // Por ejemplo:
    //
    // /pago-pedido/5
    this.router.navigate([
      '/pago-pedido',
      this.pedido.idPedido
    ]);
  }


  // =========================================================
  // VOLVER A MIS PEDIDOS
  // =========================================================

  // Regresa al historial de pedidos.
  volverMisPedidos(): void {

    this.router.navigate([
      '/mis-pedidos'
    ]);
  }


  // =========================================================
  // VOLVER AL INICIO
  // =========================================================

  // Regresa al dashboard del Cliente.
  volverInicio(): void {

    this.router.navigate([
      '/dashboard'
    ]);
  }
}
