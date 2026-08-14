// Importa CommonModule para usar directivas y pipes de Angular.
import { CommonModule } from '@angular/common';

// Importa Component para crear el componente.
import { Component } from '@angular/core';

// Importa FormsModule para utilizar ngModel.
import { FormsModule } from '@angular/forms';

// Importa Router para navegar entre pantallas.
import { Router } from '@angular/router';

// Importa HttpClient y HttpHeaders para comunicarse con la API.
import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';

// Importa botones de PrimeNG.
import { ButtonModule } from 'primeng/button';

// Importa campos de formulario de Angular Material.
import { MatFormFieldModule } from '@angular/material/form-field';

// Importa inputs de Angular Material.
import { MatInputModule } from '@angular/material/input';

// Importa iconos de Angular Material.
import { MatIconModule } from '@angular/material/icon';

// Define la estructura de cada producto del carrito.
interface ProductoCarrito {
  // Guarda el identificador del producto.
  id: number;

  // Guarda el nombre del producto.
  nombre: string;

  // Guarda la marca del producto.
  marca: string;

  // Guarda el precio unitario.
  precio: number;

  // Guarda el stock utilizado por los productos nuevos.
  stock?: number;

  // Mantiene compatibilidad con productos anteriores.
  disponibles?: number;

  // Guarda la categoría cuando está disponible.
  categoriaId?: number;

  // Guarda la imagen del producto.
  imagen: string;

  // Guarda la cantidad seleccionada.
  cantidad: number;
}

// Define la estructura de una promoción.
interface Promocion {
  // Guarda el identificador de la promoción.
  id: number;

  // Guarda el nombre de la promoción.
  nombre: string;

  // Guarda la descripción.
  descripcion: string;

  // Guarda la cantidad mínima requerida.
  cantidadMinima: number;

  // Guarda el porcentaje de descuento.
  porcentaje: number;

  // Guarda el nombre del icono Material.
  icono: string;
}

// Define cada producto que se enviará a la API.
interface DetalleConfirmarPedido {
  // Guarda el identificador del producto.
  idProducto: number;

  // Guarda la cantidad solicitada.
  cantidad: number;
}

// Define el objeto completo que recibirá la API.
interface ConfirmarPedidoRequest {
  // Guarda la dirección de entrega.
  direccionEntrega: string;

  // Guarda el identificador de la promoción seleccionada.
  idPromocion: number | null;

  // Guarda el porcentaje de descuento seleccionado.
  porcentajeDescuento: number;

  // Guarda los productos incluidos en el pedido.
  detalles: DetalleConfirmarPedido[];
}

// Configura el componente Confirmar Pedido.
@Component({
  selector: 'app-confirmar-pedido',

  // Indica que el componente funciona de manera independiente.
  standalone: true,

  // Registra los módulos utilizados en el HTML.
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule
  ],

  // Define los archivos visuales del componente.
  templateUrl: './confirmar-pedido.html',
  styleUrl: './confirmar-pedido.css'
})
export class ConfirmarPedido {

  // Guarda los productos que vienen del carrito.
  productos: ProductoCarrito[] = [];

  // Guarda la promoción seleccionada.
  promocionActiva: Promocion | null = null;

  // Guarda la dirección escrita por el cliente.
  direccionEntrega = '';

  // Indica si el pedido se está procesando.
  procesando = false;

  // Guarda los mensajes de error.
  mensajeError = '';

  // Guarda los mensajes de éxito.
  mensajeExito = '';

  // Guarda la dirección del controlador de pedidos.
  private readonly apiUrl =
    'https://localhost:7196/api/Pedidos';

  // Inyecta HttpClient y Router.
  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    // Carga la información necesaria al abrir la pantalla.
    this.cargarCarrito();
    this.cargarPromocion();
  }

  // Carga los productos guardados en localStorage.
  cargarCarrito(): void {
    // Busca el carrito guardado.
    const carritoGuardado =
      localStorage.getItem('carrito');

    // Termina si no existe información.
    if (!carritoGuardado) {
      this.productos = [];
      return;
    }

    try {
      // Convierte el JSON nuevamente en productos.
      this.productos =
        JSON.parse(carritoGuardado);
    } catch {
      // Deja el carrito vacío si el contenido no es válido.
      this.productos = [];
    }
  }

  // Carga la promoción seleccionada.
  cargarPromocion(): void {
    // Busca la promoción guardada.
    const promocionGuardada =
      localStorage.getItem('promocionActiva');

    // Termina si no existe promoción.
    if (!promocionGuardada) {
      this.promocionActiva = null;
      return;
    }

    try {
      // Convierte el JSON nuevamente en una promoción.
      this.promocionActiva =
        JSON.parse(promocionGuardada);
    } catch {
      // Limpia la promoción si el contenido no es válido.
      this.promocionActiva = null;
    }
  }

  // Obtiene el stock disponible del producto.
  obtenerStock(
    producto: ProductoCarrito
  ): number {
    // Utiliza stock o disponibles según el formato guardado.
    return Number(
      producto.stock ??
      producto.disponibles ??
      0
    );
  }

  // Calcula la cantidad total de artículos.
  get cantidadTotal(): number {
    // Suma las cantidades de todos los productos.
    return this.productos.reduce(
      (
        total,
        producto
      ) =>
        total +
        Number(producto.cantidad),
      0
    );
  }

  // Calcula el subtotal general mostrado en Angular.
  get subtotal(): number {
    // Suma precio por cantidad de cada producto.
    return this.productos.reduce(
      (
        total,
        producto
      ) =>
        total +
        (
          Number(producto.precio) *
          Number(producto.cantidad)
        ),
      0
    );
  }

  // Comprueba si se cumple la promoción.
  get promocionCumplida(): boolean {
    // Devuelve false si no existe promoción.
    if (!this.promocionActiva) {
      return false;
    }

    // Compara la cantidad actual con la mínima.
    return (
      this.cantidadTotal >=
      this.promocionActiva.cantidadMinima
    );
  }

  // Calcula el monto de descuento mostrado en Angular.
  get montoDescuento(): number {
    // No aplica descuento si la promoción no se cumple.
    if (
      !this.promocionActiva ||
      !this.promocionCumplida
    ) {
      return 0;
    }

    // Calcula el porcentaje sobre el subtotal.
    return (
      this.subtotal *
      this.promocionActiva.porcentaje /
      100
    );
  }

  // Calcula el total mostrado antes de confirmar.
  get total(): number {
    // Resta el descuento al subtotal.
    return (
      this.subtotal -
      this.montoDescuento
    );
  }

  // Calcula cuántos productos faltan para la promoción.
  get productosFaltantes(): number {
    // Devuelve cero si no existe promoción.
    if (!this.promocionActiva) {
      return 0;
    }

    // Calcula la diferencia sin permitir negativos.
    return Math.max(
      this.promocionActiva.cantidadMinima -
      this.cantidadTotal,
      0
    );
  }

  // Regresa a la pantalla del carrito.
  volverAlCarrito(): void {
    // Navega hacia Carrito.
    this.router.navigate([
      '/carrito'
    ]);
  }

  // Regresa al Dashboard.
  volverDashboard(): void {
    // Navega hacia el Dashboard del cliente.
    this.router.navigate([
      '/dashboard'
    ]);
  }

  // Crea el objeto que necesita la API.
  crearSolicitudPedido(): ConfirmarPedidoRequest {
    // Convierte los productos al formato del backend.
    const detalles: DetalleConfirmarPedido[] =
      this.productos.map(
        producto => ({
          // Envía el identificador del producto.
          idProducto: producto.id,

          // Envía la cantidad seleccionada.
          cantidad:
            Number(producto.cantidad)
        })
      );

    // Comprueba si la promoción realmente cumple la condición.
    const promocionValida =
      this.promocionActiva &&
      this.promocionCumplida;

    // Devuelve la solicitud completa.
    return {
      // Envía la dirección sin espacios innecesarios.
      direccionEntrega:
        this.direccionEntrega.trim(),

      // Envía el identificador solo si la promoción aplica.
      idPromocion:
        promocionValida
          ? this.promocionActiva!.id
          : null,

      // Envía el porcentaje solo si la promoción aplica.
      porcentajeDescuento:
        promocionValida
          ? this.promocionActiva!.porcentaje
          : 0,

      // Incluye todos los productos seleccionados.
      detalles
    };
  }

  // Confirma el pedido mediante la API.
  continuarCompra(): void {
    // Limpia mensajes anteriores.
    this.mensajeError = '';
    this.mensajeExito = '';

    // Comprueba que existan productos.
    if (this.productos.length === 0) {
      // Informa que el carrito está vacío.
      this.mensajeError =
        'No hay productos en el carrito.';

      return;
    }

    // Comprueba que exista una dirección.
    if (!this.direccionEntrega.trim()) {
      // Informa que la dirección es obligatoria.
      this.mensajeError =
        'Debe indicar una dirección de entrega.';

      return;
    }

    // Comprueba que la dirección sea suficientemente completa.
    if (
      this.direccionEntrega.trim().length <
      10
    ) {
      // Solicita una dirección más detallada.
      this.mensajeError =
        'Escriba una dirección de entrega más completa.';

      return;
    }

    // Obtiene el token del usuario.
    const token =
      localStorage.getItem('token');

    // Comprueba que exista una sesión válida.
    if (!token) {
      // Informa que debe iniciar sesión nuevamente.
      this.mensajeError =
        'La sesión no es válida. Inicie sesión nuevamente.';

      return;
    }

    // Bloquea el botón mientras se procesa.
    this.procesando = true;

    // Crea la solicitud con productos y promoción.
    const solicitud =
      this.crearSolicitudPedido();

    // Crea los encabezados con el JWT.
    const headers =
      new HttpHeaders({
        // Envía el token al backend.
        Authorization:
          `Bearer ${token}`
      });

    // Envía el pedido al endpoint confirmar.
    this.http.post(
      `${this.apiUrl}/confirmar`,
      solicitud,
      { headers }
    ).subscribe({

      // Se ejecuta cuando el pedido fue creado.
      next: () => {
        // Muestra el mensaje de confirmación.
        this.mensajeExito =
          'Pedido confirmado correctamente.';

        // Elimina el carrito ya comprado.
        localStorage.removeItem(
          'carrito'
        );

        // Elimina la promoción utilizada.
        localStorage.removeItem(
          'promocionActiva'
        );

        // Vacía los productos actuales.
        this.productos = [];

        // Finaliza el procesamiento.
        this.procesando = false;

        // Espera un momento antes de cambiar de pantalla.
        setTimeout(
          () => {
            // Navega hacia Mis Pedidos.
            this.router.navigate([
              '/mis-pedidos'
            ]);
          },
          1200
        );
      },

      // Se ejecuta cuando la API devuelve un error.
      error: (error: any) => {
        // Muestra el error completo durante el desarrollo.
        console.error(
          'Error al confirmar el pedido:',
          error
        );

        // Utiliza el mensaje enviado por la API.
        if (error?.error?.mensaje) {
          this.mensajeError =
            error.error.mensaje;
        }

        // Utiliza el texto si la respuesta viene como string.
        else if (
          typeof error?.error === 'string'
        ) {
          this.mensajeError =
            error.error;
        }

        // Utiliza un mensaje general en cualquier otro caso.
        else {
          this.mensajeError =
            'No se pudo confirmar el pedido.';
        }

        // Vuelve a habilitar el botón.
        this.procesando = false;
      }
    });
  }
}
