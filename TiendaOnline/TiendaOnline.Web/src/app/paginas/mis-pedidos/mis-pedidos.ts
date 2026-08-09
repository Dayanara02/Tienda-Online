// Importa CommonModule para poder utilizar
// funciones comunes de Angular dentro del HTML,
// como *ngIf, *ngFor y los pipes.
import { CommonModule } from '@angular/common';

// Importa Component para crear el componente.
//
// ChangeDetectorRef permite indicarle a Angular
// que actualice manualmente la pantalla cuando sea necesario.
import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

// Importa HttpClient para realizar peticiones hacia la API.
//
// HttpErrorResponse permite manejar los errores HTTP.
//
// HttpHeaders permite enviar el token JWT
// dentro de los encabezados de la petición.
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders
} from '@angular/common/http';

// Importa Router para navegar
// hacia otras páginas del sistema.
import { Router } from '@angular/router';


// Esta interfaz representa la estructura
// de un pedido recibido desde la API.
interface Pedido {

  // Identificador único del pedido.
  idPedido: number;

  // Fecha en la que se realizó el pedido.
  fechaPedido: string;

  // Estado actual del pedido.
  // Por ejemplo: Pendiente, Confirmado, Pagado o Entregado.
  estado: string;

  // Subtotal de la compra antes
  // de impuestos y descuentos.
  subtotal: number;

  // Monto correspondiente al impuesto.
  impuesto: number;

  // Monto correspondiente al descuento.
  descuento: number;

  // Total final de la compra.
  total: number;

  // Dirección donde debe entregarse el pedido.
  // Puede ser null si no existe una dirección registrada.
  direccionEntrega: string | null;
}


// Configuración principal del componente.
@Component({

  // Nombre interno del componente.
  selector: 'app-mis-pedidos',

  // Indica que este componente
  // funciona de manera independiente.
  standalone: true,

  // Importa CommonModule para utilizar
  // directivas y pipes dentro del HTML.
  imports: [
    CommonModule
  ],

  // Archivo HTML asociado al componente.
  templateUrl: './mis-pedidos.html',

  // Archivo CSS asociado al componente.
  styleUrl: './mis-pedidos.css'
})


// Esta clase contiene toda la lógica
// de la pantalla Mis Pedidos.
export class MisPedidos {

  // Guarda los pedidos que devuelve la API.
  pedidos: Pedido[] = [];

  // Indica si actualmente
  // se están consultando los pedidos.
  cargando: boolean = true;

  // Guarda el mensaje que se mostrará
  // si ocurre algún problema.
  mensajeError: string = '';

  // Dirección base del controlador Pedidos
  // dentro de nuestra API.
  private readonly apiUrl =
    'https://localhost:7196/api/Pedidos';


  // El constructor recibe las dependencias
  // necesarias mediante inyección de dependencias.
  constructor(

    // Permite realizar peticiones HTTP.
    private http: HttpClient,

    // Permite navegar entre páginas.
    private router: Router,

    // Permite actualizar manualmente
    // lo que Angular muestra en pantalla.
    private changeDetector: ChangeDetectorRef
  ) {

    // Cuando se abre esta página,
    // carga automáticamente los pedidos del cliente.
    this.cargarPedidos();
  }


  // Este método consulta los pedidos
  // que pertenecen al cliente autenticado.
  cargarPedidos(): void {

    // Indica que comienza la carga.
    this.cargando = true;

    // Limpia cualquier error anterior.
    this.mensajeError = '';

    // Limpia la lista antes
    // de hacer una nueva consulta.
    this.pedidos = [];


    // Busca el token JWT guardado
    // cuando el cliente inició sesión.
    const token =
      localStorage.getItem('token');


    // Muestra en la consola solamente
    // si existe o no un token.
    //
    // No se imprime el token completo
    // para evitar mostrar información sensible.
    console.log(
      '¿Existe token?',
      token ? 'Sí' : 'No'
    );


    // Comprueba que exista un token.
    if (!token) {

      // Muestra un mensaje si
      // no existe una sesión activa.
      this.mensajeError =
        'No existe una sesión activa. Inicia sesión nuevamente.';

      // Termina el estado de carga.
      this.cargando = false;

      // Actualiza la pantalla.
      this.changeDetector.detectChanges();

      // Detiene el método.
      return;
    }


    // Crea los encabezados que serán
    // enviados junto con la petición.
    const headers =
      new HttpHeaders({

        // Envía el token utilizando
        // el formato Bearer esperado por la API.
        Authorization:
          `Bearer ${token}`
      });


    // Este mensaje solamente sirve
    // para comprobar durante las pruebas
    // que Angular llegó hasta la petición.
    console.log(
      'Consultando GET /api/Pedidos/mis-pedidos...'
    );


    // Realiza la petición GET hacia
    // el endpoint de Mis Pedidos.
    this.http.get<Pedido[]>(

      // Dirección completa del endpoint.
      `${this.apiUrl}/mis-pedidos`,

      {
        // Envía los encabezados
        // con el token JWT.
        headers: headers,

        // Si la petición tarda más de 10 segundos,
        // Angular deja de esperar la respuesta.
        timeout: 10000
      }

    ).subscribe({

      // Este bloque se ejecuta
      // cuando la API responde correctamente.
      next: (respuesta: Pedido[]) => {

        // Muestra en consola los pedidos
        // recibidos para facilitar las pruebas.
        console.log(
          'Pedidos recibidos:',
          respuesta
        );


        // Guarda los pedidos recibidos.
        //
        // Si por alguna razón la respuesta
        // fuera null, utiliza un arreglo vacío.
        this.pedidos =
          respuesta ?? [];


        // Quita el mensaje
        // "Cargando pedidos...".
        this.cargando = false;


        // Fuerza la actualización
        // de la pantalla.
        this.changeDetector.detectChanges();
      },


      // Este bloque se ejecuta
      // cuando la petición falla.
      error: (error: HttpErrorResponse) => {

        // Muestra el error completo
        // en la consola del navegador.
        console.error(
          'Error al cargar pedidos:',
          error
        );


        // Si la API devuelve 401,
        // el problema está relacionado con la sesión o token.
        if (error.status === 401) {

          this.mensajeError =
            'La sesión no es válida. Inicia sesión nuevamente.';
        }


        // Si devuelve 403,
        // significa que el usuario sí inició sesión,
        // pero no tiene permiso para este endpoint.
        else if (error.status === 403) {

          this.mensajeError =
            'Tu usuario no tiene permiso para consultar estos pedidos.';
        }


        // El estado 0 aparece cuando Angular
        // no logra completar la comunicación con la API.
        //
        // También puede ocurrir cuando la petición
        // supera el tiempo máximo configurado.
        else if (error.status === 0) {

          this.mensajeError =
            'No se pudo completar la conexión con la API.';
        }


        // Para cualquier otro código de error,
        // muestra un mensaje general.
        else {

          this.mensajeError =
            'No se pudieron cargar los pedidos.';
        }


        // Siempre termina el estado de carga
        // aunque la petición haya fallado.
        this.cargando = false;


        // Actualiza la pantalla
        // para mostrar el mensaje correspondiente.
        this.changeDetector.detectChanges();
      },


      // Este bloque se ejecuta cuando
      // la petición termina correctamente.
      complete: () => {

        // Mensaje solamente utilizado
        // durante las pruebas del sistema.
        console.log(
          'Consulta de pedidos finalizada.'
        );
      }
    });
  }


  // Este método abre la página
  // con el detalle de un pedido específico.
  verDetalle(
    idPedido: number
  ): void {

    // Navega hacia una dirección como:
    // /detalle-pedido/5
    this.router.navigate([
      '/detalle-pedido',
      idPedido
    ]);
  }


  // Este método regresa
  // al dashboard principal del cliente.
  volverDashboard(): void {

    // Navega hacia el dashboard.
    this.router.navigate([
      '/dashboard'
    ]);
  }
}
