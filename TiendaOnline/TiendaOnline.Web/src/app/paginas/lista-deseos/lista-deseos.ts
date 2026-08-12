// Importa CommonModule para poder utilizar
// directivas comunes como *ngIf y *ngFor.
import { CommonModule } from '@angular/common';


// Importa Component para crear
// el componente Lista de deseos.
import { Component } from '@angular/core';


// Importa Router para navegar
// hacia otras páginas del cliente.
import { Router } from '@angular/router';


// Importa ButtonModule de PrimeNG
// para utilizar botones PrimeNG.
import { ButtonModule } from 'primeng/button';


// Importa MatIconModule de Angular Material
// para utilizar iconos Material.
import { MatIconModule } from '@angular/material/icon';


// Importa MatButtonModule de Angular Material
// para utilizar botones Material.
import { MatButtonModule } from '@angular/material/button';


// Importa MatTooltipModule de Angular Material
// para mostrar pequeños mensajes de ayuda.
import { MatTooltipModule } from '@angular/material/tooltip';


// Define la estructura de los productos
// que pueden aparecer en la lista de deseos.
interface Producto {

  // Guarda el identificador del producto.
  id: number;

  // Guarda el nombre del producto.
  nombre: string;

  // Guarda la marca del producto.
  marca: string;

  // Guarda el precio unitario.
  precio: number;

  // Guarda la imagen del producto.
  imagen: string;

  // Guarda el stock cuando esté disponible.
  // Es opcional porque algunos favoritos antiguos
  // pueden no tener este dato guardado.
  stock?: number;

  // Mantiene compatibilidad con favoritos antiguos
  // que utilizaban el nombre disponibles.
  disponibles?: number;

  // Guarda la categoría cuando exista.
  categoria?: string;

  // Mantiene compatibilidad con productos antiguos.
  categoriaId?: number;

  // Guarda una descripción cuando exista.
  descripcion?: string;

  // Guarda una cantidad cuando sea necesaria.
  cantidad?: number;
}


// Define la estructura de un producto
// cuando se guarda dentro del carrito.
interface ProductoCarrito {

  // Guarda el identificador.
  id: number;

  // Guarda el nombre.
  nombre: string;

  // Guarda la marca.
  marca: string;

  // Guarda el precio.
  precio: number;

  // Guarda la imagen.
  imagen: string;

  // Guarda la cantidad agregada.
  cantidad: number;

  // Guarda el stock conocido del producto.
  stock: number;
}


// Configura el componente Lista de deseos.
@Component({

  // Define el selector del componente.
  selector: 'app-lista-deseos',

  // Registra los módulos utilizados en el HTML.
  imports: [

    // Permite utilizar funciones comunes de Angular.
    CommonModule,

    // Permite utilizar botones PrimeNG.
    ButtonModule,

    // Permite utilizar iconos Angular Material.
    MatIconModule,

    // Permite utilizar botones Angular Material.
    MatButtonModule,

    // Permite utilizar mensajes emergentes.
    MatTooltipModule
  ],

  // Define el archivo HTML.
  templateUrl: './lista-deseos.html',

  // Define el archivo CSS.
  styleUrl: './lista-deseos.css'
})
export class ListaDeseos {


  // Guarda todos los productos
  // marcados como favoritos.
  productosDeseados: Producto[] = [];


  // Guarda la cantidad total
  // de productos del carrito.
  cantidadCarrito = 0;


  // Guarda un mensaje temporal
  // después de realizar una acción.
  mensaje = '';


  // Define el tipo del mensaje mostrado.
  tipoMensaje: 'exito' | 'info' | 'error' =
    'info';


  // Constructor del componente.
  constructor(

    // Permite navegar entre las páginas.
    private router: Router
  ) {


    // Carga los favoritos guardados.
    this.cargarDeseos();


    // Actualiza la cantidad del carrito.
    this.actualizarCantidadCarrito();
  }


  // Carga la lista de deseos
  // guardada dentro del navegador.
  cargarDeseos(): void {


    // Busca el contenido guardado.
    const deseosGuardados =
      localStorage.getItem(
        'listaDeseos'
      );


    // Si no existe ninguna lista guardada...
    if (!deseosGuardados) {


      // Utiliza un arreglo vacío.
      this.productosDeseados =
        [];


      // Detiene el método.
      return;
    }


    // Intenta convertir el texto JSON
    // nuevamente a un arreglo.
    try {


      // Guarda los productos obtenidos.
      this.productosDeseados =
        JSON.parse(
          deseosGuardados
        );


      // Si el contenido guardado no es válido...
    } catch {


      // Deja la lista vacía.
      this.productosDeseados =
        [];
    }
  }


  // Elimina un producto
  // de la lista de deseos.
  eliminarDeseo(
    idProducto: number
  ): void {


    // Filtra la lista y conserva solamente
    // los productos diferentes al eliminado.
    this.productosDeseados =
      this.productosDeseados.filter(
        producto =>
          producto.id !== idProducto
      );


    // Guarda nuevamente la lista actualizada.
    localStorage.setItem(
      'listaDeseos',
      JSON.stringify(
        this.productosDeseados
      )
    );


    // Muestra un mensaje al cliente.
    this.mostrarMensaje(
      'Producto eliminado de favoritos.',
      'info'
    );
  }


  // Agrega un producto favorito
  // al carrito de compras.
  agregarAlCarrito(
    producto: Producto
  ): void {


    // Obtiene el carrito actual.
    const carrito =
      this.obtenerCarrito();


    // Busca si el producto
    // ya se encuentra en el carrito.
    const existente =
      carrito.find(
        item =>
          item.id === producto.id
      );


    // Obtiene el stock conocido.
    // Primero intenta utilizar stock.
    // Si no existe, intenta utilizar disponibles.
    const stockProducto =
      producto.stock ??
      producto.disponibles ??
      1;


    // Si el producto ya existe en el carrito...
    if (existente) {


      // Comprueba si todavía puede
      // agregarse otra unidad.
      if (
        existente.cantidad >=
        existente.stock
      ) {


        // Muestra un mensaje de error.
        this.mostrarMensaje(
          'No puedes agregar más unidades de este producto.',
          'error'
        );


        // Detiene el método.
        return;
      }


      // Aumenta una unidad.
      existente.cantidad +=
        1;


      // Si todavía no se encuentra
      // dentro del carrito...
    } else {


      // Crea un nuevo producto para el carrito.
      const nuevoProducto: ProductoCarrito = {

        // Guarda el identificador.
        id: producto.id,

        // Guarda el nombre.
        nombre: producto.nombre,

        // Guarda la marca.
        marca: producto.marca,

        // Guarda el precio.
        precio: producto.precio,

        // Guarda la imagen.
        imagen: producto.imagen,

        // Agrega inicialmente una unidad.
        cantidad: 1,

        // Guarda el stock conocido.
        stock: stockProducto
      };


      // Agrega el producto al carrito.
      carrito.push(
        nuevoProducto
      );
    }


    // Guarda nuevamente el carrito.
    localStorage.setItem(
      'carrito',
      JSON.stringify(carrito)
    );


    // Actualiza el contador.
    this.actualizarCantidadCarrito();


    // Muestra un mensaje de éxito.
    this.mostrarMensaje(
      'Producto agregado al carrito.',
      'exito'
    );
  }


  // Obtiene el carrito guardado
  // dentro del navegador.
  private obtenerCarrito():
    ProductoCarrito[] {


    // Busca el carrito.
    const carritoGuardado =
      localStorage.getItem(
        'carrito'
      );


    // Si todavía no existe...
    if (!carritoGuardado) {


      // Devuelve un arreglo vacío.
      return [];
    }


    // Intenta convertir el contenido.
    try {


      // Devuelve el carrito convertido.
      return JSON.parse(
        carritoGuardado
      );


      // Si existe algún problema con el JSON...
    } catch {


      // Devuelve una lista vacía.
      return [];
    }
  }


  // Actualiza el contador
  // de productos del carrito.
  actualizarCantidadCarrito(): void {


    // Obtiene el carrito actual.
    const carrito =
      this.obtenerCarrito();


    // Suma las cantidades
    // de todos los productos.
    this.cantidadCarrito =
      carrito.reduce(
        (
          total,
          producto
        ) =>
          total +
          producto.cantidad,
        0
      );
  }


  // Muestra un mensaje temporal
  // dentro de la pantalla.
  mostrarMensaje(
    texto: string,
    tipo: 'exito' | 'info' | 'error'
  ): void {


    // Guarda el texto recibido.
    this.mensaje =
      texto;


    // Guarda el tipo del mensaje.
    this.tipoMensaje =
      tipo;


    // Espera dos segundos y medio.
    setTimeout(
      () => {


        // Limpia el mensaje.
        this.mensaje =
          '';
      },
      2500
    );
  }


  // Regresa al Dashboard del cliente.
  volverDashboard(): void {


    // Navega hacia el Dashboard.
    this.router.navigate([
      '/dashboard'
    ]);
  }


  // Abre la pantalla Productos.
  irAProductos(): void {


    // Navega hacia Productos.
    this.router.navigate([
      '/productos'
    ]);
  }


  // Abre el carrito.
  irAlCarrito(): void {


    // Navega hacia Carrito.
    this.router.navigate([
      '/carrito'
    ]);
  }
}
