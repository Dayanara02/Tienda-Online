import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

interface Producto {
  id: number;
  nombre: string;
  marca: string;
  precio: number;
  disponibles: number;
  categoriaId: number;
  imagen: string;
  cantidad: number;
}

@Component({
  selector: 'app-lista-deseos',
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './lista-deseos.html',
  styleUrl: './lista-deseos.css'
})
export class ListaDeseos {

  productosDeseados: Producto[] = [];

  constructor() {
    this.cargarDeseos();
  }

  cargarDeseos(): void {
    this.productosDeseados = JSON.parse(
      localStorage.getItem('listaDeseos') || '[]'
    );
  }

  eliminarDeseo(idProducto: number): void {

    this.productosDeseados =
      this.productosDeseados.filter(
        producto =>
          producto.id !== idProducto
      );

    localStorage.setItem(
      'listaDeseos',
      JSON.stringify(this.productosDeseados)
    );
  }

  agregarAlCarrito(producto: Producto): void {

    const carrito = JSON.parse(
      localStorage.getItem('carrito') || '[]'
    );

    const existente = carrito.find(
      (item: Producto) =>
        item.id === producto.id
    );

    if (existente) {
      existente.cantidad += 1;
    } else {
      carrito.push({
        ...producto,
        cantidad: 1
      });
    }

    localStorage.setItem(
      'carrito',
      JSON.stringify(carrito)
    );
  }
}