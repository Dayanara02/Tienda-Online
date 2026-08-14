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


// =====================================================
// PRODUCTO RECIBIDO DESDE LA API
// =====================================================

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


// =====================================================
// PRODUCTO UTILIZADO EN EL DASHBOARD
// =====================================================

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


// =====================================================
// PRODUCTO DEL CARRITO
// =====================================================

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


// =====================================================
// PRODUCTO FAVORITO
// =====================================================

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


// =====================================================
// COMPONENTE DASHBOARD
// =====================================================

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


  // ===================================================
  // VARIABLES
  // ===================================================

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
  tipoMensaje:
    'exito' |
    'info' |
    'error' =
    'info';


  // ===================================================
  // CONSTRUCTOR
  // ===================================================

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

    // Actualiza carrito y favoritos.
    this.actualizarContadores();

    // Carga la cantidad de pedidos.
    this.cargarCantidadPedidos();
  }


  // ===================================================
  // CARGAR PRODUCTOS
  // ===================================================

  // Obtiene los productos reales desde la API.
  cargarProductos(): void {

    // Activa el estado de carga.
    this.cargandoProductos =
      true;

    // Consulta el endpoint de productos.
    this.http
      .get<ProductoApi[]>(
        this.urlProductos
      )
      .subscribe({

        // Se ejecuta cuando la API responde correctamente.
        next: respuesta => {

          // Convierte la respuesta
          // al formato utilizado por el Dashboard.
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
                  Number(
                    producto.precio
                  ),

                // Utiliza el stock real del inventario.
                stock:
                  Number(
                    producto.stock
                  ),

                // Obtiene la imagen correcta
                // desde public/productosImagenes.
                imagen:
                  this.obtenerImagenProducto(
                    producto
                  ),

                // Utiliza la descripción real.
                descripcion:
                  producto.descripcion ||
                  'Sin descripción disponible.',

                // Cada producto inicia
                // con una unidad seleccionada.
                cantidad:
                  1
              })
            );

          // Finaliza el estado de carga.
          this.cargandoProductos =
            false;

          // Actualiza inmediatamente la pantalla.
          this.cdr.detectChanges();
        },


        // Se ejecuta cuando ocurre un error.
        error: error => {

          // Muestra el error durante el desarrollo.
          console.error(
            'Error al cargar productos:',
            error
          );

          // Limpia la lista.
          this.productos =
            [];

          // Finaliza la carga.
          this.cargandoProductos =
            false;

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


  // ===================================================
  // IMÁGENES DE LOS PRODUCTOS
  // ===================================================

  // Obtiene la imagen correspondiente
  // a cada producto.
  private obtenerImagenProducto(
    producto: ProductoApi
  ): string {

    // Si SQL ya contiene
    // el nombre de una imagen...
    if (
      producto.imagen &&
      producto.imagen.trim() !== ''
    ) {

      // Cambia las barras de Windows
      // por barras normales.
      const partes =
        producto.imagen
          .replace(/\\/g, '/')
          .split('/');

      // Obtiene únicamente
      // el nombre del archivo.
      const archivo =
        partes[
        partes.length - 1
        ];

      // Busca la imagen dentro
      // de public/productosImagenes.
      return (
        `/productosImagenes/${archivo}`
      );
    }


    // Convierte el nombre
    // para facilitar comparaciones.
    const nombre =
      producto.nombre
        .toLowerCase()
        .normalize('NFD')
        .replace(
          /[\u0300-\u036f]/g,
          ''
        );


    // BODY MIST FLORAL
    if (
      nombre.includes(
        'body mist'
      )
    ) {
      return '/productosImagenes/body-mist-floral.jpg';
    }


    // CREMA CORPORAL
    if (
      nombre.includes(
        'crema corporal'
      )
    ) {
      return '/productosImagenes/crema-corporal.jpg';
    }


    // EXFOLIANTE CORPORAL
    if (
      nombre.includes(
        'exfoliante'
      )
    ) {
      return '/productosImagenes/exfoliante-corporal.jpg';
    }


    // KIT SKINCARE
    if (
      nombre.includes(
        'skincare'
      )
    ) {
      return '/productosImagenes/kit-skincare.jpg';
    }


    // KIT SPA RELAX
    if (
      nombre.includes(
        'spa'
      )
    ) {
      return '/productosImagenes/kit-spa-relax.jpg';
    }


    // LIMPIADOR FACIAL
    if (
      nombre.includes(
        'limpiador'
      )
    ) {
      return '/productosImagenes/limpiador-facial.jpg';
    }


    // MASCARILLA CAPILAR
    if (
      nombre.includes(
        'mascarilla'
      )
    ) {
      return '/productosImagenes/mascarilla-capilar.jpg';
    }


    // PERFUME ELEGANCE
    if (
      nombre.includes(
        'perfume'
      ) ||
      nombre.includes(
        'elegance'
      )
    ) {
      return '/productosImagenes/perfume-elegance.jpg';
    }


    // SERUM FACIAL
    if (
      nombre.includes(
        'serum'
      )
    ) {
      return '/productosImagenes/serum-facial.jpg';
    }


    // SHAMPOO NUTRITIVO
    if (
      nombre.includes(
        'shampoo'
      )
    ) {
      return '/productosImagenes/shampoo-nutritivo.jpg';
    }


    // Si no encuentra imagen,
    // deja la ruta vacía.
    return '';
  }


  // ===================================================
  // CATEGORÍAS
  // ===================================================

  // Obtiene las categorías reales sin repetirlas.
  get categorias(): string[] {

    // Extrae los nombres de las categorías.
    const categorias =
      this.productos.map(
        producto =>
          producto.categoria
      );

    // Devuelve Todos
    // junto con las categorías reales.
    return [
      'Todos',

      ...Array.from(
        new Set(
          categorias
        )
      )
    ];
  }


  // ===================================================
  // FILTRAR PRODUCTOS
  // ===================================================

  // Filtra los productos
  // por búsqueda y categoría.
  get productosFiltrados():
    Producto[] {

    // Prepara el texto escrito.
    const busqueda =
      this.textoBusqueda
        .trim()
        .toLowerCase();

    // Filtra los productos cargados.
    return this.productos.filter(
      producto => {

        // Comprueba categoría.
        const coincideCategoria =
          this.categoriaSeleccionada ===
          'Todos' ||
          producto.categoria ===
          this.categoriaSeleccionada;

        // Comprueba nombre,
        // marca, categoría y descripción.
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

        // Devuelve las coincidencias.
        return (
          coincideCategoria &&
          coincideBusqueda
        );
      }
    );
  }


  // ===================================================
  // PRODUCTOS DESTACADOS
  // ===================================================

  // Devuelve algunos productos
  // para la sección destacada.
  get productosDestacados():
    Producto[] {

    // Utiliza los primeros cuatro productos.
    return this.productos.slice(
      0,
      4
    );
  }


  // ===================================================
  // TOTAL DE PRODUCTOS
  // ===================================================

  // Devuelve la cantidad total.
  get totalProductos(): number {

    return this.productos.length;
  }


  // ===================================================
  // SELECCIONAR CATEGORÍA
  // ===================================================

  seleccionarCategoria(
    categoria: string
  ): void {

    // Guarda la nueva categoría.
    this.categoriaSeleccionada =
      categoria;
  }


  // ===================================================
  // LIMPIAR FILTROS
  // ===================================================

  limpiarFiltros(): void {

    // Restablece la categoría.
    this.categoriaSeleccionada =
      'Todos';

    // Limpia el buscador.
    this.textoBusqueda =
      '';
  }


  // ===================================================
  // AUMENTAR CANTIDAD
  // ===================================================

  aumentarCantidad(
    producto: Producto
  ): void {

    // Comprueba que existan
    // más unidades disponibles.
    if (
      producto.cantidad <
      producto.stock
    ) {
      producto.cantidad++;
    }
  }


  // ===================================================
  // DISMINUIR CANTIDAD
  // ===================================================

  disminuirCantidad(
    producto: Producto
  ): void {

    // No permite bajar de uno.
    if (
      producto.cantidad > 1
    ) {
      producto.cantidad--;
    }
  }


  // ===================================================
  // AGREGAR AL CARRITO
  // ===================================================

  agregarAlCarrito(
    producto: Producto
  ): void {

    // Impide agregar productos
    // sin existencias.
    if (
      producto.stock <= 0
    ) {

      this.mostrarMensaje(
        'Este producto no tiene existencias disponibles.',
        'error'
      );

      return;
    }


    // Obtiene el carrito actual.
    const carrito =
      this.obtenerCarrito();


    // Busca si el producto
    // ya estaba agregado.
    const productoExistente =
      carrito.find(
        item =>
          item.id ===
          producto.id
      );


    // Si ya existe...
    if (
      productoExistente
    ) {

      // Calcula la nueva cantidad.
      const nuevaCantidad =
        productoExistente.cantidad +
        producto.cantidad;


      // Impide superar el stock.
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


      // Actualiza el stock.
      productoExistente.stock =
        producto.stock;

    } else {

      // Crea el producto
      // que se guardará en el carrito.
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


      // Agrega el producto.
      carrito.push(
        nuevoProducto
      );
    }


    // Guarda el carrito actualizado.
    localStorage.setItem(
      'carrito',
      JSON.stringify(
        carrito
      )
    );


    // Actualiza los contadores.
    this.actualizarContadores();


    // Reinicia la cantidad.
    producto.cantidad =
      1;


    // Informa que se agregó.
    this.mostrarMensaje(
      'Producto agregado al carrito.',
      'exito'
    );
  }


  // ===================================================
  // FAVORITOS
  // ===================================================

  cambiarFavorito(
    producto: Producto
  ): void {

    // Obtiene favoritos.
    const favoritos =
      this.obtenerFavoritos();


    // Busca el producto.
    const indice =
      favoritos.findIndex(
        item =>
          item.id ===
          producto.id
      );


    // Si ya existe...
    if (
      indice >= 0
    ) {

      // Lo elimina.
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

      // Crea producto favorito.
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


      // Agrega favorito.
      favoritos.push(
        favorito
      );


      // Informa el cambio.
      this.mostrarMensaje(
        'Producto agregado a favoritos.',
        'exito'
      );
    }


    // Guarda favoritos.
    localStorage.setItem(
      'listaDeseos',
      JSON.stringify(
        favoritos
      )
    );


    // Actualiza contador.
    this.actualizarContadores();
  }


  // ===================================================
  // COMPROBAR FAVORITO
  // ===================================================

  esFavorito(
    producto: Producto
  ): boolean {

    // Busca el identificador.
    return this.obtenerFavoritos()
      .some(
        item =>
          item.id ===
          producto.id
      );
  }


  // ===================================================
  // OBTENER CARRITO
  // ===================================================

  private obtenerCarrito():
    ProductoCarrito[] {

    // Busca el carrito guardado.
    const carritoGuardado =
      localStorage.getItem(
        'carrito'
      );


    // Si no existe...
    if (
      !carritoGuardado
    ) {
      return [];
    }


    try {

      // Convierte JSON a lista.
      return JSON.parse(
        carritoGuardado
      );

    } catch {

      // Si es inválido...
      return [];
    }
  }


  // ===================================================
  // OBTENER FAVORITOS
  // ===================================================

  private obtenerFavoritos():
    ProductoFavorito[] {

    // Busca favoritos guardados.
    const favoritosGuardados =
      localStorage.getItem(
        'listaDeseos'
      );


    // Si no existen...
    if (
      !favoritosGuardados
    ) {
      return [];
    }


    try {

      // Convierte JSON a lista.
      return JSON.parse(
        favoritosGuardados
      );

    } catch {

      // Si es inválido...
      return [];
    }
  }


  // ===================================================
  // ACTUALIZAR CONTADORES
  // ===================================================

  actualizarContadores(): void {

    // Obtiene carrito.
    const carrito =
      this.obtenerCarrito();


    // Suma todas las cantidades.
    this.cantidadCarrito =
      carrito.reduce(
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


    // Cuenta favoritos.
    this.cantidadDeseos =
      this.obtenerFavoritos()
        .length;
  }


  // ===================================================
  // CARGAR CANTIDAD DE PEDIDOS
  // ===================================================

  cargarCantidadPedidos(): void {

    // Obtiene token.
    const token =
      localStorage.getItem(
        'token'
      );


    // Si no existe sesión...
    if (
      !token
    ) {

      this.cantidadPedidos =
        0;

      return;
    }


    // Activa carga.
    this.cargandoPedidos =
      true;


    // Crea encabezados JWT.
    const headers =
      new HttpHeaders({

        Authorization:
          `Bearer ${token}`
      });


    // Consulta pedidos.
    this.http
      .get<any[]>(
        this.urlMisPedidos,
        {
          headers
        }
      )
      .subscribe({

        // Si funciona.
        next: pedidos => {

          // Guarda cantidad.
          this.cantidadPedidos =
            Array.isArray(
              pedidos
            )
              ? pedidos.length
              : 0;


          // Finaliza carga.
          this.cargandoPedidos =
            false;


          // Actualiza pantalla.
          this.cdr.detectChanges();
        },


        // Si falla.
        error: error => {

          console.error(
            'Error al cargar pedidos:',
            error
          );


          // Mantiene Dashboard funcionando.
          this.cantidadPedidos =
            0;


          // Finaliza carga.
          this.cargandoPedidos =
            false;


          // Actualiza pantalla.
          this.cdr.detectChanges();
        }
      });
  }


  // ===================================================
  // MENSAJES
  // ===================================================

  mostrarMensaje(
    texto: string,
    tipo:
      'exito' |
      'info' |
      'error'
  ): void {

    // Guarda contenido.
    this.mensaje =
      texto;


    // Guarda tipo.
    this.tipoMensaje =
      tipo;


    // Actualiza pantalla.
    this.cdr.detectChanges();


    // Limpia después de 2.5 segundos.
    setTimeout(
      () => {

        this.mensaje =
          '';


        this.cdr.detectChanges();
      },

      2500
    );
  }


  // ===================================================
  // NAVEGACIÓN
  // ===================================================

  // Navega hacia Productos.
  irAProductos(): void {

    this.router.navigate([
      '/productos'
    ]);
  }


  // Navega hacia el Carrito.
  irAlCarrito(): void {

    this.router.navigate([
      '/carrito'
    ]);
  }


  // Navega hacia Favoritos.
  irAFavoritos(): void {

    this.router.navigate([
      '/lista-deseos'
    ]);
  }


  // Navega hacia Mis Pedidos.
  irAMisPedidos(): void {

    this.router.navigate([
      '/mis-pedidos'
    ]);
  }


  // Navega hacia Notificaciones.
  irANotificaciones(): void {

    this.router.navigate([
      '/notificaciones'
    ]);
  }


  // Navega hacia Descuentos.
  irADescuentos(): void {

    this.router.navigate([
      '/descuentos'
    ]);
  }


  // Navega hacia Perfil.
  irAPerfil(): void {

    this.router.navigate([
      '/perfil'
    ]);
  }


  // ===================================================
  // CERRAR SESIÓN
  // ===================================================

  cerrarSesion(): void {

    // Elimina token.
    localStorage.removeItem(
      'token'
    );

    // Elimina rol.
    localStorage.removeItem(
      'rol'
    );

    // Elimina usuario.
    localStorage.removeItem(
      'idUsuario'
    );

    // Elimina nombre.
    localStorage.removeItem(
      'nombreUsuario'
    );

    // Elimina correo.
    localStorage.removeItem(
      'correoUsuario'
    );


    // Regresa al Login.
    this.router.navigate([
      '/login'
    ]);
  }
}
