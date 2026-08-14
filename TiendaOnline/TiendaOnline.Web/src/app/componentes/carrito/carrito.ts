import {
  CommonModule
} from '@angular/common';

import {
  Component
} from '@angular/core';

import {
  MatIconModule
} from '@angular/material/icon';

import {
  Router,
  RouterLink
} from '@angular/router';

import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';


interface ProductoCarrito {
  id: number;
  nombre: string;
  marca: string;
  precio: number;
  stock?: number;
  disponibles?: number;
  categoriaId?: number;
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


interface CarritoSql {
  idCarrito: number;
  idUsuario: number;
  fechaCreacion: string;
  estado: string;
}


@Component({
  selector: 'app-carrito',

  imports: [
    CommonModule,
    RouterLink,
    MatIconModule
  ],

  templateUrl:
    './carrito.html',

  styleUrl:
    './carrito.css'
})
export class Carrito {

  productos:
    ProductoCarrito[] = [];

  promocionActiva:
    Promocion | null = null;

  // Guarda el carrito real de SQL.
  idCarritoActual =
    0;


  // Dirección del controlador Carritos.
  apiCarritos =
    'https://localhost:7196/api/Carritos';

  // Dirección del controlador DetalleCarritos.
  apiDetalles =
    'https://localhost:7196/api/DetalleCarritos';


  constructor(
    private router: Router,
    private http: HttpClient
  ) {

    // Carga datos locales.
    this.cargarCarrito();

    this.cargarPromocion();

    // Sincroniza con SQL.
    this.sincronizarCarritoConSql();
  }


  // Obtiene el token.
  private obtenerHeaders():
    HttpHeaders | null {

    const token =
      localStorage.getItem(
        'token'
      );


    if (!token) {
      return null;
    }


    return new HttpHeaders({
      Authorization:
        `Bearer ${token}`
    });
  }


  // Carga carrito local.
  cargarCarrito(): void {

    try {

      this.productos =
        JSON.parse(
          localStorage.getItem(
            'carrito'
          )
          ||
          '[]'
        );

    } catch {

      this.productos =
        [];
    }
  }


  // Sincroniza el carrito
  // actual con SQL Server.
  sincronizarCarritoConSql(
    alTerminar?: () => void
  ): void {

    // Si está vacío no necesita
    // crear un carrito todavía.
    if (
      this.productos.length === 0
    ) {

      if (alTerminar) {
        alTerminar();
      }

      return;
    }


    const headers =
      this.obtenerHeaders();


    if (!headers) {

      if (alTerminar) {
        alTerminar();
      }

      return;
    }


    this.http
      .post<CarritoSql>(
        `${this.apiCarritos}/actual`,
        {},
        {
          headers
        }
      )
      .subscribe({

        next: carrito => {

          // Guarda el ID real.
          this.idCarritoActual =
            carrito.idCarrito;


          localStorage.setItem(
            'idCarritoActual',
            String(
              carrito.idCarrito
            )
          );


          // Guarda todos los
          // productos en SQL.
          this.productos.forEach(
            producto => {

              this.sincronizarProductoSql(
                producto
              );
            }
          );


          if (alTerminar) {
            alTerminar();
          }
        },


        error: error => {

          console.error(
            'Error al sincronizar carrito:',
            error
          );


          if (alTerminar) {
            alTerminar();
          }
        }
      });
  }


  // Guarda un producto
  // dentro de DetalleCarrito.
  private sincronizarProductoSql(
    producto: ProductoCarrito
  ): void {

    if (
      this.idCarritoActual <= 0
    ) {
      return;
    }


    const headers =
      this.obtenerHeaders();


    if (!headers) {
      return;
    }


    const detalle = {

      idCarrito:
        this.idCarritoActual,

      idProducto:
        producto.id,

      cantidad:
        producto.cantidad,

      precioUnitario:
        Number(
          producto.precio
        )
    };


    this.http
      .post(
        this.apiDetalles,
        detalle,
        {
          headers
        }
      )
      .subscribe({

        error: error => {

          console.error(
            'Error guardando detalle del carrito:',
            error
          );
        }
      });
  }


  // Carga promoción.
  cargarPromocion(): void {

    const guardada =
      localStorage.getItem(
        'promocionActiva'
      );


    if (!guardada) {

      this.promocionActiva =
        null;

      return;
    }


    try {

      this.promocionActiva =
        JSON.parse(
          guardada
        );

    } catch {

      this.promocionActiva =
        null;
    }
  }


  // Guarda localmente.
  guardarCarrito(): void {

    localStorage.setItem(
      'carrito',
      JSON.stringify(
        this.productos
      )
    );
  }


  // Obtiene stock.
  obtenerStock(
    producto: ProductoCarrito
  ): number {

    return Number(
      producto.stock
      ??
      producto.disponibles
      ??
      0
    );
  }


  // Aumenta cantidad.
  aumentarCantidad(
    producto: ProductoCarrito
  ): void {

    const stock =
      this.obtenerStock(
        producto
      );


    if (
      producto.cantidad <
      stock
    ) {

      producto.cantidad++;

      this.guardarCarrito();


      if (
        this.idCarritoActual > 0
      ) {

        this.sincronizarProductoSql(
          producto
        );

      } else {

        this.sincronizarCarritoConSql();
      }
    }
  }


  // Disminuye cantidad.
  disminuirCantidad(
    producto: ProductoCarrito
  ): void {

    if (
      producto.cantidad > 1
    ) {

      producto.cantidad--;

      this.guardarCarrito();


      if (
        this.idCarritoActual > 0
      ) {

        this.sincronizarProductoSql(
          producto
        );

      } else {

        this.sincronizarCarritoConSql();
      }
    }
  }


  // Elimina producto.
  eliminarProducto(
    idProducto: number
  ): void {

    this.productos =
      this.productos.filter(
        producto =>
          producto.id !==
          idProducto
      );


    this.guardarCarrito();


    if (
      this.idCarritoActual <= 0
    ) {
      return;
    }


    const headers =
      this.obtenerHeaders();


    if (!headers) {
      return;
    }


    this.http
      .delete(
        `${this.apiDetalles}/carrito/${this.idCarritoActual}/producto/${idProducto}`,
        {
          headers
        }
      )
      .subscribe({

        error: error => {

          console.error(
            'Error eliminando producto de SQL:',
            error
          );
        }
      });
  }


  // Vacía carrito.
  vaciarCarrito(): void {

    this.productos =
      [];

    localStorage.removeItem(
      'carrito'
    );


    if (
      this.idCarritoActual <= 0
    ) {
      return;
    }


    const headers =
      this.obtenerHeaders();


    if (!headers) {
      return;
    }


    this.http
      .delete(
        `${this.apiDetalles}/carrito/${this.idCarritoActual}`,
        {
          headers
        }
      )
      .subscribe({

        error: error => {

          console.error(
            'Error vaciando carrito SQL:',
            error
          );
        }
      });
  }


  // Quita promoción.
  quitarPromocion(): void {

    localStorage.removeItem(
      'promocionActiva'
    );

    this.promocionActiva =
      null;
  }


  // Subtotal producto.
  subtotalProducto(
    producto: ProductoCarrito
  ): number {

    return (
      Number(
        producto.precio
      )
      *
      Number(
        producto.cantidad
      )
    );
  }


  get cantidadTotal():
    number {

    return this.productos
      .reduce(
        (
          total,
          producto
        ) =>
          total +
          Number(
            producto.cantidad
          ),
        0
      );
  }


  get subtotal():
    number {

    return this.productos
      .reduce(
        (
          total,
          producto
        ) =>
          total +
          (
            Number(
              producto.precio
            )
            *
            Number(
              producto.cantidad
            )
          ),
        0
      );
  }


  get promocionCumplida():
    boolean {

    if (
      !this.promocionActiva
    ) {
      return false;
    }


    return (
      this.cantidadTotal >=
      this.promocionActiva
        .cantidadMinima
    );
  }


  get productosFaltantes():
    number {

    if (
      !this.promocionActiva
    ) {
      return 0;
    }


    return Math.max(
      this.promocionActiva
        .cantidadMinima
      -
      this.cantidadTotal,
      0
    );
  }


  get montoDescuento():
    number {

    if (
      !this.promocionActiva
      ||
      !this.promocionCumplida
    ) {
      return 0;
    }


    return (
      this.subtotal
      *
      this.promocionActiva
        .porcentaje
      /
      100
    );
  }


  get total():
    number {

    return (
      this.subtotal -
      this.montoDescuento
    );
  }


  // Sincroniza antes de
  // ir a confirmar.
  confirmarPedido(): void {

    if (
      this.productos.length === 0
    ) {
      return;
    }


    this.sincronizarCarritoConSql(
      () => {

        this.router.navigate([
          '/confirmar-pedido'
        ]);
      }
    );
  }
}
