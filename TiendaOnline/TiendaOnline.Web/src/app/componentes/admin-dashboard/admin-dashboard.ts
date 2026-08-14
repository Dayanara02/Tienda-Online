// Permite utilizar directivas básicas como *ngIf y *ngFor.
import { CommonModule } from '@angular/common';

// Importa las herramientas principales para crear el componente.
import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

// Permite realizar consultas al backend.
import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';

// Permite navegar entre las pantallas del sistema.
import { Router } from '@angular/router';

// Permite utilizar los iconos de Angular Material.
import { MatIconModule } from '@angular/material/icon';

// Permite utilizar botones de Angular Material.
import { MatButtonModule } from '@angular/material/button';

// Representa un producto recibido desde la API.
interface ProductoAdmin {
  idProducto: number;
  nombre: string;
  precio: number;
  stock: number;
}

// Representa un pedido recibido desde la API.
interface PedidoAdmin {
  idPedido: number;
  fechaPedido: string;
  estado: string;
  total: number;
  direccionEntrega?: string | null;
}

// Representa un envío recibido desde la API.
interface EnvioAdmin {
  idEnvio: number;
  idPedido: number;
  empresaEnvio?: string | null;
  numeroSeguimiento?: string | null;
  estado: string;
}

// Representa cada módulo mostrado en el Dashboard.
interface ModuloAdmin {
  titulo: string;
  descripcion: string;
  icono: string;
}

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule
  ],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboard implements OnInit {

  // Dirección principal de la API.
  private readonly apiUrl =
    'https://localhost:7196/api';

  // Guarda los productos cargados.
  productos: ProductoAdmin[] = [];

  // Guarda los pedidos cargados.
  pedidos: PedidoAdmin[] = [];

  // Guarda los envíos cargados.
  envios: EnvioAdmin[] = [];

  // Indica si los datos todavía están cargando.
  cargando = true;

  // Guarda un mensaje cuando ocurre un error.
  mensajeError = '';

  // Módulos administrativos del sistema.
  modulos: ModuloAdmin[] = [
    {
      titulo: 'Productos',
      descripcion: 'Administrar el catálogo de productos.',
      icono: 'inventory_2'
    },
    {
      titulo: 'Categorías',
      descripcion: 'Administrar las categorías de productos.',
      icono: 'category'
    },
    {
      titulo: 'Pedidos',
      descripcion: 'Consultar y gestionar pedidos de clientes.',
      icono: 'shopping_bag'
    },
    {
      titulo: 'Envíos',
      descripcion: 'Controlar el envío y entrega de pedidos.',
      icono: 'local_shipping'
    },
    {
      titulo: 'Proveedores',
      descripcion: 'Administrar los proveedores de la tienda.',
      icono: 'business'
    },
    {
      titulo: 'Compras a proveedores',
      descripcion: 'Registrar compras realizadas a proveedores.',
      icono: 'shopping_cart_checkout'
    },
    {
      titulo: 'Proformas',
      descripcion: 'Consultar y administrar proformas.',
      icono: 'description'
    },
    {
      titulo: 'Inventario',
      descripcion: 'Consultar existencias disponibles.',
      icono: 'warehouse'
    },
    {
      titulo: 'Movimientos de inventario',
      descripcion: 'Consultar entradas y salidas del inventario.',
      icono: 'swap_vert'
    },
    {
      titulo: 'Usuarios',
      descripcion: 'Administrar usuarios y accesos.',
      icono: 'group'
    },
    {
      titulo: 'Historial de acceso',
      descripcion: 'Consultar los accesos realizados al sistema.',
      icono: 'history'
    },
    {
      titulo: 'Bitácora',
      descripcion: 'Consultar las actividades realizadas.',
      icono: 'fact_check'
    },
    {
      titulo: 'Impuestos',
      descripcion: 'Administrar los impuestos utilizados.',
      icono: 'percent'
    },
    {
      titulo: 'Descuentos',
      descripcion: 'Administrar promociones y descuentos.',
      icono: 'sell'
    }
  ];

  constructor(
    private http: HttpClient,
    private router: Router,
    private cd: ChangeDetectorRef
  ) { }

  // Se ejecuta cuando se abre el Dashboard.
  ngOnInit(): void {
    this.cargarDashboard();
  }

  // Devuelve la cantidad de productos registrados.
  get totalProductos(): number {
    return this.productos.length;
  }

  // Devuelve la cantidad de pedidos registrados.
  get totalPedidos(): number {
    return this.pedidos.length;
  }

  // Devuelve la cantidad de envíos registrados.
  get totalEnvios(): number {
    return this.envios.length;
  }

  // Cuenta únicamente los envíos que ya fueron entregados.
  get pedidosEntregados(): number {
    return this.envios.filter(
      envio =>
        envio.estado?.toLowerCase() ===
        'entregado'
    ).length;
  }

  // Calcula únicamente el total de los pedidos pagados.
  get totalVentas(): number {
    return this.pedidos
      .filter(
        pedido =>
          pedido.estado?.toLowerCase() ===
          'pagado'
      )
      .reduce(
        (total, pedido) =>
          total + Number(pedido.total || 0),
        0
      );
  }

  // Devuelve todos los pedidos del más reciente al más antiguo.
  get pedidosOrdenados(): PedidoAdmin[] {
    return [...this.pedidos]
      .sort(
        (a, b) =>
          b.idPedido - a.idPedido
      );
  }

  // Carga los datos principales del Dashboard.
  cargarDashboard(): void {
    this.cargando = true;
    this.mensajeError = '';

    // Obtiene los encabezados con el token.
    const headers =
      this.obtenerHeaders();

    // Controla cuando terminan las tres solicitudes.
    let solicitudesTerminadas = 0;

    // Finaliza la pantalla de carga.
    const finalizar = () => {
      solicitudesTerminadas++;

      if (solicitudesTerminadas === 3) {
        this.cargando = false;
        this.cd.detectChanges();
      }
    };

    // Consulta los productos.
    this.http
      .get<ProductoAdmin[]>(
        `${this.apiUrl}/Productos`,
        { headers }
      )
      .subscribe({
        next: respuesta => {
          this.productos =
            respuesta ?? [];

          finalizar();
        },
        error: () => {
          this.productos = [];
          finalizar();
        }
      });

    // Consulta los pedidos.
    this.http
      .get<PedidoAdmin[]>(
        `${this.apiUrl}/Pedidos`,
        { headers }
      )
      .subscribe({
        next: respuesta => {
          this.pedidos =
            respuesta ?? [];

          finalizar();
        },
        error: error => {
          this.pedidos = [];

          // Muestra un mensaje según el tipo de error.
          if (error.status === 403) {
            this.mensajeError =
              'El usuario no tiene permisos de administrador.';
          } else if (error.status === 401) {
            this.mensajeError =
              'La sesión ha vencido.';
          } else {
            this.mensajeError =
              'No fue posible consultar los pedidos.';
          }

          finalizar();
        }
      });

    // Consulta los envíos.
    this.http
      .get<EnvioAdmin[]>(
        `${this.apiUrl}/Envios`,
        { headers }
      )
      .subscribe({
        next: respuesta => {
          this.envios =
            respuesta ?? [];

          finalizar();
        },
        error: () => {
          this.envios = [];
          finalizar();
        }
      });
  }

  // Crea los encabezados utilizados para consultar la API.
  private obtenerHeaders(): HttpHeaders {

    // Obtiene el token guardado al iniciar sesión.
    const token =
      localStorage.getItem('token') ??
      localStorage.getItem('authToken') ??
      '';

    // Agrega el token JWT a la solicitud.
    return new HttpHeaders({
      Authorization:
        `Bearer ${token}`
    });
  }

  // Devuelve el estilo correspondiente al estado.
  claseEstado(estado: string): string {

    // Convierte el estado a minúscula.
    const valor =
      estado?.toLowerCase() ?? '';

    // Estado entregado.
    if (valor === 'entregado') {
      return 'estado-entregado';
    }

    // Estado enviado.
    if (valor === 'enviado') {
      return 'estado-enviado';
    }

    // Estado cancelado.
    if (valor === 'cancelado') {
      return 'estado-cancelado';
    }

    // Estado pagado o confirmado.
    if (
      valor === 'pagado' ||
      valor === 'confirmado'
    ) {
      return 'estado-pagado';
    }

    // Cualquier otro estado se considera pendiente.
    return 'estado-pendiente';
  }

  // Formatea la fecha para mostrarla en Costa Rica.
  formatearFecha(fecha: string): string {

    // Evita errores cuando no hay fecha.
    if (!fecha) {
      return '-';
    }

    // Devuelve la fecha en formato local.
    return new Date(fecha)
      .toLocaleDateString('es-CR');
  }

  // Identifica temporalmente el módulo seleccionado.
  abrirModulo(modulo: ModuloAdmin): void {

    // Luego conectaremos cada módulo con su ruta real.
    console.log(
      `Módulo seleccionado: ${modulo.titulo}`
    );
  }

  // Recarga la información del Dashboard.
  actualizar(): void {
    this.cargarDashboard();
  }

  // Cierra la sesión del administrador.
  cerrarSesion(): void {

    // Elimina la información de autenticación.
    localStorage.removeItem('token');
    localStorage.removeItem('authToken');

    // Regresa a la pantalla de inicio de sesión.
    this.router.navigate([
      '/login'
    ]);
  }
}
