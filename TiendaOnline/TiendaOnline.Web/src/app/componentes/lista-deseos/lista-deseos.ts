// Importa CommonModule para utilizar
// directivas como *ngIf y *ngFor.
import {
  CommonModule
} from '@angular/common';


// Importa Component para crear
// el componente Lista de deseos.
import {
  Component
} from '@angular/core';


// Importa Router para navegar
// entre las pantallas de Angular.
import {
  Router
} from '@angular/router';


// Importa HttpClient para comunicarse
// directamente con la API.
//
// HttpHeaders permite enviar el token JWT.
import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';


// Importa los botones de PrimeNG.
import {
  ButtonModule
} from 'primeng/button';


// Importa los iconos de Angular Material.
import {
  MatIconModule
} from '@angular/material/icon';


// Importa los botones de Angular Material.
import {
  MatButtonModule
} from '@angular/material/button';


// Importa los mensajes emergentes
// de Angular Material.
import {
  MatTooltipModule
} from '@angular/material/tooltip';


// =====================================================
// PRODUCTO DE LISTA DE DESEOS
// =====================================================

// Representa un producto favorito.
interface Producto {

  // Identificador real del producto.
  id: number;

  // Nombre del producto.
  nombre: string;

  // Marca del producto.
  marca: string;

  // Precio unitario.
  precio: number;

  // Ruta de la imagen.
  imagen: string;

  // Stock actual.
  stock?: number;

  // Mantiene compatibilidad
  // con productos antiguos.
  disponibles?: number;

  // Nombre de la categoría.
  categoria?: string;

  // Identificador antiguo
  // de la categoría.
  categoriaId?: number;

  // Descripción opcional.
  descripcion?: string;

  // Cantidad opcional.
  cantidad?: number;
}


// =====================================================
// PRODUCTO DEL CARRITO
// =====================================================

// Representa un producto guardado
// dentro del carrito.
interface ProductoCarrito {

  // Identificador.
  id: number;

  // Nombre.
  nombre: string;

  // Marca.
  marca: string;

  // Precio.
  precio: number;

  // Imagen.
  imagen: string;

  // Cantidad agregada.
  cantidad: number;

  // Stock disponible.
  stock: number;
}


// =====================================================
// RESPUESTA DE LISTA DE DESEOS
// =====================================================

// Representa la lista creada
// en SQL Server.
interface ListaDeseosSql {

  // Identificador real de la lista.
  idListaDeseos: number;

  // Usuario dueño de la lista.
  idUsuario: number;

  // Fecha de creación.
  fechaCreacion: string;
}


// =====================================================
// RESPUESTA DEL CARRITO
// =====================================================

// Representa el carrito activo
// creado en SQL Server.
interface CarritoSql {

  // Identificador del carrito.
  idCarrito: number;

  // Usuario dueño del carrito.
  idUsuario: number;

  // Fecha de creación.
  fechaCreacion: string;

  // Activo o Inactivo.
  estado: string;
}


// =====================================================
// COMPONENTE
// =====================================================

@Component({

  // Nombre utilizado por Angular.
  selector:
    'app-lista-deseos',

  // Módulos utilizados por este componente.
  imports: [

    // Directivas comunes.
    CommonModule,

    // Botones PrimeNG.
    ButtonModule,

    // Iconos Material.
    MatIconModule,

    // Botones Material.
    MatButtonModule,

    // Tooltips Material.
    MatTooltipModule
  ],

  // Archivo HTML.
  templateUrl:
    './lista-deseos.html',

  // Archivo CSS.
  styleUrl:
    './lista-deseos.css'
})
export class ListaDeseos {


  // ===================================================
  // VARIABLES PRINCIPALES
  // ===================================================

  // Guarda los productos favoritos
  // que se muestran en pantalla.
  productosDeseados:
    Producto[] = [];


  // Guarda la cantidad total
  // de artículos del carrito.
  cantidadCarrito =
    0;


  // Guarda mensajes temporales.
  mensaje =
    '';


  // Define el tipo de mensaje.
  tipoMensaje:
    'exito' |
    'info' |
    'error' =
    'info';


  // Guarda el identificador real
  // de ListaDeseos en SQL Server.
  idListaDeseosActual =
    0;


  // ===================================================
  // ENDPOINTS DE LA API
  // ===================================================

  // Dirección del controlador ListaDeseos.
  apiListaDeseos =
    'https://localhost:7196/api/ListaDeseos';

  // Dirección del controlador DetalleListaDeseos.
  apiDetalleLista =
    'https://localhost:7196/api/DetalleListaDeseos';

  // Dirección del controlador Carritos.
  apiCarritos =
    'https://localhost:7196/api/Carritos';

  // Dirección del controlador DetalleCarritos.
  apiDetalleCarritos =
    'https://localhost:7196/api/DetalleCarritos';

  // ===================================================
  // CONSTRUCTOR
  // ===================================================

  constructor(

    // Permite navegar entre páginas.
    private router: Router,

    // Permite comunicarse directamente
    // con la API.
    private http: HttpClient

  ) {

    // Carga los favoritos almacenados
    // actualmente en el navegador.
    this.cargarDeseos();

    // Obtiene o crea la lista real
    // del usuario en SQL.
    this.sincronizarListaConSql();

    // Actualiza el contador del carrito.
    this.actualizarCantidadCarrito();
  }


  // ===================================================
  // AUTORIZACIÓN
  // ===================================================

  // Obtiene el token JWT y crea
  // los encabezados para la API.
  private obtenerHeaders():
    HttpHeaders | null {

    // Busca el token almacenado
    // después del inicio de sesión.
    const token =
      localStorage.getItem(
        'token'
      );


    // Si no existe token,
    // no puede realizar solicitudes protegidas.
    if (!token) {

      return null;
    }


    // Devuelve el encabezado Authorization.
    return new HttpHeaders({

      Authorization:
        `Bearer ${token}`
    });
  }


  // ===================================================
  // CARGAR FAVORITOS LOCALES
  // ===================================================

  // Carga los favoritos guardados
  // actualmente en localStorage.
  cargarDeseos(): void {

    // Obtiene los favoritos.
    const deseosGuardados =
      localStorage.getItem(
        'listaDeseos'
      );


    // Si no existe nada guardado...
    if (!deseosGuardados) {

      // Utiliza una lista vacía.
      this.productosDeseados =
        [];

      return;
    }


    try {

      // Convierte el JSON
      // nuevamente en productos.
      this.productosDeseados =
        JSON.parse(
          deseosGuardados
        );

    } catch {

      // Si el JSON está dañado,
      // utiliza una lista vacía.
      this.productosDeseados =
        [];
    }
  }


  // ===================================================
  // OBTENER O CREAR LISTA EN SQL
  // ===================================================

  // Obtiene la ListaDeseos del usuario.
  //
  // Si todavía no existe,
  // el backend la crea automáticamente.
  private obtenerOCrearListaActual(
    alTerminar?:
      (
        idListaDeseos: number
      ) => void
  ): void {

    // Obtiene autorización.
    const headers =
      this.obtenerHeaders();


    // Si no existe sesión...
    if (!headers) {

      this.mostrarMensaje(
        'No existe una sesión activa.',
        'error'
      );

      return;
    }


    // Llama:
    //
    // POST api/ListaDeseos/actual
    this.http
      .post<ListaDeseosSql>(
        `${this.apiListaDeseos}/actual`,
        {},
        {
          headers
        }
      )
      .subscribe({

        // Si funciona...
        next: lista => {

          // Guarda el ID real de SQL.
          this.idListaDeseosActual =
            lista.idListaDeseos;


          // También lo conserva
          // en localStorage.
          localStorage.setItem(
            'idListaDeseosActual',
            String(
              lista.idListaDeseos
            )
          );


          // Ejecuta la acción recibida,
          // si existe.
          if (alTerminar) {

            alTerminar(
              lista.idListaDeseos
            );
          }
        },


        // Si ocurre un error...
        error: error => {

          console.error(
            'Error obteniendo la lista de deseos:',
            error
          );


          this.mostrarMensaje(
            'No se pudo conectar la lista de deseos con la base de datos.',
            'error'
          );
        }
      });
  }


  // ===================================================
  // SINCRONIZAR FAVORITOS CON SQL
  // ===================================================

  // Crea u obtiene la lista
  // y registra los productos favoritos
  // actuales dentro de SQL Server.
  sincronizarListaConSql(): void {

    this.obtenerOCrearListaActual(
      (
        idListaDeseos
      ) => {

        // Recorre todos los favoritos
        // actualmente guardados.
        this.productosDeseados
          .forEach(
            producto => {

              // Guarda cada producto
              // dentro de DetalleListaDeseo.
              this.guardarDeseoSql(
                idListaDeseos,
                producto.id
              );
            }
          );
      }
    );
  }


  // ===================================================
  // GUARDAR FAVORITO EN SQL
  // ===================================================

  // Guarda un producto dentro
  // de DetalleListaDeseo.
  private guardarDeseoSql(
    idListaDeseos: number,
    idProducto: number
  ): void {

    // Obtiene autorización.
    const headers =
      this.obtenerHeaders();


    // Si no existe token...
    if (!headers) {

      return;
    }


    // Crea el objeto que recibirá
    // DetalleListaDeseosController.
    const detalle = {

      // Lista a la que pertenece.
      idListaDeseos:
        idListaDeseos,

      // Producto favorito.
      idProducto:
        idProducto
    };


    // Envía el favorito al backend.
    this.http
      .post(
        this.apiDetalleLista,
        detalle,
        {
          headers
        }
      )
      .subscribe({

        // No necesita hacer nada
        // si funciona.
        next: () => {
        },


        // Muestra posibles errores.
        error: error => {

          console.error(
            'Error guardando favorito en SQL:',
            error
          );
        }
      });
  }


  // ===================================================
  // ELIMINAR FAVORITO
  // ===================================================

  // Elimina un producto tanto
  // del navegador como de SQL.
  eliminarDeseo(
    idProducto: number
  ): void {

    // Primero elimina el producto
    // de la lista visible.
    this.productosDeseados =
      this.productosDeseados
        .filter(
          producto =>
            producto.id !==
            idProducto
        );


    // Actualiza localStorage.
    localStorage.setItem(
      'listaDeseos',
      JSON.stringify(
        this.productosDeseados
      )
    );


    // Si ya conocemos el ID
    // de la lista...
    if (
      this.idListaDeseosActual > 0
    ) {

      // Elimina directamente de SQL.
      this.eliminarDeseoSql(
        this.idListaDeseosActual,
        idProducto
      );

    } else {

      // Si todavía no conocemos la lista,
      // primero la obtiene.
      this.obtenerOCrearListaActual(
        (
          idListaDeseos
        ) => {

          // Después elimina el producto.
          this.eliminarDeseoSql(
            idListaDeseos,
            idProducto
          );
        }
      );
    }


    // Informa al usuario.
    this.mostrarMensaje(
      'Producto eliminado de favoritos.',
      'info'
    );
  }


  // Elimina físicamente el favorito
  // desde DetalleListaDeseo.
  private eliminarDeseoSql(
    idListaDeseos: number,
    idProducto: number
  ): void {

    // Obtiene autorización.
    const headers =
      this.obtenerHeaders();


    if (!headers) {

      return;
    }


    // Llama:
    //
    // DELETE
    // api/DetalleListaDeseos/lista/producto
    this.http
      .delete(
        `${this.apiDetalleLista}/${idListaDeseos}/${idProducto}`,
        {
          headers
        }
      )
      .subscribe({

        // No necesita hacer nada
        // adicional si funciona.
        next: () => {
        },


        // Maneja errores.
        error: error => {

          console.error(
            'Error eliminando favorito de SQL:',
            error
          );
        }
      });
  }


  // ===================================================
  // AGREGAR FAVORITO AL CARRITO
  // ===================================================

  // Agrega un producto favorito
  // al carrito de compras.
  agregarAlCarrito(
    producto: Producto
  ): void {

    // Obtiene el carrito local.
    const carrito =
      this.obtenerCarrito();


    // Busca si el producto
    // ya estaba agregado.
    const existente =
      carrito.find(
        item =>
          item.id ===
          producto.id
      );


    // Obtiene el stock disponible.
    const stockProducto =
      Number(
        producto.stock
        ??
        producto.disponibles
        ??
        1
      );


    // Si ya existe...
    if (existente) {

      // Comprueba que exista stock.
      if (
        existente.cantidad >=
        existente.stock
      ) {

        this.mostrarMensaje(
          'No puedes agregar más unidades de este producto.',
          'error'
        );

        return;
      }


      // Aumenta una unidad.
      existente.cantidad +=
        1;

    } else {

      // Si todavía no existe,
      // crea el producto.
      const nuevoProducto:
        ProductoCarrito = {

        id:
          producto.id,

        nombre:
          producto.nombre,

        marca:
          producto.marca,

        precio:
          Number(
            producto.precio
          ),

        imagen:
          producto.imagen,

        cantidad:
          1,

        stock:
          stockProducto
      };


      // Agrega el producto.
      carrito.push(
        nuevoProducto
      );
    }


    // Guarda el carrito local.
    localStorage.setItem(
      'carrito',
      JSON.stringify(
        carrito
      )
    );


    // Busca el producto ya actualizado.
    const productoCarrito =
      carrito.find(
        item =>
          item.id ===
          producto.id
      );


    // Si lo encontró...
    if (productoCarrito) {

      // También lo registra
      // en SQL Server.
      this.sincronizarProductoCarritoSql(
        productoCarrito
      );
    }


    // Actualiza contador.
    this.actualizarCantidadCarrito();


    // Muestra mensaje.
    this.mostrarMensaje(
      'Producto agregado al carrito.',
      'exito'
    );
  }


  // ===================================================
  // SINCRONIZAR CARRITO CON SQL
  // ===================================================

  // Obtiene o crea el carrito activo
  // y registra el producto.
  private sincronizarProductoCarritoSql(
    producto: ProductoCarrito
  ): void {

    // Obtiene autorización.
    const headers =
      this.obtenerHeaders();


    if (!headers) {

      return;
    }


    // Obtiene o crea el carrito actual.
    this.http
      .post<CarritoSql>(
        `${this.apiCarritos}/actual`,
        {},
        {
          headers
        }
      )
      .subscribe({

        // Si obtiene carrito...
        next: carrito => {

          // Guarda el ID del carrito
          // para otras pantallas.
          localStorage.setItem(
            'idCarritoActual',
            String(
              carrito.idCarrito
            )
          );


          // Prepara el detalle.
          const detalle = {

            idCarrito:
              carrito.idCarrito,

            idProducto:
              producto.id,

            cantidad:
              producto.cantidad,

            precioUnitario:
              Number(
                producto.precio
              )
          };


          // Registra el producto
          // en DetalleCarrito.
          this.http
            .post(
              this.apiDetalleCarritos,
              detalle,
              {
                headers
              }
            )
            .subscribe({

              // No necesita hacer nada
              // si funciona.
              next: () => {
              },


              // Maneja errores.
              error: error => {

                console.error(
                  'Error guardando producto en DetalleCarrito:',
                  error
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
            'El producto se agregó localmente, pero no se pudo guardar en SQL.',
            'error'
          );
        }
      });
  }


  // ===================================================
  // OBTENER CARRITO LOCAL
  // ===================================================

  // Obtiene los productos
  // almacenados en el carrito.
  private obtenerCarrito():
    ProductoCarrito[] {

    // Busca el carrito.
    const carritoGuardado =
      localStorage.getItem(
        'carrito'
      );


    // Si no existe...
    if (!carritoGuardado) {

      return [];
    }


    try {

      // Convierte el JSON.
      return JSON.parse(
        carritoGuardado
      );

    } catch {

      // Si existe un error,
      // devuelve lista vacía.
      return [];
    }
  }


  // ===================================================
  // CONTADOR DEL CARRITO
  // ===================================================

  // Actualiza la cantidad total
  // mostrada en pantalla.
  actualizarCantidadCarrito(): void {

    // Obtiene el carrito.
    const carrito =
      this.obtenerCarrito();


    // Suma las cantidades.
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
  }


  // ===================================================
  // MENSAJES
  // ===================================================

  // Muestra un mensaje temporal.
  mostrarMensaje(
    texto: string,
    tipo:
      'exito' |
      'info' |
      'error'
  ): void {

    // Guarda el texto.
    this.mensaje =
      texto;


    // Guarda el tipo.
    this.tipoMensaje =
      tipo;


    // Lo elimina después
    // de dos segundos y medio.
    setTimeout(
      () => {

        this.mensaje =
          '';

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


  // Abre Productos.
  irAProductos(): void {

    this.router.navigate([
      '/productos'
    ]);
  }


  // Abre Carrito.
  irAlCarrito(): void {

    this.router.navigate([
      '/carrito'
    ]);
  }
}
