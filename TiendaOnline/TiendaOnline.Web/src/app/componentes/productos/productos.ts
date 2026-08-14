// Importa CommonModule para usar directivas y pipes de Angular.
import { CommonModule } from '@angular/common';

// Importa Component y ChangeDetectorRef para crear y actualizar la pantalla.
import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

// Importa FormsModule para utilizar ngModel en el buscador.
import { FormsModule } from '@angular/forms';

// Importa HttpClient para consultar los productos de la API.
import { HttpClient } from '@angular/common/http';

// Importa Router para navegar entre pantallas.
import { Router } from '@angular/router';

// Importa botones de PrimeNG.
import { ButtonModule } from 'primeng/button';

// Importa iconos de Angular Material.
import { MatIconModule } from '@angular/material/icon';

// Importa botones de Angular Material.
import { MatButtonModule } from '@angular/material/button';

// Importa ayudas visuales de Angular Material.
import { MatTooltipModule } from '@angular/material/tooltip';

// Define la respuesta enviada por la API.
interface ProductoApi {
  idProducto: number;
  idCategoria: number;
  categoria: string;
  nombre: string;
  descripcion: string | null;
  precio: number;
  imagen: string | null;
  stock: number;
}

// Define la estructura utilizada dentro de Productos.
interface Producto {
  id: number;
  idCategoria: number;
  nombre: string;
  marca: string;
  categoria: string;
  precio: number;
  stock: number;
  imagen: string;
  descripcion: string;
  cantidad: number;
}

// Define la estructura utilizada en el carrito.
interface ProductoCarrito {
  id: number;
  nombre: string;
  marca: string;
  precio: number;
  imagen: string;
  cantidad: number;
  stock: number;
}

// Define la estructura utilizada en favoritos.
interface ProductoFavorito {
  id: number;
  nombre: string;
  marca: string;
  precio: number;
  imagen: string;
  stock: number;
  categoria: string;
}

// Configura el componente Productos.
@Component({
  selector: 'app-productos',

  // Registra los módulos utilizados por el HTML.
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule
  ],

  // Define los archivos visuales del componente.
  templateUrl: './productos.html',
  styleUrl: './productos.css'
})
export class Productos {

  // Guarda la dirección del endpoint de productos.
  private readonly apiUrl =
    'https://localhost:7196/api/Productos';

  // Guarda los productos obtenidos desde SQL Server.
  productos: Producto[] = [];

  // Guarda el texto escrito en el buscador.
  textoBusqueda = '';

  // Guarda la categoría actualmente seleccionada.
  categoriaSeleccionada = 'Todos';

  // Guarda la cantidad de artículos del carrito.
  cantidadCarrito = 0;

  // Guarda la cantidad de productos favoritos.
  cantidadDeseos = 0;

  // Indica si los productos todavía se están cargando.
  cargando = false;

  // Guarda mensajes temporales.
  mensaje = '';

  // Guarda el tipo del mensaje mostrado.
  tipoMensaje: 'exito' | 'info' | 'error' =
    'info';

  // Inyecta los servicios necesarios.
  constructor(
    private http: HttpClient,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    // Carga productos y contadores al abrir la pantalla.
    this.cargarProductos();
    this.actualizarContadores();
  }

  // Obtiene los productos reales desde la API.
  cargarProductos(): void {
    // Activa el mensaje de carga.
    this.cargando = true;

    // Consulta el endpoint público de productos.
    this.http.get<ProductoApi[]>(
      this.apiUrl
    ).subscribe({

      // Se ejecuta cuando la API responde correctamente.
      next: (respuesta) => {

        // Convierte los datos recibidos al formato de la pantalla.
        this.productos =
          respuesta.map(
            producto => ({
              // Utiliza el identificador real de SQL Server.
              id:
                producto.idProducto,

              // Guarda la categoría real.
              idCategoria:
                producto.idCategoria,

              // Utiliza el nombre real del producto.
              nombre:
                producto.nombre,

              // Mantiene Esencia como marca visual de la tienda.
              marca:
                'Esencia',

              // Utiliza el nombre real de la categoría.
              categoria:
                producto.categoria,

              // Utiliza el precio real de SQL Server.
              precio:
                Number(producto.precio),

              // Utiliza el stock real del inventario.
              stock:
                Number(producto.stock),

              // Construye la ruta hacia public/productos.
              imagen:
                producto.imagen
                  ? `/productos/${producto.imagen}`
                  : '',

              // Utiliza la descripción real del producto.
              descripcion:
                producto.descripcion ||
                'Sin descripción disponible.',

              // Cada producto inicia con una unidad seleccionada.
              cantidad:
                1
            })
          );

        // Indica que la carga terminó.
        this.cargando = false;

        // Fuerza a Angular a actualizar la pantalla.
        this.cdr.detectChanges();
      },

      // Se ejecuta cuando ocurre un error.
      error: (error) => {

        // Muestra el error completo durante el desarrollo.
        console.error(
          'Error al cargar los productos:',
          error
        );

        // Limpia la lista si la consulta falla.
        this.productos = [];

        // Finaliza el estado de carga.
        this.cargando = false;

        // Informa el problema al cliente.
        this.mostrarMensaje(
          'No se pudieron cargar los productos.',
          'error'
        );

        // Actualiza inmediatamente la pantalla.
        this.cdr.detectChanges();
      }
    });
  }

  // Obtiene las categorías reales sin repetirlas.
  get categorias(): string[] {

    // Extrae los nombres de las categorías.
    const categorias =
      this.productos.map(
        producto =>
          producto.categoria
      );

    // Agrega Todos y elimina categorías repetidas.
    return [
      'Todos',
      ...Array.from(
        new Set(categorias)
      )
    ];
  }

  // Filtra los productos por búsqueda y categoría.
  get productosFiltrados(): Producto[] {

    // Prepara el texto para realizar la búsqueda.
    const busqueda =
      this.textoBusqueda
        .trim()
        .toLowerCase();

    // Filtra todos los productos cargados.
    return this.productos.filter(
      producto => {

        // Comprueba la categoría seleccionada.
        const coincideCategoria =
          this.categoriaSeleccionada ===
          'Todos' ||
          producto.categoria ===
          this.categoriaSeleccionada;

        // Comprueba nombre, marca, categoría y descripción.
        const coincideBusqueda =
          !busqueda ||
          producto.nombre
            .toLowerCase()
            .includes(busqueda) ||
          producto.marca
            .toLowerCase()
            .includes(busqueda) ||
          producto.categoria
            .toLowerCase()
            .includes(busqueda) ||
          producto.descripcion
            .toLowerCase()
            .includes(busqueda);

        // Devuelve solamente las coincidencias.
        return (
          coincideCategoria &&
          coincideBusqueda
        );
      }
    );
  }

  // Cambia la categoría seleccionada.
  seleccionarCategoria(
    categoria: string
  ): void {

    // Guarda la nueva categoría.
    this.categoriaSeleccionada =
      categoria;
  }

  // Limpia el buscador y la categoría.
  limpiarFiltros(): void {

    // Vuelve a mostrar todas las categorías.
    this.categoriaSeleccionada =
      'Todos';

    // Limpia el texto del buscador.
    this.textoBusqueda =
      '';
  }

  // Aumenta la cantidad sin superar el stock.
  aumentarCantidad(
    producto: Producto
  ): void {

    // Comprueba que existan más unidades disponibles.
    if (
      producto.cantidad <
      producto.stock
    ) {
      // Aumenta una unidad.
      producto.cantidad++;
    }
  }

  // Disminuye la cantidad sin bajar de uno.
  disminuirCantidad(
    producto: Producto
  ): void {

    // Comprueba que exista más de una unidad.
    if (
      producto.cantidad > 1
    ) {
      // Reduce una unidad.
      producto.cantidad--;
    }
  }

  // Agrega un producto al carrito.
  agregarAlCarrito(
    producto: Producto
  ): void {

    // Impide agregar productos sin stock.
    if (producto.stock <= 0) {
      this.mostrarMensaje(
        'Este producto no tiene existencias disponibles.',
        'error'
      );

      return;
    }

    // Obtiene el carrito actual.
    const carrito =
      this.obtenerCarrito();

    // Busca si el producto ya fue agregado.
    const productoExistente =
      carrito.find(
        item =>
          item.id === producto.id
      );

    // Actualiza el producto si ya existe.
    if (productoExistente) {

      // Calcula la nueva cantidad.
      const nuevaCantidad =
        productoExistente.cantidad +
        producto.cantidad;

      // Impide superar el stock real.
      if (
        nuevaCantidad >
        producto.stock
      ) {
        this.mostrarMensaje(
          'No puedes agregar una cantidad mayor al stock disponible.',
          'error'
        );

        return;
      }

      // Actualiza la cantidad.
      productoExistente.cantidad =
        nuevaCantidad;

      // Actualiza el stock almacenado.
      productoExistente.stock =
        producto.stock;
    } else {

      // Crea el producto para guardarlo en el carrito.
      const nuevoProducto:
        ProductoCarrito = {
        id:
          producto.id,
        nombre:
          producto.nombre,
        marca:
          producto.marca,
        precio:
          producto.precio,
        imagen:
          producto.imagen,
        cantidad:
          producto.cantidad,
        stock:
          producto.stock
      };

      // Agrega el nuevo producto al carrito.
      carrito.push(
        nuevoProducto
      );
    }

    // Guarda el carrito actualizado.
    localStorage.setItem(
      'carrito',
      JSON.stringify(carrito)
    );

    // Actualiza el contador del carrito.
    this.actualizarContadores();

    // Regresa la cantidad seleccionada a uno.
    producto.cantidad =
      1;

    // Informa que se agregó correctamente.
    this.mostrarMensaje(
      'Producto agregado al carrito.',
      'exito'
    );
  }

  // Agrega o elimina un producto de favoritos.
  cambiarFavorito(
    producto: Producto
  ): void {

    // Obtiene la lista actual de favoritos.
    const favoritos =
      this.obtenerFavoritos();

    // Busca el producto dentro de favoritos.
    const indice =
      favoritos.findIndex(
        item =>
          item.id === producto.id
      );

    // Elimina el producto si ya estaba guardado.
    if (indice >= 0) {

      // Quita el producto de favoritos.
      favoritos.splice(
        indice,
        1
      );

      // Informa el cambio.
      this.mostrarMensaje(
        'Producto eliminado de favoritos.',
        'info'
      );
    } else {

      // Crea el producto favorito.
      const favorito:
        ProductoFavorito = {
        id:
          producto.id,
        nombre:
          producto.nombre,
        marca:
          producto.marca,
        precio:
          producto.precio,
        imagen:
          producto.imagen,
        stock:
          producto.stock,
        categoria:
          producto.categoria
      };

      // Agrega el producto a favoritos.
      favoritos.push(
        favorito
      );

      // Informa el cambio.
      this.mostrarMensaje(
        'Producto agregado a favoritos.',
        'exito'
      );
    }

    // Guarda los favoritos actualizados.
    localStorage.setItem(
      'listaDeseos',
      JSON.stringify(favoritos)
    );

    // Actualiza el contador.
    this.actualizarContadores();
  }

  // Comprueba si un producto está guardado en favoritos.
  esFavorito(
    producto: Producto
  ): boolean {

    // Busca el producto por identificador.
    return this.obtenerFavoritos()
      .some(
        item =>
          item.id === producto.id
      );
  }

  // Obtiene el carrito guardado en localStorage.
  private obtenerCarrito():
    ProductoCarrito[] {

    // Busca el carrito almacenado.
    const carritoGuardado =
      localStorage.getItem(
        'carrito'
      );

    // Devuelve una lista vacía si no existe.
    if (!carritoGuardado) {
      return [];
    }

    try {
      // Convierte el JSON nuevamente en una lista.
      return JSON.parse(
        carritoGuardado
      );
    } catch {
      // Devuelve una lista vacía si el JSON es inválido.
      return [];
    }
  }

  // Obtiene los favoritos guardados.
  private obtenerFavoritos():
    ProductoFavorito[] {

    // Busca la lista almacenada.
    const favoritosGuardados =
      localStorage.getItem(
        'listaDeseos'
      );

    // Devuelve una lista vacía si no existe.
    if (!favoritosGuardados) {
      return [];
    }

    try {
      // Convierte el JSON nuevamente en una lista.
      return JSON.parse(
        favoritosGuardados
      );
    } catch {
      // Devuelve una lista vacía si el JSON es inválido.
      return [];
    }
  }

  // Actualiza los contadores superiores.
  actualizarContadores(): void {

    // Obtiene el carrito actual.
    const carrito =
      this.obtenerCarrito();

    // Suma todas las unidades del carrito.
    this.cantidadCarrito =
      carrito.reduce(
        (
          total,
          producto
        ) =>
          total +
          Number(producto.cantidad),
        0
      );

    // Cuenta los productos favoritos.
    this.cantidadDeseos =
      this.obtenerFavoritos().length;
  }

  // Muestra un mensaje temporal.
  mostrarMensaje(
    texto: string,
    tipo: 'exito' | 'info' | 'error'
  ): void {

    // Guarda el contenido del mensaje.
    this.mensaje =
      texto;

    // Guarda el tipo del mensaje.
    this.tipoMensaje =
      tipo;

    // Actualiza inmediatamente la pantalla.
    this.cdr.detectChanges();

    // Limpia el mensaje después de unos segundos.
    setTimeout(
      () => {

        // Borra el mensaje.
        this.mensaje =
          '';

        // Actualiza nuevamente la pantalla.
        this.cdr.detectChanges();
      },
      2500
    );
  }

  // Regresa al Dashboard.
  volverDashboard(): void {

    // Navega hacia Dashboard.
    this.router.navigate([
      '/dashboard'
    ]);
  }

  // Abre el Carrito.
  irAlCarrito(): void {

    // Navega hacia Carrito.
    this.router.navigate([
      '/carrito'
    ]);
  }

  // Abre la lista de favoritos.
  irAFavoritos(): void {

    // Navega hacia Lista de deseos.
    this.router.navigate([
      '/lista-deseos'
    ]);
  }

  // Abre Mis Pedidos.
  irAMisPedidos(): void {

    // Navega hacia Mis Pedidos.
    this.router.navigate([
      '/mis-pedidos'
    ]);
  }
}
