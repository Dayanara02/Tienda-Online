// Importa CommonModule para poder usar directivas comunes de Angular
// como *ngIf, *ngFor y pipes dentro del HTML.
import { CommonModule } from '@angular/common';

// Importa Component para poder crear el componente del carrito.
import { Component } from '@angular/core';

// Importa Router para navegar entre páginas.
// Importa RouterLink para poder usar enlaces con routerLink en el HTML.
import { Router, RouterLink } from '@angular/router';


// Esta interfaz define la estructura que debe tener
// cada producto que se guarda dentro del carrito.
interface ProductoCarrito {

  // Identificador único del producto.
  id: number;

  // Nombre del producto que se mostrará al cliente.
  nombre: string;

  // Marca del producto.
  marca: string;

  // Precio unitario del producto.
  precio: number;

  // Cantidad máxima disponible de este producto.
  // Sirve para evitar agregar más unidades de las que hay disponibles.
  disponibles: number;

  // Identificador de la categoría a la que pertenece el producto.
  categoriaId: number;

  // Ruta o nombre de la imagen del producto.
  imagen: string;

  // Cantidad de unidades que el cliente agregó al carrito.
  cantidad: number;
}


// Esta interfaz define la estructura de una promoción.
interface Promocion {

  // Identificador único de la promoción.
  id: number;

  // Nombre que se muestra al cliente.
  nombre: string;

  // Explicación de la promoción.
  descripcion: string;

  // Cantidad mínima de productos necesarios
  // para poder aplicar el descuento.
  cantidadMinima: number;

  // Porcentaje que se descontará del subtotal.
  porcentaje: number;

  // Guarda el icono utilizado para mostrar visualmente la promoción.
  icono: string;
}


// Esta configuración le indica a Angular
// cómo debe funcionar el componente Carrito.
@Component({

  // Nombre interno del componente.
  selector: 'app-carrito',

  // Importa los módulos que necesita este componente.
  imports: [

    // Permite usar funciones comunes de Angular en el HTML.
    CommonModule,

    // Permite navegar usando routerLink desde el HTML.
    RouterLink
  ],

  // Archivo HTML que contiene la estructura visual del carrito.
  templateUrl: './carrito.html',

  // Archivo CSS que contiene los estilos del carrito.
  styleUrl: './carrito.css'
})


// Esta clase contiene toda la lógica de la página Carrito.
export class Carrito {

  // Guarda todos los productos que actualmente
  // se encuentran dentro del carrito.
  productos: ProductoCarrito[] = [];

  // Guarda la promoción seleccionada por el cliente.
  // Si no hay ninguna promoción activa, su valor será null.
  promocionActiva: Promocion | null = null;


  // El constructor se ejecuta automáticamente
  // cuando Angular crea el componente.
  constructor(

    // Router permite navegar desde esta clase
    // hacia otras páginas del sistema.
    private router: Router

  ) {

    // Carga los productos que estaban guardados en localStorage.
    this.cargarCarrito();

    // Carga la promoción que el cliente había seleccionado.
    this.cargarPromocion();
  }


  // Este método obtiene del navegador
  // los productos guardados anteriormente en el carrito.
  cargarCarrito(): void {

    // Busca la clave "carrito" en localStorage.
    // Si no existe, utiliza un arreglo vacío representado como texto.
    // JSON.parse convierte ese texto nuevamente en un arreglo.
    this.productos = JSON.parse(
      localStorage.getItem('carrito') || '[]'
    );
  }


  // Este método carga la promoción activa
  // que fue seleccionada anteriormente por el cliente.
  cargarPromocion(): void {

    // Busca en localStorage la promoción guardada.
    const promocionGuardada =
      localStorage.getItem('promocionActiva');

    // Comprueba que realmente exista una promoción.
    if (promocionGuardada) {

      // Convierte el texto JSON nuevamente
      // en un objeto de tipo promoción.
      this.promocionActiva =
        JSON.parse(promocionGuardada);
    }
  }


  // Este método guarda el estado actual del carrito
  // para que no se pierda al cambiar de página.
  guardarCarrito(): void {

    // Convierte el arreglo de productos en texto JSON
    // y lo almacena usando la clave "carrito".
    localStorage.setItem(
      'carrito',
      JSON.stringify(this.productos)
    );
  }


  // Este método aumenta en una unidad
  // la cantidad de un producto del carrito.
  aumentarCantidad(
    producto: ProductoCarrito
  ): void {

    // Comprueba que todavía haya unidades disponibles.
    // Esto evita que el cliente agregue más productos
    // de los que existen actualmente.
    if (
      producto.cantidad <
      producto.disponibles
    ) {

      // Aumenta la cantidad en una unidad.
      producto.cantidad++;

      // Guarda inmediatamente el nuevo estado del carrito.
      this.guardarCarrito();
    }
  }


  // Este método disminuye la cantidad
  // de un producto del carrito.
  disminuirCantidad(
    producto: ProductoCarrito
  ): void {

    // Solo permite disminuir si existe más de una unidad.
    // Así evita que la cantidad llegue a cero.
    if (producto.cantidad > 1) {

      // Resta una unidad.
      producto.cantidad--;

      // Guarda la nueva cantidad en localStorage.
      this.guardarCarrito();
    }
  }


  // Este método elimina completamente
  // un producto específico del carrito.
  eliminarProducto(
    idProducto: number
  ): void {

    // filter crea un nuevo arreglo
    // conservando todos los productos excepto
    // el que tenga el identificador recibido.
    this.productos =
      this.productos.filter(
        producto =>
          producto.id !== idProducto
      );

    // Guarda el carrito después de eliminar el producto.
    this.guardarCarrito();
  }


  // Este método elimina todos los productos del carrito.
  vaciarCarrito(): void {

    // Deja vacío el arreglo que se muestra en pantalla.
    this.productos = [];

    // Elimina también el carrito guardado en localStorage.
    localStorage.removeItem('carrito');
  }


  // Este método elimina la promoción seleccionada.
  quitarPromocion(): void {

    // Borra la promoción del almacenamiento del navegador.
    localStorage.removeItem(
      'promocionActiva'
    );

    // Deja la variable en null
    // para indicar que ya no existe una promoción activa.
    this.promocionActiva = null;
  }


  // Este método calcula el subtotal
  // de un solo producto del carrito.
  subtotalProducto(
    producto: ProductoCarrito
  ): number {

    // Multiplica el precio unitario
    // por la cantidad seleccionada.
    return (
      producto.precio *
      producto.cantidad
    );
  }


  // Este getter calcula la cantidad total
  // de artículos que hay en el carrito.
  get cantidadTotal(): number {

    // reduce recorre todos los productos
    // y va sumando sus cantidades.
    return this.productos.reduce(
      (

        // Guarda la suma acumulada.
        total: number,

        // Representa el producto actual del recorrido.
        producto: ProductoCarrito
      ) =>

        // Suma la cantidad de este producto al total.
        total +
        Number(producto.cantidad),

      // La suma comienza en cero.
      0
    );
  }


  // Este getter calcula el subtotal general
  // de todos los productos antes de descuentos.
  get subtotal(): number {

    // reduce recorre todo el carrito.
    return this.productos.reduce(
      (

        // Guarda el total acumulado.
        total: number,

        // Representa cada producto del carrito.
        producto: ProductoCarrito
      ) =>

        // Suma al total el precio por la cantidad
        // correspondiente a cada producto.
        total +
        (
          Number(producto.precio) *
          Number(producto.cantidad)
        ),

      // El subtotal comienza en cero.
      0
    );
  }


  // Este getter indica si el cliente
  // cumple con los requisitos de la promoción.
  get promocionCumplida(): boolean {

    // Si no existe ninguna promoción,
    // entonces no puede aplicarse descuento.
    if (!this.promocionActiva) {
      return false;
    }

    // Compara la cantidad total del carrito
    // con la cantidad mínima requerida.
    return (
      this.cantidadTotal >=
      this.promocionActiva.cantidadMinima
    );
  }


  // Este getter calcula cuántos productos
  // faltan para poder activar la promoción.
  get productosFaltantes(): number {

    // Si no existe promoción,
    // no hay productos pendientes para cumplirla.
    if (!this.promocionActiva) {
      return 0;
    }

    // Resta la cantidad actual
    // a la cantidad mínima requerida.
    // Math.max evita que el resultado sea negativo.
    return Math.max(
      this.promocionActiva.cantidadMinima -
      this.cantidadTotal,
      0
    );
  }


  // Este getter calcula cuánto dinero
  // se descontará del subtotal.
  get montoDescuento(): number {

    // Si no hay promoción o no se cumple,
    // el descuento debe ser cero.
    if (
      !this.promocionActiva ||
      !this.promocionCumplida
    ) {
      return 0;
    }

    // Calcula el porcentaje de descuento
    // sobre el subtotal de la compra.
    return (
      this.subtotal *
      this.promocionActiva.porcentaje /
      100
    );
  }


  // Este getter calcula el total final
  // que el cliente debe pagar.
  get total(): number {

    // Resta el descuento aplicado
    // al subtotal original.
    return (
      this.subtotal -
      this.montoDescuento
    );
  }


  // Este método se ejecuta cuando el cliente
  // quiere avanzar a la confirmación del pedido.
  confirmarPedido(): void {

    // Comprueba que existan productos en el carrito.
    if (this.productos.length === 0) {

      // Si está vacío, termina el método
      // y evita avanzar a la siguiente pantalla.
      return;
    }

    // Navega hacia la página donde
    // se confirmará finalmente la compra.
    this.router.navigate([
      '/confirmar-pedido'
    ]);
  }
}
