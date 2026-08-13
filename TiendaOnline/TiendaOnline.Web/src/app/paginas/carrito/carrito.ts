// Importa CommonModule para usar directivas y pipes comunes de Angular.
import { CommonModule } from '@angular/common';

// Importa Component para crear el componente Carrito.
import { Component } from '@angular/core';

// Importa los iconos de Angular Material.
import { MatIconModule } from '@angular/material/icon';

// Importa Router para navegar y RouterLink para usar enlaces en el HTML.
import {
  Router,
  RouterLink
  
} from '@angular/router';

// Define la estructura de cada producto guardado en el carrito.
interface ProductoCarrito {
  // Identificador del producto.
  id: number;

  // Nombre del producto.
  nombre: string;

  // Marca del producto.
  marca: string;

  // Precio unitario.
  precio: number;

  // Stock utilizado por los productos nuevos.
  stock?: number;

  // Mantiene compatibilidad con productos antiguos.
  disponibles?: number;

  // Identificador antiguo de la categoría.
  categoriaId?: number;

  // Imagen del producto.
  imagen: string;

  // Cantidad agregada al carrito.
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

  // Cantidad mínima necesaria.
  cantidadMinima: number;

  // Porcentaje de descuento.
  porcentaje: number;

  // Icono de la promoción.
  icono: string;
}

@Component({
  selector: 'app-carrito',

  // Registra los módulos que utiliza este componente.
  imports: [
    CommonModule,
    RouterLink,
    MatIconModule
  ],

  templateUrl: './carrito.html',
  styleUrl: './carrito.css'
})

export class Carrito {

  // Guarda los productos actuales del carrito.
  productos: ProductoCarrito[] = [];

  // Guarda la promoción seleccionada.
  promocionActiva: Promocion | null = null;

  // Inyecta Router para navegar entre páginas.
  constructor(
    private router: Router
  ) {
    // Carga los productos guardados.
    this.cargarCarrito();

    // Carga la promoción guardada.
    this.cargarPromocion();
  }

  // Carga el carrito desde localStorage.
  cargarCarrito(): void {
    try {
      // Obtiene y convierte el carrito guardado.
      this.productos = JSON.parse(
        localStorage.getItem('carrito') || '[]'
      );
    } catch {
      // Usa una lista vacía si el contenido es inválido.
      this.productos = [];
    }
  }

  // Carga la promoción activa desde localStorage.
  cargarPromocion(): void {
    // Obtiene la promoción guardada.
    const promocionGuardada =
      localStorage.getItem('promocionActiva');

    // Termina si no existe promoción.
    if (!promocionGuardada) {
      this.promocionActiva = null;
      return;
    }

    try {
      // Convierte la promoción guardada.
      this.promocionActiva =
        JSON.parse(promocionGuardada);
    } catch {
      // Elimina una promoción inválida.
      this.promocionActiva = null;
    }
  }

  // Guarda el carrito actual en localStorage.
  guardarCarrito(): void {
    localStorage.setItem(
      'carrito',
      JSON.stringify(this.productos)
    );
  }

  // Obtiene el stock disponible sin importar el nombre utilizado.
  obtenerStock(
    producto: ProductoCarrito
  ): number {
    // Usa stock si existe y después disponibles.
    return Number(
      producto.stock ??
      producto.disponibles ??
      0
    );
  }

  // Aumenta una unidad sin superar el stock.
  aumentarCantidad(
    producto: ProductoCarrito
  ): void {
    // Obtiene el stock real del producto.
    const stockDisponible =
      this.obtenerStock(producto);

    // Aumenta solo si todavía existe disponibilidad.
    if (
      producto.cantidad <
      stockDisponible
    ) {
      producto.cantidad++;
      this.guardarCarrito();
    }
  }

  // Disminuye una unidad sin bajar de uno.
  disminuirCantidad(
    producto: ProductoCarrito
  ): void {
    // Evita que la cantidad llegue a cero.
    if (producto.cantidad > 1) {
      producto.cantidad--;
      this.guardarCarrito();
    }
  }

  // Elimina un producto específico.
  eliminarProducto(
    idProducto: number
  ): void {
    // Conserva todos menos el producto seleccionado.
    this.productos =
      this.productos.filter(
        producto =>
          producto.id !== idProducto
      );

    // Guarda el nuevo carrito.
    this.guardarCarrito();
  }

  // Elimina todos los productos del carrito.
  vaciarCarrito(): void {
    // Vacía la lista actual.
    this.productos = [];

    // Elimina el carrito guardado.
    localStorage.removeItem('carrito');
  }

  // Elimina la promoción activa.
  quitarPromocion(): void {
    // Elimina la promoción guardada.
    localStorage.removeItem(
      'promocionActiva'
    );

    // Limpia la variable actual.
    this.promocionActiva = null;
  }

  // Calcula el subtotal de un producto.
  subtotalProducto(
    producto: ProductoCarrito
  ): number {
    // Multiplica precio por cantidad.
    return (
      Number(producto.precio) *
      Number(producto.cantidad)
    );
  }

  // Calcula la cantidad total de artículos.
  get cantidadTotal(): number {
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

  // Calcula el subtotal general.
  get subtotal(): number {
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

  // Indica si la promoción cumple la cantidad mínima.
  get promocionCumplida(): boolean {
    // Devuelve false si no existe promoción.
    if (!this.promocionActiva) {
      return false;
    }

    // Compara la cantidad actual con el requisito.
    return (
      this.cantidadTotal >=
      this.promocionActiva.cantidadMinima
    );
  }

  // Calcula cuántos productos faltan para activar la promoción.
  get productosFaltantes(): number {
    // Devuelve cero si no existe promoción.
    if (!this.promocionActiva) {
      return 0;
    }

    // Calcula lo que falta sin permitir números negativos.
    return Math.max(
      this.promocionActiva.cantidadMinima -
      this.cantidadTotal,
      0
    );
  }

  // Calcula el monto del descuento.
  get montoDescuento(): number {
    // Devuelve cero si no aplica promoción.
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

  // Calcula el total final.
  get total(): number {
    // Resta el descuento al subtotal.
    return (
      this.subtotal -
      this.montoDescuento
    );
  }

  // Navega a la confirmación del pedido.
  confirmarPedido(): void {
    // Evita avanzar con el carrito vacío.
    if (this.productos.length === 0) {
      return;
    }

    // Abre la pantalla Confirmar Pedido.
    this.router.navigate([
      '/confirmar-pedido'
    ]);
  }
}
