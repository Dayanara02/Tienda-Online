// Importa CommonModule para usar directivas y pipes de Angular.
import { CommonModule } from '@angular/common';

// Importa HttpClient y HttpHeaders para consultar la API.
import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';

// Importa Component y ChangeDetectorRef para crear y actualizar la pantalla.
import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

// Importa FormsModule para utilizar ngModel en el buscador.
import { FormsModule } from '@angular/forms';

// Importa Router y RouterLink para navegar entre páginas.
import {
  Router,
  RouterLink
} from '@angular/router';

// Importa botones de PrimeNG.
import { ButtonModule } from 'primeng/button';

// Importa iconos de Angular Material.
import { MatIconModule } from '@angular/material/icon';

// Importa botones de Angular Material.
import { MatButtonModule } from '@angular/material/button';

// Importa contadores visuales de Angular Material.
import { MatBadgeModule } from '@angular/material/badge';

// Importa ayudas emergentes de Angular Material.
import { MatTooltipModule } from '@angular/material/tooltip';

// Define la respuesta que llega desde la API.
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

// Define la estructura utilizada por el Dashboard.
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

// Configura el componente Dashboard.
@Component({
  selector: 'app-dashboard',

  // Registra los módulos utilizados en el HTML.
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    ButtonModule,
    MatIconModule,
    MatButtonModule,
    MatBadgeModule,
    MatTooltipModule
  ],

  // Define los archivos visuales del componente.
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard {

  // Guarda la dirección del endpoint de productos.
  private readonly urlProductos =
    'https://localhost:7196/api/Productos';

  // Guarda la dirección del endpoint Mis Pedidos.
  private readonly urlMisPedidos =
    'https://localhost:7196/api/Pedidos/mis-pedidos';

  // Guarda el nombre del usuario conectado.
  nombreUsuario = 'Cliente';

  // Guarda todos los productos obtenidos desde SQL Server.
  productos: Producto[] = [];

  // Guarda el texto escrito en el buscador.
  textoBusqueda = '';

  // Guarda la categoría seleccionada.
  categoriaSeleccionada = 'Todos';

  // Guarda la cantidad de artículos del carrito.
  cantidadCarrito = 0;

  // Guarda la cantidad de productos favoritos.
  cantidadDeseos = 0;

  // Guarda la cantidad de pedidos del cliente.
  cantidadPedidos = 0;

  // Indica si los productos todavía se están cargando.
  cargandoProductos = false;

  // Indica si los pedidos todavía se están consultando.
  cargandoPedidos = false;

  // Guarda mensajes temporales.
  mensaje = '';

  // Guarda el tipo del mensaje mostrado.
  tipoMensaje: 'exito' | 'info' | 'error' =
    'info';

  // Inyecta los servicios utilizados por el Dashboard.
  constructor(
    private router: Router,
    private http: HttpClient,
    private cdr: ChangeDetectorRef
  ) {
    // Recupera el nombre guardado durante el Login.
    const nombreGuardado =
      localStorage.getItem(
        'nombreUsuario'
      );

    // Utiliza el nombre guardado si existe.
    if (nombreGuardado) {
      this.nombreUsuario =
        nombreGuardado;
    }

    // Carga la información inicial del Dashboard.
    this.cargarProductos();
    this.actualizarContadores();
    this.cargarCantidadPedidos();
  }

  // Obtiene los productos reales desde la API.
  cargarProductos(): void {
    // Activa el estado de carga.
    this.cargandoProductos = true;

    // Consulta el endpoint público de productos.
    this.http.get<ProductoApi[]>(
      this.urlProductos
    ).subscribe({

      // Se ejecuta cuando la API responde correctamente.
      next: (respuesta) => {

        // Convierte la respuesta al formato utilizado por el Dashboard.
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

              // Mantiene Esencia como marca visual.
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

        // Finaliza el estado de carga.
        this.cargandoProductos = false;

        // Actualiza inmediatamente la pantalla.
        this.cdr.detectChanges();
      },

      // Se ejecuta cuando ocurre un error.
      error: (error) => {

        // Muestra el error durante el desarrollo.
        console.error(
          'Error al cargar productos:',
          error
        );

        // Limpia la lista si la consulta falla.
        this.productos = [];

        // Finaliza la carga.
        this.cargandoProductos = false;

        // Informa el problema al cliente.
        this.mostrarMensaje(
          'No se pudieron cargar los productos.',
          'error'
        );

        // Actualiza la pantalla.
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

    // Devuelve Todos junto con las categorías reales.
    return [
      'Todos',
      ...Array.from(
        new Set(categorias)
      )
    ];
  }

  // Filtra los productos por búsqueda y categoría.
  get productosFiltrados(): Producto[] {
    // Prepara el texto escrito por el cliente.
    const busqueda =
      this.textoBusqueda
        .trim()
        .toLowerCase();

    // Filtra los productos cargados.
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

  // Devuelve algunos productos para la sección destacada.
  get productosDestacados(): Producto[] {
    // Utiliza los primeros cuatro productos reales.
    return this.productos.slice(
      0,
      4
    );
  }

  // Devuelve la cantidad total de productos cargados.
  get totalProductos(): number {
    // Cuenta todos los productos de la API.
    return this.productos.length;
  }

  // Cambia la categoría seleccionada.
  seleccionarCategoria(
    categoria: string
  ): void {
    // Guarda la nueva categoría.
    this.categoriaSeleccionada =
      categoria;
  }

  // Limpia la búsqueda y la categoría.
  limpiarFiltros(): void {
    // Restablece la categoría.
    this.categoriaSeleccionada =
      'Todos';

    // Limpia el buscador.
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
    // Impide agregar productos sin existencias.
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

    // Busca si el producto ya estaba agregado.
    const productoExistente =
      carrito.find(
        item =>
          item.id === producto.id
      );

    // Actualiza la cantidad si el producto ya existe.
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

      // Guarda la nueva cantidad.
      productoExistente.cantidad =
        nuevaCantidad;

      // Actualiza el stock almacenado.
      productoExistente.stock =
        producto.stock;
    } else {

      // Crea el producto que se guardará en el carrito.
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

      // Agrega el producto al carrito.
      carrito.push(
        nuevoProducto
      );
    }

    // Guarda el carrito actualizado.
    localStorage.setItem(
      'carrito',
      JSON.stringify(carrito)
    );

    // Actualiza los contadores.
    this.actualizarContadores();

    // Reinicia la cantidad seleccionada.
    producto.cantidad =
      1;

    // Informa que el producto fue agregado.
    this.mostrarMensaje(
      'Producto agregado al carrito.',
      'exito'
    );
  }

  // Agrega o elimina un producto de favoritos.
  cambiarFavorito(
    producto: Producto
  ): void {
    // Obtiene los favoritos actuales.
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
      // Quita el producto.
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

    // Guarda la lista actualizada.
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
    // Busca el identificador dentro de la lista.
    return this.obtenerFavoritos()
      .some(
        item =>
          item.id === producto.id
      );
  }

  // Obtiene el carrito almacenado en localStorage.
  private obtenerCarrito():
    ProductoCarrito[] {
    // Busca el carrito guardado.
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
      // Devuelve una lista vacía si el contenido es inválido.
      return [];
    }
  }

  // Obtiene los favoritos almacenados en localStorage.
  private obtenerFavoritos():
    ProductoFavorito[] {
    // Busca la lista guardada.
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
      // Devuelve una lista vacía si el contenido es inválido.
      return [];
    }
  }

  // Actualiza los contadores de carrito y favoritos.
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

    // Cuenta los favoritos guardados.
    this.cantidadDeseos =
      this.obtenerFavoritos().length;
  }

  // Consulta cuántos pedidos tiene el cliente.
  cargarCantidadPedidos(): void {
    // Obtiene el token guardado.
    const token =
      localStorage.getItem(
        'token'
      );

    // Termina si no existe una sesión válida.
    if (!token) {
      this.cantidadPedidos =
        0;

      return;
    }

    // Activa el estado de carga.
    this.cargandoPedidos =
      true;

    // Crea los encabezados con el JWT.
    const headers =
      new HttpHeaders({
        Authorization:
          `Bearer ${token}`
      });

    // Consulta los pedidos del cliente.
    this.http.get<any[]>(
      this.urlMisPedidos,
      { headers }
    ).subscribe({

      // Se ejecuta cuando la API responde correctamente.
      next: (pedidos) => {

        // Guarda la cantidad recibida.
        this.cantidadPedidos =
          Array.isArray(pedidos)
            ? pedidos.length
            : 0;

        // Finaliza la carga.
        this.cargandoPedidos =
          false;

        // Actualiza la pantalla.
        this.cdr.detectChanges();
      },

      // Se ejecuta cuando ocurre un error.
      error: (error) => {

        // Muestra el error durante el desarrollo.
        console.error(
          'Error al cargar pedidos:',
          error
        );

        // Mantiene el Dashboard funcionando.
        this.cantidadPedidos =
          0;

        // Finaliza la carga.
        this.cargandoPedidos =
          false;

        // Actualiza la pantalla.
        this.cdr.detectChanges();
      }
    });
  }

  // Muestra un mensaje temporal.
  mostrarMensaje(
    texto: string,
    tipo: 'exito' | 'info' | 'error'
  ): void {
    // Guarda el contenido y el tipo.
    this.mensaje =
      texto;

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

  // Navega hacia Productos.
  irAProductos(): void {
    // Abre el catálogo completo.
    this.router.navigate([
      '/productos'
    ]);
  }

  // Navega hacia el Carrito.
  irAlCarrito(): void {
    // Abre la pantalla Carrito.
    this.router.navigate([
      '/carrito'
    ]);
  }

  // Navega hacia Favoritos.
  irAFavoritos(): void {
    // Abre Lista de deseos.
    this.router.navigate([
      '/lista-deseos'
    ]);
  }

  // Navega hacia Mis Pedidos.
  irAMisPedidos(): void {
    // Abre el historial de pedidos.
    this.router.navigate([
      '/mis-pedidos'
    ]);
  }

  // Navega hacia Descuentos.
  irADescuentos(): void {
    // Abre la pantalla de promociones.
    this.router.navigate([
      '/descuentos'
    ]);
  }

  // Navega hacia el Perfil.
  irAPerfil(): void {
    // Abre los datos del usuario.
    this.router.navigate([
      '/perfil'
    ]);
  }

  // Cierra la sesión actual.
  cerrarSesion(): void {
    // Elimina los datos de autenticación.
    localStorage.removeItem(
      'token'
    );

    localStorage.removeItem(
      'rol'
    );

    localStorage.removeItem(
      'idUsuario'
    );

    localStorage.removeItem(
      'nombreUsuario'
    );

    localStorage.removeItem(
      'correoUsuario'
    );

    // Regresa al Login.
    this.router.navigate([
      '/login'
    ]);
  }
}
