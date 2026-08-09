// Importa CommonModule para poder usar directivas comunes de Angular
// como *ngIf, *ngFor y pipes dentro del HTML.
import { CommonModule } from '@angular/common';

// Importa Component para poder crear el componente.
//
// ChangeDetectorRef permite forzar la actualización visual
// cuando los datos llegan desde la API.
import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

// Importa ActivatedRoute para obtener el id del pedido
// que viene dentro de la dirección URL.
//
// Router permite navegar hacia otras páginas.
import {
  ActivatedRoute,
  Router
} from '@angular/router';

// Importa HttpClient para consultar la API.
//
// HttpHeaders permite enviar el token JWT.
//
// HttpErrorResponse permite manejar errores HTTP.
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders
} from '@angular/common/http';


// Esta interfaz representa la información principal
// del pedido que se mostrará en pantalla.
interface PedidoDetalle {

  // Identificador único del pedido.
  idPedido: number;

  // Identificador del usuario que realizó la compra.
  idUsuario: number;

  // Fecha en que se realizó el pedido.
  fechaPedido: string;

  // Estado actual del pedido.
  estado: string;

  // Subtotal antes de impuestos y descuentos.
  subtotal: number;

  // Monto correspondiente al impuesto.
  impuesto: number;

  // Monto de descuento aplicado.
  descuento: number;

  // Total final de la compra.
  total: number;

  // Dirección registrada para la entrega.
  direccionEntrega: string | null;

  // Identificador del estado del pedido.
  idEstadoPedido: number | null;
}


// Configuración principal del componente.
@Component({

  // Nombre interno del componente.
  selector: 'app-detalle-pedido',

  // Indica que este componente funciona
  // de manera independiente.
  standalone: true,

  // Importa CommonModule para utilizar
  // directivas y pipes dentro del HTML.
  imports: [
    CommonModule
  ],

  // Archivo HTML relacionado con este componente.
  templateUrl: './detalle-pedido.html',

  // Archivo CSS relacionado con este componente.
  styleUrl: './detalle-pedido.css'
})


// Esta clase contiene toda la lógica
// de la pantalla Detalle de Pedido.
export class DetallePedido {

  // Guarda la información del pedido
  // recibida desde la API.
  pedido: PedidoDetalle | null = null;

  // Guarda el identificador obtenido desde la URL.
  idPedido: number = 0;

  // Indica si la información todavía se está cargando.
  cargando: boolean = true;

  // Guarda un mensaje cuando ocurre algún error.
  mensajeError: string = '';

  // Dirección base del controlador Pedidos.
  private readonly apiUrl =
    'https://localhost:7196/api/Pedidos';


  // El constructor recibe las dependencias
  // necesarias mediante inyección de dependencias.
  constructor(

    // ActivatedRoute permite leer datos
    // que vienen dentro de la URL.
    private route: ActivatedRoute,

    // HttpClient permite consultar la API.
    private http: HttpClient,

    // Router permite navegar entre páginas.
    private router: Router,

    // Permite actualizar manualmente la pantalla.
    private changeDetector: ChangeDetectorRef
  ) {

    // Obtiene el id del pedido desde una ruta como:
    // /detalle-pedido/6
    this.obtenerIdPedido();
  }


  // Este método obtiene el identificador
  // del pedido desde la dirección URL.
  obtenerIdPedido(): void {

    // Busca el parámetro llamado "id"
    // definido en la ruta detalle-pedido/:id.
    const idTexto =
      this.route.snapshot.paramMap.get('id');


    // Intenta convertir el valor recibido
    // a un número entero.
    this.idPedido =
      Number(idTexto);


    // Comprueba que el identificador
    // realmente sea un número válido.
    if (
      !this.idPedido ||
      this.idPedido <= 0
    ) {

      // Muestra un error si el id no es válido.
      this.mensajeError =
        'El pedido seleccionado no es válido.';

      // Termina el estado de carga.
      this.cargando = false;

      // Actualiza la pantalla.
      this.changeDetector.detectChanges();

      // Detiene el proceso.
      return;
    }


    // Si el identificador es válido,
    // consulta la información del pedido.
    this.cargarPedido();
  }


  // Este método consulta en la API
  // el pedido correspondiente al identificador recibido.
  cargarPedido(): void {

    // Indica que comienza la carga.
    this.cargando = true;

    // Limpia cualquier error anterior.
    this.mensajeError = '';


    // Obtiene el token JWT guardado
    // cuando el cliente inició sesión.
    const token =
      localStorage.getItem('token');


    // Comprueba que exista una sesión activa.
    if (!token) {

      // Muestra un mensaje si no existe token.
      this.mensajeError =
        'No existe una sesión activa.';

      // Termina la carga.
      this.cargando = false;

      // Actualiza la pantalla.
      this.changeDetector.detectChanges();

      // Detiene el método.
      return;
    }


    // Crea los encabezados HTTP
    // que se enviarán hacia la API.
    const headers =
      new HttpHeaders({

        // Envía el token utilizando
        // el formato Bearer.
        Authorization:
          `Bearer ${token}`
      });


    // Realiza la petición GET.
    //
    // Por ejemplo:
    // GET /api/Pedidos/6
    this.http.get<PedidoDetalle>(
      `${this.apiUrl}/${this.idPedido}`,
      {
        headers: headers
      }
    ).subscribe({

      // Este bloque se ejecuta
      // cuando la API responde correctamente.
      next: (respuesta: PedidoDetalle) => {

        // Guarda el pedido recibido.
        this.pedido =
          respuesta;

        // Finaliza el estado de carga.
        this.cargando = false;

        // Actualiza la pantalla.
        this.changeDetector.detectChanges();
      },


      // Este bloque se ejecuta
      // cuando ocurre algún error.
      error: (error: HttpErrorResponse) => {

        // Muestra el error completo
        // en la consola durante las pruebas.
        console.error(
          'Error al cargar el detalle del pedido:',
          error
        );


        // Si la API devuelve 401,
        // significa que la sesión no es válida.
        if (error.status === 401) {

          this.mensajeError =
            'La sesión no es válida. Inicia sesión nuevamente.';
        }

        // Si devuelve 403,
        // significa que el pedido pertenece a otro usuario.
        else if (error.status === 403) {

          this.mensajeError =
            'No tienes permiso para consultar este pedido.';
        }

        // Si devuelve 404,
        // significa que el pedido no existe.
        else if (error.status === 404) {

          this.mensajeError =
            'El pedido no existe.';
        }

        // Para cualquier otro error,
        // muestra un mensaje general.
        else {

          this.mensajeError =
            'No se pudo cargar el detalle del pedido.';
        }


        // Termina el estado de carga.
        this.cargando = false;

        // Actualiza la pantalla.
        this.changeDetector.detectChanges();
      }
    });
  }


  // Este método permite regresar
  // a la lista de pedidos del cliente.
  volverMisPedidos(): void {

    // Navega hacia Mis Pedidos.
    this.router.navigate([
      '/mis-pedidos'
    ]);
  }


  // Este método permite regresar
  // al dashboard principal del cliente.
  volverInicio(): void {

    // Navega hacia el dashboard.
    this.router.navigate([
      '/dashboard'
    ]);
  }
}
