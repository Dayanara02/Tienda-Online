import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

interface Categoria {
  id: number;
  nombre: string;
  descripcion: string;
  icono: string;
}

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
  selector: 'app-dashboard',
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard {

  nombreUsuario =
    localStorage.getItem('nombreUsuario') || 'Cliente';

  categoriaSeleccionada = 0;

  cantidadCarrito = 0;
  cantidadDeseos = 0;

  mensaje = '';

  listaDeseos: Producto[] = [];

  categorias: Categoria[] = [
    {
      id: 1,
      nombre: 'Limpieza facial',
      descripcion: 'Geles y espumas',
      icono: '🫧'
    },
    {
      id: 2,
      nombre: 'Hidratación facial',
      descripcion: 'Sérums y cremas',
      icono: '💧'
    },
    {
      id: 3,
      nombre: 'Cuidado corporal',
      descripcion: 'Lociones y exfoliantes',
      icono: '🧴'
    },
    {
      id: 4,
      nombre: 'Protección solar',
      descripcion: 'Rostro y cuerpo',
      icono: '☀️'
    }
  ];

  productos: Producto[] = [
    {
      id: 1,
      nombre: 'Gel limpiador facial',
      marca: 'Cureology',
      precio: 7500,
      disponibles: 12,
      categoriaId: 1,
      imagen:
        'https://images.unsplash.com/photo-1556228578-8c89e6adf883?auto=format&fit=crop&w=600&q=80',
      cantidad: 1
    },
    {
      id: 2,
      nombre: 'Espuma facial suave',
      marca: 'Pure Skin',
      precio: 8900,
      disponibles: 8,
      categoriaId: 1,
      imagen:
        'https://images.unsplash.com/photo-1571781926291-c477ebfd024b?auto=format&fit=crop&w=600&q=80',
      cantidad: 1
    },
    {
      id: 3,
      nombre: 'Agua micelar',
      marca: 'Dermal Care',
      precio: 6200,
      disponibles: 18,
      categoriaId: 1,
      imagen:
        'https://images.unsplash.com/photo-1608248597279-f99d160bfcbc?auto=format&fit=crop&w=600&q=80',
      cantidad: 1
    },
    {
      id: 4,
      nombre: 'Sérum Vitamina C',
      marca: 'Natural Beauty',
      precio: 14500,
      disponibles: 14,
      categoriaId: 2,
      imagen:
        'https://images.unsplash.com/photo-1620916566398-39f1143ab7be?auto=format&fit=crop&w=600&q=80',
      cantidad: 1
    },
    {
      id: 5,
      nombre: 'Crema hidratante facial',
      marca: 'Hydra Skin',
      precio: 11900,
      disponibles: 10,
      categoriaId: 2,
      imagen:
        'https://images.unsplash.com/photo-1601049676869-702ea24cfd58?auto=format&fit=crop&w=600&q=80',
      cantidad: 1
    },
    {
      id: 6,
      nombre: 'Bruma facial de rosas',
      marca: 'Rose Essence',
      precio: 8900,
      disponibles: 11,
      categoriaId: 2,
      imagen:
        'https://images.unsplash.com/photo-1598440947619-2c35fc9aa908?auto=format&fit=crop&w=600&q=80',
      cantidad: 1
    },
    {
      id: 7,
      nombre: 'Loción corporal de avena',
      marca: 'Soft Care',
      precio: 5500,
      disponibles: 20,
      categoriaId: 3,
      imagen:
        'https://images.unsplash.com/photo-1608571423902-eed4a5ad8108?auto=format&fit=crop&w=600&q=80',
      cantidad: 1
    },
    {
      id: 8,
      nombre: 'Exfoliante corporal',
      marca: 'Body Ritual',
      precio: 6800,
      disponibles: 7,
      categoriaId: 3,
      imagen:
        'https://images.unsplash.com/photo-1556229010-6c3f2c9ca5f8?auto=format&fit=crop&w=600&q=80',
      cantidad: 1
    },
    {
      id: 9,
      nombre: 'Aceite corporal nutritivo',
      marca: 'Botanical Care',
      precio: 9400,
      disponibles: 13,
      categoriaId: 3,
      imagen:
        'https://images.unsplash.com/photo-1611930022073-b7a4ba5fcccd?auto=format&fit=crop&w=600&q=80',
      cantidad: 1
    },
    {
      id: 10,
      nombre: 'Protector solar facial FPS 50',
      marca: 'Sun Defense',
      precio: 9800,
      disponibles: 15,
      categoriaId: 4,
      imagen:
        'https://images.unsplash.com/photo-1598440947619-2c35fc9aa908?auto=format&fit=crop&w=600&q=80',
      cantidad: 1
    },
    {
      id: 11,
      nombre: 'Protector solar corporal',
      marca: 'Solar Care',
      precio: 12500,
      disponibles: 9,
      categoriaId: 4,
      imagen:
        'https://images.unsplash.com/photo-1556229010-6c3f2c9ca5f8?auto=format&fit=crop&w=600&q=80',
      cantidad: 1
    },
    {
      id: 12,
      nombre: 'Protector solar en spray',
      marca: 'Sun Fresh',
      precio: 10800,
      disponibles: 16,
      categoriaId: 4,
      imagen:
        'https://images.unsplash.com/photo-1608248597279-f99d160bfcbc?auto=format&fit=crop&w=600&q=80',
      cantidad: 1
    }
  ];

  constructor(
    private router: Router
  ) {
    this.actualizarContadorCarrito();
    this.cargarListaDeseos();
  }

  get productosFiltrados(): Producto[] {

    if (this.categoriaSeleccionada === 0) {
      return this.productos;
    }

    return this.productos.filter(
      producto =>
        producto.categoriaId ===
        this.categoriaSeleccionada
    );
  }

  irASeccion(idSeccion: string): void {

    const elemento =
      document.getElementById(idSeccion);

    if (elemento) {
      elemento.scrollIntoView({
        behavior: 'smooth',
        block: 'start'
      });
    }
  }

  irAMisPedidos(): void {
    this.router.navigate([
      '/mis-pedidos'
    ]);
  }

  irADescuentos(): void {
    this.router.navigate([
      '/descuentos'
    ]);
  }

  seleccionarCategoria(
    idCategoria: number
  ): void {

    this.categoriaSeleccionada =
      idCategoria;

    this.mensaje = '';
  }

  mostrarTodos(): void {

    this.categoriaSeleccionada = 0;

    this.mensaje = '';
  }

  aumentarCantidad(
    producto: Producto
  ): void {

    if (
      producto.cantidad <
      producto.disponibles
    ) {
      producto.cantidad++;
    }
  }

  disminuirCantidad(
    producto: Producto
  ): void {

    if (producto.cantidad > 1) {
      producto.cantidad--;
    }
  }

  agregarAlCarrito(
    producto: Producto
  ): void {

    if (producto.disponibles <= 0) {

      this.mensaje =
        'Este producto se encuentra agotado.';

      return;
    }

    const carritoActual: Producto[] =
      JSON.parse(
        localStorage.getItem('carrito') || '[]'
      );

    const productoExistente =
      carritoActual.find(
        item =>
          item.id === producto.id
      );

    if (productoExistente) {

      const nuevaCantidad =
        productoExistente.cantidad +
        producto.cantidad;

      productoExistente.cantidad =
        Math.min(
          nuevaCantidad,
          producto.disponibles
        );

    } else {

      carritoActual.push({
        ...producto
      });
    }

    localStorage.setItem(
      'carrito',
      JSON.stringify(carritoActual)
    );

    this.actualizarContadorCarrito();

    this.mensaje =
      `${producto.nombre} fue añadido al carrito.`;
  }

  actualizarContadorCarrito(): void {

    const carrito: Producto[] =
      JSON.parse(
        localStorage.getItem('carrito') || '[]'
      );

    this.cantidadCarrito =
      carrito.reduce(
        (
          total: number,
          item: Producto
        ) =>
          total +
          Number(item.cantidad),
        0
      );
  }

  cargarListaDeseos(): void {

    this.listaDeseos =
      JSON.parse(
        localStorage.getItem('listaDeseos') || '[]'
      );

    this.cantidadDeseos =
      this.listaDeseos.length;
  }

  estaEnDeseos(
    idProducto: number
  ): boolean {

    return this.listaDeseos.some(
      producto =>
        producto.id === idProducto
    );
  }

  irADeseos(
    producto: Producto
  ): void {

    if (!this.estaEnDeseos(producto.id)) {

      this.listaDeseos.push({
        ...producto,
        cantidad: 1
      });

      localStorage.setItem(
        'listaDeseos',
        JSON.stringify(this.listaDeseos)
      );

      this.cantidadDeseos =
        this.listaDeseos.length;
    }

    this.router.navigate([
      '/lista-deseos'
    ]);
  }

  cerrarSesion(): void {

    localStorage.removeItem('token');
    localStorage.removeItem('rol');
    localStorage.removeItem('idUsuario');
    localStorage.removeItem('nombreUsuario');
    localStorage.removeItem('correoUsuario');

    this.router.navigate([
      '/login'
    ]);
  }
}