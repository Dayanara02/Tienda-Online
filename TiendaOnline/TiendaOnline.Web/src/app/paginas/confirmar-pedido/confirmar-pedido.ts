// Importa CommonModule para poder usar directivas comunes de Angular
// como *ngIf, *ngFor y pipes dentro del HTML.
import { CommonModule } from '@angular/common';

// Importa Component para poder crear este componente de Angular.
import { Component } from '@angular/core';

// Importa FormsModule para poder usar [(ngModel)]
// y conectar los campos del formulario con variables de TypeScript.
import { FormsModule } from '@angular/forms';

// Importa Router para poder navegar entre páginas.
import { Router } from '@angular/router';

// Importa HttpClient para poder hacer peticiones HTTP hacia la API.
import { HttpClient, HttpHeaders } from '@angular/common/http';


// Define la estructura que debe tener cada producto del carrito.
interface ProductoCarrito {

  // Identificador único del producto.
  id: number;

  // Nombre del producto.
  nombre: string;

  // Marca del producto.
  marca: string;

  // Precio unitario.
  precio: number;

  // Cantidad disponible.
  disponibles: number;

  // Identificador de la categoría.
  categoriaId: number;

  // Imagen del producto.
  imagen: string;

  // Cantidad que el cliente desea comprar.
  cantidad: number;
}


// Define la estructura de una promoción.
interface Promocion {

  // Identificador de la promoción.
  id: number;

  // Nombre de la promoción.
  nombre: string;

  // Descripción de la promoción.
  descripcion: string;

  // Cantidad mínima necesaria para aplicar el descuento.
  cantidadMinima: number;

  // Porcentaje de descuento.
  porcentaje: number;

  // Icono utilizado para mostrar la promoción.
  icono: string;
}


// Define la estructura que se enviará a la API
// para cada producto incluido dentro del pedido.
interface DetalleConfirmarPedido {

  // Identificador del producto que se comprará.
  idProducto: number;

  // Cantidad que el cliente desea comprar.
  cantidad: number;
}


// Define la estructura completa que espera
// el endpoint POST /api/Pedidos/confirmar.
interface ConfirmarPedidoRequest {

  // Dirección donde se entregará el pedido.
  direccionEntrega: string;

  // Lista de productos y cantidades del pedido.
  detalles: DetalleConfirmarPedido[];
}


@Component({
  // Nombre interno del componente.
  selector: 'app-confirmar-pedido',

  // Indica que este componente funciona de manera independiente.
  standalone: true,

  // Importa los módulos necesarios para el HTML y los formularios.
  imports: [
    CommonModule,
    FormsModule
  ],

  // Archivo HTML que contiene la estructura visual.
  templateUrl: './confirmar-pedido.html',

  // Archivo CSS que contiene los estilos.
  styleUrl: './confirmar-pedido.css'
})


// Esta clase contiene toda la lógica
// de la pantalla donde el cliente confirma su compra.
export class ConfirmarPedido {

  // Guarda los productos que vienen del carrito.
  productos: ProductoCarrito[] = [];

  // Guarda la promoción seleccionada por el cliente.
  promocionActiva: Promocion | null = null;

  // Guarda la dirección escrita por el cliente.
  direccionEntrega: string = '';

  // Indica si actualmente se está enviando
  // el pedido hacia la API.
  procesando: boolean = false;

  // Guarda mensajes de error para mostrarlos en pantalla.
  mensajeError: string = '';

  // Guarda un mensaje cuando la compra se realiza correctamente.
  mensajeExito: string = '';

  // Guarda la dirección base de la API.
  // Esta dirección corresponde al proyecto TiendaOnline.API.
  private readonly apiUrl =
    'https://localhost:7196/api/Pedidos';


  // El constructor recibe HttpClient y Router
  // por medio de inyección de dependencias.
  constructor(
    private http: HttpClient,
    private router: Router
  ) {

    // Carga los productos guardados en el carrito.
    this.cargarCarrito();

    // Carga la promoción seleccionada.
    this.cargarPromocion();
  }


  // Este método obtiene los productos
  // que fueron guardados en localStorage.
  cargarCarrito(): void {

    // Busca el carrito guardado en el navegador.
    const carritoGuardado =
      localStorage.getItem('carrito');

    // Comprueba que realmente exista información.
    if (carritoGuardado) {

      // Convierte el texto JSON
      // nuevamente en un arreglo de productos.
      this.productos =
        JSON.parse(carritoGuardado);
    }
  }


  // Este método obtiene la promoción activa.
  cargarPromocion(): void {

    // Busca la promoción almacenada en localStorage.
    const promocionGuardada =
      localStorage.getItem('promocionActiva');

    // Comprueba que exista una promoción guardada.
    if (promocionGuardada) {

      // Convierte el JSON nuevamente
      // en un objeto de promoción.
      this.promocionActiva =
        JSON.parse(promocionGuardada);
    }
  }


  // Calcula la cantidad total
  // de artículos que hay en el carrito.
  get cantidadTotal(): number {

    // Recorre todos los productos
    // y suma sus cantidades.
    return this.productos.reduce(
      (
        total: number,
        producto: ProductoCarrito
      ) =>
        total + Number(producto.cantidad),
      0
    );
  }


  // Calcula el subtotal general
  // antes de aplicar descuentos.
  get subtotal(): number {

    // Recorre todos los productos
    // multiplicando precio por cantidad.
    return this.productos.reduce(
      (
        total: number,
        producto: ProductoCarrito
      ) =>
        total +
        (
          Number(producto.precio) *
          Number(producto.cantidad)
        ),
      0
    );
  }


  // Comprueba si el cliente cumple
  // los requisitos de la promoción.
  get promocionCumplida(): boolean {

    // Si no hay promoción activa,
    // no se puede aplicar descuento.
    if (!this.promocionActiva) {
      return false;
    }

    // Compara la cantidad comprada
    // con la cantidad mínima requerida.
    return (
      this.cantidadTotal >=
      this.promocionActiva.cantidadMinima
    );
  }


  // Calcula cuánto dinero
  // se descontará de la compra.
  get montoDescuento(): number {

    // Si no existe promoción
    // o no se cumple, el descuento es cero.
    if (
      !this.promocionActiva ||
      !this.promocionCumplida
    ) {
      return 0;
    }

    // Calcula el porcentaje de descuento
    // sobre el subtotal.
    return (
      this.subtotal *
      this.promocionActiva.porcentaje /
      100
    );
  }


  // Calcula el total final
  // que debe pagar el cliente.
  get total(): number {

    // Resta el descuento al subtotal.
    return (
      this.subtotal -
      this.montoDescuento
    );
  }


  // Este método permite regresar al carrito.
  volverAlCarrito(): void {

    // Navega nuevamente a la página del carrito.
    this.router.navigate(['/carrito']);
  }


  // Este método arma el objeto
  // que la API necesita para confirmar el pedido.
  crearSolicitudPedido(): ConfirmarPedidoRequest {

    // Convierte cada producto del carrito
    // al formato que espera el backend.
    const detalles: DetalleConfirmarPedido[] =
      this.productos.map(
        (producto: ProductoCarrito) => ({

          // Usa el id del carrito como IdProducto
          // porque ese valor representa el producto real.
          idProducto: producto.id,

          // Envía la cantidad seleccionada por el cliente.
          cantidad: producto.cantidad
        })
      );


    // Devuelve el objeto completo
    // que será enviado mediante POST.
    return {

      // trim elimina espacios innecesarios
      // al inicio y al final de la dirección.
      direccionEntrega:
        this.direccionEntrega.trim(),

      // Incluye todos los productos del pedido.
      detalles: detalles
    };
  }


  // Este método confirma realmente la compra
  // enviando el pedido hacia la API.
  continuarCompra(): void {

    // Limpia cualquier mensaje anterior.
    this.mensajeError = '';
    this.mensajeExito = '';


    // Comprueba que existan productos.
    if (this.productos.length === 0) {

      // Muestra el error si el carrito está vacío.
      this.mensajeError =
        'No hay productos en el carrito.';

      // Detiene el proceso.
      return;
    }


    // Comprueba que el cliente haya escrito una dirección.
    if (!this.direccionEntrega.trim()) {

      // Guarda el mensaje que se mostrará en pantalla.
      this.mensajeError =
        'Debe indicar una dirección de entrega.';

      // Detiene el proceso.
      return;
    }


    // Obtiene el token que fue guardado
    // cuando el usuario inició sesión.
    const token =
      localStorage.getItem('token');


    // Comprueba que exista una sesión iniciada.
    if (!token) {

      // Informa que el cliente debe iniciar sesión nuevamente.
      this.mensajeError =
        'La sesión no es válida. Inicie sesión nuevamente.';

      // Evita enviar la petición sin autorización.
      return;
    }


    // Cambia el estado a true
    // para bloquear el botón mientras se procesa la compra.
    this.procesando = true;


    // Crea el objeto con la dirección
    // y los productos que se enviarán a la API.
    const solicitud =
      this.crearSolicitudPedido();


    // Crea los encabezados HTTP.
    // Authorization envía el token JWT al backend
    // para identificar al usuario que está realizando la compra.
    const headers = new HttpHeaders({

      // El formato Bearer es el formato utilizado
      // por la autenticación JWT.
      Authorization: `Bearer ${token}`
    });


    // Realiza una petición POST
    // al endpoint encargado de confirmar el pedido.
    this.http.post(
      `${this.apiUrl}/confirmar`,
      solicitud,
      { headers }
    ).subscribe({

      // Este bloque se ejecuta
      // cuando la API responde correctamente.
      next: (respuesta: any) => {

        // Muestra un mensaje de éxito.
        this.mensajeExito =
          'Pedido confirmado correctamente.';

        // Elimina el carrito del navegador
        // porque la compra ya fue realizada.
        localStorage.removeItem('carrito');

        // Elimina también la promoción activa
        // para que no quede aplicada a una compra futura.
        localStorage.removeItem('promocionActiva');

        // Vacía el arreglo local.
        this.productos = [];

        // Indica que terminó el procesamiento.
        this.procesando = false;


        // Espera un momento para que el cliente
        // pueda ver el mensaje de confirmación.
        setTimeout(() => {

          // Envía al cliente a la página
          // donde podrá consultar sus pedidos.
          this.router.navigate(['/mis-pedidos']);

        }, 1200);
      },


      // Este bloque se ejecuta
      // cuando la API devuelve algún error.
      error: (error: any) => {

        // Muestra en consola el error completo.
        // Esto ayuda durante el desarrollo.
        console.error(
          'Error al confirmar el pedido:',
          error
        );


        // Comprueba si el backend envió
        // un mensaje específico.
        if (error?.error?.mensaje) {

          // Muestra el mensaje enviado por la API.
          this.mensajeError =
            error.error.mensaje;
        }

        // Comprueba si la respuesta de error
        // viene directamente como texto.
        else if (
          typeof error?.error === 'string'
        ) {

          // Muestra el texto recibido.
          this.mensajeError =
            error.error;
        }

        else {

          // Utiliza un mensaje general
          // si la API no envió uno específico.
          this.mensajeError =
            'No se pudo confirmar el pedido.';
        }


        // Vuelve a habilitar el botón
        // para que el cliente pueda intentarlo nuevamente.
        this.procesando = false;
      }
    });
  }
}
