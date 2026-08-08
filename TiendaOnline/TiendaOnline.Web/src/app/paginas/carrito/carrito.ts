import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

interface ProductoCarrito {
  id: number;
  nombre: string;
  marca: string;
  precio: number;
  disponibles: number;
  categoriaId: number;
  imagen: string;
  cantidad: number;
}

interface Promocion {
  id: number;
  nombre: string;
  descripcion: string;
  cantidadMinima: number;
  porcentaje: number;
  icono: string;
}

@Component({
  selector: 'app-carrito',
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './carrito.html',
  styleUrl: './carrito.css'
})
export class Carrito {

  productos: ProductoCarrito[] = [];

  promocionActiva: Promocion | null = null;

  constructor(
    private router: Router
  ) {
    this.cargarCarrito();
    this.cargarPromocion();
  }

  cargarCarrito(): void {
    this.productos = JSON.parse(
      localStorage.getItem('carrito') || '[]'
    );
  }

  cargarPromocion(): void {
    const promocionGuardada =
      localStorage.getItem('promocionActiva');

    if (promocionGuardada) {
      this.promocionActiva =
        JSON.parse(promocionGuardada);
    }
  }

  guardarCarrito(): void {
    localStorage.setItem(
      'carrito',
      JSON.stringify(this.productos)
    );
  }

  aumentarCantidad(
    producto: ProductoCarrito
  ): void {

    if (
      producto.cantidad <
      producto.disponibles
    ) {
      producto.cantidad++;

      this.guardarCarrito();
    }
  }

  disminuirCantidad(
    producto: ProductoCarrito
  ): void {

    if (producto.cantidad > 1) {
      producto.cantidad--;

      this.guardarCarrito();
    }
  }

  eliminarProducto(
    idProducto: number
  ): void {

    this.productos =
      this.productos.filter(
        producto =>
          producto.id !== idProducto
      );

    this.guardarCarrito();
  }

  vaciarCarrito(): void {

    this.productos = [];

    localStorage.removeItem('carrito');
  }

  quitarPromocion(): void {

    localStorage.removeItem(
      'promocionActiva'
    );

    this.promocionActiva = null;
  }

  subtotalProducto(
    producto: ProductoCarrito
  ): number {

    return (
      producto.precio *
      producto.cantidad
    );
  }

  get cantidadTotal(): number {

    return this.productos.reduce(
      (
        total: number,
        producto: ProductoCarrito
      ) =>
        total +
        Number(producto.cantidad),
      0
    );
  }

  get subtotal(): number {

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

  get promocionCumplida(): boolean {

    if (!this.promocionActiva) {
      return false;
    }

    return (
      this.cantidadTotal >=
      this.promocionActiva.cantidadMinima
    );
  }

  get productosFaltantes(): number {

    if (!this.promocionActiva) {
      return 0;
    }

    return Math.max(
      this.promocionActiva.cantidadMinima -
      this.cantidadTotal,
      0
    );
  }

  get montoDescuento(): number {

    if (
      !this.promocionActiva ||
      !this.promocionCumplida
    ) {
      return 0;
    }

    return (
      this.subtotal *
      this.promocionActiva.porcentaje /
      100
    );
  }

  get total(): number {

    return (
      this.subtotal -
      this.montoDescuento
    );
  }

  confirmarPedido(): void {

    if (this.productos.length === 0) {
      return;
    }

    this.router.navigate([
      '/confirmar-pedido'
    ]);
  }
}