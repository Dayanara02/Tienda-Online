// Importa CommonModule para usar directivas y pipes de Angular.
import { CommonModule } from '@angular/common';

// Importa Component y ChangeDetectorRef.
import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

// Importa FormsModule para utilizar ngModel.
import { FormsModule } from '@angular/forms';

// Importa HttpClient para consultar directamente la API.
// HttpHeaders permite enviar el token JWT.
import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';

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


// =====================================================
// PRODUCTO RECIBIDO DESDE LA API
// =====================================================

// Define la respuesta enviada por ProductosController.
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
// PRODUCTO MOSTRADO EN PANTALLA
// =====================================================

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
// RESPUESTA DEL CARRITO DESDE SQL
// =====================================================

interface CarritoSql {

  idCarrito: number;

  idUsuario: number;

  fechaCreacion: string;

  estado: string;
}


// =====================================================
// RESPUESTA DE LISTA DE DESEOS DESDE SQL
// =====================================================

interface ListaDeseosSql {

  idListaDeseos: number;

  idUsuario: number;

  fechaCreacion: string;
}


// =====================================================
// COMPONENTE PRODUCTOS
// =====================================================

@Component({

  selector: 'app-productos',

  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule
  ],

  templateUrl: './productos.html',

  styleUrl: './productos.css'
})
export class Productos {


  // ===================================================
  // DIRECCIONES DE LA API
  // ===================================================

  // Productos.
  apiProductos =
    'https://localhost:7196/api/Productos';

  // Carritos.
  apiCarritos =
    'https://localhost:7196/api/Carritos';

  // Detalles del carrito.
  apiDetalleCarritos =
    'https://localhost:7196/api/DetalleCarritos';

  // Lista de deseos.
  apiListaDeseos =
    'https://localhost:7196/api/ListaDeseos';

  // Detalles de lista de deseos.
  apiDetalleListaDeseos =
    'https://localhost:7196/api/DetalleListaDeseos';


  // ===================================================
  // VARIABLES
  // ===================================================

  // Guarda los productos obtenidos desde SQL Server.
  productos: Producto[] = [];

  // Texto utilizado en el buscador.
  textoBusqueda = '';

  // Categoría seleccionada.
  categoriaSeleccionada = 'Todos';

  // Cantidad total del carrito.
  cantidadCarrito = 0;

  // Cantidad total de favoritos.
  cantidadDeseos = 0;

  // Indica si los productos están cargando.
  cargando = false;

  // Mensaje temporal.
  mensaje = '';

  // Tipo de mensaje.
  tipoMensaje:
    'exito' |
    'info' |
    'error' =
    'info';


  // ===================================================
  // CONSTRUCTOR
  // ===================================================

  constructor(
    private http: HttpClient,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {

    // Carga productos.
    this.cargarProductos();

    // Actualiza los contadores.
    this.actualizarContadores();
  }


  // ===================================================
  // TOKEN JWT
  // ===================================================

  // Obtiene el token del usuario que inició sesión.
  private obtenerHeaders():
    HttpHeaders | null {

    const token =
      localStorage.getItem(
        'token'
      );


    // Si no existe token,
    // no se pueden utilizar los endpoints protegidos.
    if (!token) {

      return null;
    }


    // Envía el JWT en Authorization.
    return new HttpHeaders({

      Authorization:
        `Bearer ${token}`
    });
  }


  // ===================================================
  // CARGAR PRODUCTOS
  // ===================================================

  // Obtiene los productos desde SQL Server.
  cargarProductos(): void {

    // Activa estado de carga.
    this.cargando = true;


    // Consulta ProductosController.
    this.http
      .get<ProductoApi[]>(
        this.apiProductos
      )
      .subscribe({

        // Si funciona correctamente.
        next: respuesta => {

          // Convierte los productos
          // al formato utilizado por Angular.
          this.productos =
            respuesta.map(
              producto => ({

                // ID real del producto.
                id:
                  producto.idProducto,

                // ID real de categoría.
                idCategoria:
                  producto.idCategoria,

                // Nombre.
                nombre:
                  producto.nombre,

                // Marca utilizada visualmente.
                marca:
                  'Esencia',

                // Categoría.
                categoria:
                  producto.categoria,

                // Precio.
                precio:
                  Number(
                    producto.precio
                  ),

                // Stock.
                stock:
                  Number(
                    producto.stock
                  ),

                // Imagen.
                imagen:
                  this.obtenerImagenProducto(
                    producto
                  ),

                // Descripción.
                descripcion:
                  producto.descripcion ||
                  'Sin descripción disponible.',

                // Cantidad inicial.
                cantidad:
                  1
              })
            );


          // Finaliza carga.
          this.cargando = false;

          // Actualiza pantalla.
          this.cdr.detectChanges();
        },


        // Si ocurre un error.
        error: error => {

          console.error(
            'Error al cargar los productos:',
            error
          );


          // Limpia productos.
          this.productos = [];

          // Finaliza carga.
          this.cargando = false;


          // Muestra error.
          this.mostrarMensaje(
            'No se pudieron cargar los productos.',
            'error'
          );


          // Actualiza pantalla.
          this.cdr.detectChanges();
        }
      });
  }


  // ===================================================
  // IMÁGENES
  // ===================================================

  // Obtiene la imagen correspondiente
  // a cada producto.
  private obtenerImagenProducto(
    producto: ProductoApi
  ): string {

    // Si SQL tiene una imagen,
    // utiliza ese archivo.
    if (
      producto.imagen &&
      producto.imagen.trim() !== ''
    ) {

      // Reemplaza barras de Windows.
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


      return (
        `/productosImagenes/${archivo}`
      );
    }


    // Normaliza el nombre.
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


    // KIT SPA
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


    // PERFUME
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


    // SERUM
    if (
      nombre.includes(
        'serum'
      )
    ) {

      return '/productosImagenes/serum-facial.jpg';
    }


    // SHAMPOO
    if (
      nombre.includes(
        'shampoo'
      )
    ) {

      return '/productosImagenes/shampoo-nutritivo.jpg';
    }


    // Si no existe imagen.
    return '';
  }


  // ===================================================
  // CATEGORÍAS
  // ===================================================

  // Obtiene las categorías sin repetirlas.
  get categorias(): string[] {

    const categorias =
      this.productos.map(
        producto =>
          producto.categoria
      );


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
  // FILTROS
  // ===================================================

  // Devuelve los productos filtrados.
  get productosFiltrados():
    Producto[] {

    // Obtiene el texto escrito.
    const busqueda =
      this.textoBusqueda
        .trim()
        .toLowerCase();


    // Filtra productos.
    return this.productos.filter(
      producto => {

        // Comprueba categoría.
        const coincideCategoria =
          this.categoriaSeleccionada ===
          'Todos' ||
          producto.categoria ===
          this.categoriaSeleccionada;


        // Comprueba texto.
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


        return (
          coincideCategoria &&
          coincideBusqueda
        );
      }
    );
  }


  // Cambia categoría.
  seleccionarCategoria(
    categoria: string
  ): void {

    this.categoriaSeleccionada =
      categoria;
  }


  // Limpia filtros.
  limpiarFiltros(): void {

    this.categoriaSeleccionada =
      'Todos';

    this.textoBusqueda =
      '';
  }


  // ===================================================
  // CANTIDAD
  // ===================================================

  // Aumenta cantidad.
  aumentarCantidad(
    producto: Producto
  ): void {

    if (
      producto.cantidad <
      producto.stock
    ) {

      producto.cantidad++;
    }
  }


  // Disminuye cantidad.
  disminuirCantidad(
    producto: Producto
  ): void {

    if (
      producto.cantidad > 1
    ) {

      producto.cantidad--;
    }
  }


  // ===================================================
  // AGREGAR AL CARRITO
  // ===================================================

  // Agrega el producto tanto
  // al carrito local como a SQL Server.
  agregarAlCarrito(
    producto: Producto
  ): void {

    // Comprueba que exista stock.
    if (
      producto.stock <= 0
    ) {

      this.mostrarMensaje(
        'Este producto no tiene existencias disponibles.',
        'error'
      );

      return;
    }


    // Obtiene carrito local.
    const carrito =
      this.obtenerCarrito();


    // Busca si el producto
    // ya está en el carrito.
    const productoExistente =
      carrito.find(
        item =>
          item.id === producto.id
      );


    // Cantidad final que debe quedar.
    let cantidadFinal =
      producto.cantidad;


    // Si ya existe...
    if (
      productoExistente
    ) {

      // Calcula nueva cantidad.
      const nuevaCantidad =
        productoExistente.cantidad +
        producto.cantidad;


      // No permite superar stock.
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


      // Actualiza cantidad local.
      productoExistente.cantidad =
        nuevaCantidad;


      // Actualiza stock.
      productoExistente.stock =
        producto.stock;


      // Guarda cantidad final.
      cantidadFinal =
        nuevaCantidad;

    } else {

      // Crea nuevo producto
      // para el carrito.
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


      // Agrega producto local.
      carrito.push(
        nuevoProducto
      );
    }


    // Guarda carrito en navegador.
    localStorage.setItem(
      'carrito',
      JSON.stringify(
        carrito
      )
    );


    // Actualiza contador visual.
    this.actualizarContadores();


    // =================================================
    // AHORA GUARDA EN SQL
    // =================================================

    this.guardarProductoCarritoSql(
      producto,
      cantidadFinal
    );


    // Reinicia selector.
    producto.cantidad =
      1;
  }


  // ===================================================
  // GUARDAR CARRITO EN SQL
  // ===================================================

  // Obtiene o crea el carrito activo
  // del usuario y después guarda
  // el producto en DetalleCarrito.
  private guardarProductoCarritoSql(
    producto: Producto,
    cantidadFinal: number
  ): void {

    // Obtiene JWT.
    const headers =
      this.obtenerHeaders();


    // Si no existe sesión...
    if (!headers) {

      this.mostrarMensaje(
        'Debes iniciar sesión para agregar productos al carrito.',
        'error'
      );

      return;
    }


    // =================================================
    // PASO 1:
    // OBTENER O CREAR CARRITO ACTIVO
    // =================================================

    this.http
      .post<CarritoSql>(
        `${this.apiCarritos}/actual`,
        {},
        {
          headers
        }
      )
      .subscribe({

        // Si obtiene el carrito...
        next: carrito => {

          // Guarda el ID real del carrito.
          localStorage.setItem(
            'idCarritoActual',
            String(
              carrito.idCarrito
            )
          );


          // Prepara el detalle.
          const detalle = {

            // Carrito real de SQL.
            idCarrito:
              carrito.idCarrito,

            // Producto real.
            idProducto:
              producto.id,

            // Cantidad final.
            cantidad:
              cantidadFinal,

            // Precio actual.
            precioUnitario:
              Number(
                producto.precio
              )
          };


          // ============================================
          // PASO 2:
          // GUARDAR EN DETALLECARRITO
          // ============================================

          this.http
            .post(
              this.apiDetalleCarritos,
              detalle,
              {
                headers
              }
            )
            .subscribe({

              // Todo correcto.
              next: () => {

                this.mostrarMensaje(
                  'Producto agregado al carrito.',
                  'exito'
                );
              },


              // Error guardando detalle.
              error: error => {

                console.error(
                  'Error guardando DetalleCarrito:',
                  error
                );


                this.mostrarMensaje(
                  'El carrito fue creado, pero no se pudo guardar el producto.',
                  'error'
                );
              }
            });
        },


        // Error obteniendo carrito.
        error: error => {

          console.error(
            'Error obteniendo carrito activo:',
            error
          );


          this.mostrarMensaje(
            'No se pudo guardar el carrito en la base de datos.',
            'error'
          );
        }
      });
  }


  // ===================================================
  // FAVORITOS
  // ===================================================

  // Agrega o elimina un favorito
  // tanto localmente como en SQL.
  cambiarFavorito(
    producto: Producto
  ): void {

    // Obtiene favoritos.
    const favoritos =
      this.obtenerFavoritos();


    // Busca producto.
    const indice =
      favoritos.findIndex(
        item =>
          item.id ===
          producto.id
      );


    // =================================================
    // SI YA ERA FAVORITO:
    // ELIMINAR
    // =================================================

    if (
      indice >= 0
    ) {

      // Lo elimina localmente.
      favoritos.splice(
        indice,
        1
      );


      // Guarda favoritos actualizados.
      localStorage.setItem(
        'listaDeseos',
        JSON.stringify(
          favoritos
        )
      );


      // Actualiza contador.
      this.actualizarContadores();


      // También elimina de SQL.
      this.eliminarFavoritoSql(
        producto.id
      );


      return;
    }


    // =================================================
    // SI NO ERA FAVORITO:
    // AGREGAR
    // =================================================

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


    // Agrega localmente.
    favoritos.push(
      favorito
    );


    // Guarda en localStorage.
    localStorage.setItem(
      'listaDeseos',
      JSON.stringify(
        favoritos
      )
    );


    // Actualiza contador.
    this.actualizarContadores();


    // También guarda en SQL.
    this.guardarFavoritoSql(
      producto.id
    );
  }


  // ===================================================
  // GUARDAR FAVORITO EN SQL
  // ===================================================

  // Obtiene o crea ListaDeseos
  // y después registra el producto.
  private guardarFavoritoSql(
    idProducto: number
  ): void {

    // Obtiene JWT.
    const headers =
      this.obtenerHeaders();


    // Comprueba sesión.
    if (!headers) {

      this.mostrarMensaje(
        'Debes iniciar sesión para agregar favoritos.',
        'error'
      );

      return;
    }


    // =================================================
    // PASO 1:
    // OBTENER O CREAR LISTA DE DESEOS
    // =================================================

    this.http
      .post<ListaDeseosSql>(
        `${this.apiListaDeseos}/actual`,
        {},
        {
          headers
        }
      )
      .subscribe({

        // Lista obtenida correctamente.
        next: lista => {

          // Guarda ID real.
          localStorage.setItem(
            'idListaDeseosActual',
            String(
              lista.idListaDeseos
            )
          );


          // Prepara detalle.
          const detalle = {

            // Lista real.
            idListaDeseos:
              lista.idListaDeseos,

            // Producto seleccionado.
            idProducto:
              idProducto
          };


          // ============================================
          // PASO 2:
          // GUARDAR PRODUCTO FAVORITO
          // ============================================

          this.http
            .post(
              this.apiDetalleListaDeseos,
              detalle,
              {
                headers
              }
            )
            .subscribe({

              // Correcto.
              next: () => {

                this.mostrarMensaje(
                  'Producto agregado a favoritos.',
                  'exito'
                );
              },


              // Error.
              error: error => {

                console.error(
                  'Error guardando DetalleListaDeseos:',
                  error
                );


                this.mostrarMensaje(
                  'La lista fue creada, pero no se pudo guardar el producto.',
                  'error'
                );
              }
            });
        },


        // Error obteniendo lista.
        error: error => {

          console.error(
            'Error obteniendo ListaDeseos:',
            error
          );


          this.mostrarMensaje(
            'No se pudo guardar el favorito en la base de datos.',
            'error'
          );
        }
      });
  }


  // ===================================================
  // ELIMINAR FAVORITO DE SQL
  // ===================================================

  // Obtiene la lista actual
  // y elimina el producto favorito.
  private eliminarFavoritoSql(
    idProducto: number
  ): void {

    // Obtiene JWT.
    const headers =
      this.obtenerHeaders();


    if (!headers) {

      return;
    }


    // Obtiene la lista actual.
    this.http
      .post<ListaDeseosSql>(
        `${this.apiListaDeseos}/actual`,
        {},
        {
          headers
        }
      )
      .subscribe({

        // Si encuentra la lista...
        next: lista => {

          // Guarda ID.
          localStorage.setItem(
            'idListaDeseosActual',
            String(
              lista.idListaDeseos
            )
          );


          // Elimina el detalle.
          this.http
            .delete(
              `${this.apiDetalleListaDeseos}/${lista.idListaDeseos}/${idProducto}`,
              {
                headers
              }
            )
            .subscribe({

              // Eliminado correctamente.
              next: () => {

                this.mostrarMensaje(
                  'Producto eliminado de favoritos.',
                  'info'
                );
              },


              // Si ya no estaba en SQL,
              // muestra el error en consola.
              error: error => {

                console.error(
                  'Error eliminando favorito de SQL:',
                  error
                );


                this.mostrarMensaje(
                  'Producto eliminado de favoritos.',
                  'info'
                );
              }
            });
        },


        // Error obteniendo lista.
        error: error => {

          console.error(
            'Error obteniendo lista para eliminar favorito:',
            error
          );
        }
      });
  }


  // ===================================================
  // COMPROBAR FAVORITO
  // ===================================================

  // Comprueba si un producto
  // se encuentra marcado como favorito.
  esFavorito(
    producto: Producto
  ): boolean {

    return this.obtenerFavoritos()
      .some(
        item =>
          item.id ===
          producto.id
      );
  }


  // ===================================================
  // OBTENER CARRITO LOCAL
  // ===================================================

  private obtenerCarrito():
    ProductoCarrito[] {

    // Obtiene carrito guardado.
    const carritoGuardado =
      localStorage.getItem(
        'carrito'
      );


    // Si no existe...
    if (!carritoGuardado) {

      return [];
    }


    try {

      // Convierte JSON.
      return JSON.parse(
        carritoGuardado
      );

    } catch {

      // Si está dañado...
      return [];
    }
  }


  // ===================================================
  // OBTENER FAVORITOS LOCALES
  // ===================================================

  private obtenerFavoritos():
    ProductoFavorito[] {

    // Obtiene favoritos guardados.
    const favoritosGuardados =
      localStorage.getItem(
        'listaDeseos'
      );


    // Si no existen...
    if (!favoritosGuardados) {

      return [];
    }


    try {

      // Convierte JSON.
      return JSON.parse(
        favoritosGuardados
      );

    } catch {

      // Si existe un error...
      return [];
    }
  }


  // ===================================================
  // CONTADORES
  // ===================================================

  actualizarContadores(): void {

    // Obtiene carrito.
    const carrito =
      this.obtenerCarrito();


    // Suma cantidades.
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
  // MENSAJES
  // ===================================================

  mostrarMensaje(
    texto: string,
    tipo:
      'exito' |
      'info' |
      'error'
  ): void {

    // Guarda mensaje.
    this.mensaje =
      texto;


    // Guarda tipo.
    this.tipoMensaje =
      tipo;


    // Actualiza pantalla.
    this.cdr.detectChanges();


    // Oculta el mensaje
    // después de 2.5 segundos.
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

  // Regresa al Dashboard.
  volverDashboard(): void {

    this.router.navigate([
      '/dashboard'
    ]);
  }


  // Abre carrito.
  irAlCarrito(): void {

    this.router.navigate([
      '/carrito'
    ]);
  }


  // Abre favoritos.
  irAFavoritos(): void {

    this.router.navigate([
      '/lista-deseos'
    ]);
  }


  // Abre Mis Pedidos.
  irAMisPedidos(): void {

    this.router.navigate([
      '/mis-pedidos'
    ]);
  }
}
