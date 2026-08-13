// Permite utilizar directivas básicas como *ngIf y *ngFor.
import { CommonModule } from '@angular/common';

// Importa las herramientas principales del componente.
import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

// Permite realizar consultas HTTP al backend.
import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';

// Permite navegar entre páginas.
import { Router } from '@angular/router';

// Permite utilizar iconos de Angular Material.
import { MatIconModule } from '@angular/material/icon';

// Permite utilizar botones de Angular Material.
import { MatButtonModule } from '@angular/material/button';

// Representa un pedido recibido desde la API.
interface PedidoEmpleado {
  idPedido: number;
  fechaPedido: string;
  estado: string;
  total: number;
  direccionEntrega?: string | null;
}

// Representa un envío recibido desde la API.
interface EnvioEmpleado {
  idEnvio: number;
  idPedido: number;
  empresaEnvio?: string | null;
  numeroSeguimiento?: string | null;
  estado: string;
}

// Representa un producto recibido desde la API.
interface ProductoEmpleado {
  idProducto: number;
  nombre: string;
  precio: number;
  stock: number;
}

// Representa cada módulo disponible para el empleado.
interface ModuloEmpleado {
  titulo: string;
  descripcion: string;
  icono: string;
}

@Component({
  selector: 'app-empleado-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule
  ],
  templateUrl: './empleado-dashboard.html',
  styleUrl: './empleado-dashboard.css'
})
export class EmpleadoDashboard implements OnInit {

  // Dirección principal de la API.
  private readonly apiUrl =
    'https://localhost:7196/api';

  // Guarda todos los pedidos recibidos.
  pedidos: PedidoEmpleado[] = [];

  // Guarda todos los envíos recibidos.
  envios: EnvioEmpleado[] = [];

  // Guarda todos los productos recibidos.
  productos: ProductoEmpleado[] = [];

  // Controla el mensaje de carga.
  cargando = true;

  // Guarda mensajes de error.
  mensajeError = '';

  // Módulos disponibles para el empleado.
  modulos: ModuloEmpleado[] = [
    {
      titulo: 'Pedidos',
      descripcion: 'Consultar y gestionar los pedidos de los clientes.',
      icono: 'shopping_bag'
    },
    {
      titulo: 'Envíos',
      descripcion: 'Registrar y actualizar el estado de los envíos.',
      icono: 'local_shipping'
    },
    {
      titulo: 'Productos',
      descripcion: 'Consultar y administrar los productos.',
      icono: 'inventory_2'
    },
    {
      titulo: 'Inventario',
      descripcion: 'Consultar las existencias disponibles.',
      icono: 'warehouse'
    },
    {
      titulo: 'Movimientos de inventario',
      descripcion: 'Consultar entradas y salidas de productos.',
      icono: 'swap_vert'
    },
    {
      titulo: 'Proveedores',
      descripcion: 'Consultar y administrar proveedores.',
      icono: 'business'
    },
    {
      titulo: 'Compras a proveedores',
      descripcion: 'Registrar compras realizadas a proveedores.',
      icono: 'shopping_cart_checkout'
    },
    {
      titulo: 'Proformas',
      descripcion: 'Crear y consultar proformas.',
      icono: 'description'
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

  // Cuenta todos los pedidos recibidos desde la API.
  get totalPedidos(): number {
    return this.pedidos.length;
  }

  // Cuenta todos los productos recibidos desde la API.
  get totalProductos(): number {
    return this.productos.length;
  }

  // Cuenta todos los envíos registrados.
  get totalEnvios(): number {
    return this.envios.length;
  }

  // Cuenta únicamente los pedidos pendientes.
  get pedidosPendientes(): number {
    return this.pedidos.filter(
      pedido =>
        pedido.estado?.toLowerCase() ===
        'pendiente'
    ).length;
  }

  // Cuenta únicamente los pedidos enviados.
  get pedidosEnviados(): number {
    return this.pedidos.filter(
      pedido =>
        pedido.estado?.toLowerCase() ===
        'enviado'
    ).length;
  }

  // Cuenta los envíos que ya fueron entregados.
  // Se usa Envio.Estado porque el seguimiento del cliente
  // también utiliza el estado real del envío.
  get pedidosEntregados(): number {
    return this.envios.filter(
      envio =>
        envio.estado?.toLowerCase() ===
        'entregado'
    ).length;
  }

  // Ordena todos los pedidos del más reciente al más antiguo.
  get pedidosOrdenados(): PedidoEmpleado[] {
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

    // Obtiene los encabezados con el token JWT.
    const headers =
      this.obtenerHeaders();

    // Controla cuándo terminan las tres solicitudes.
    let solicitudesTerminadas = 0;

    // Finaliza la carga cuando terminan las solicitudes.
    const finalizar = () => {
      solicitudesTerminadas++;

      if (solicitudesTerminadas === 3) {
        this.cargando = false;
        this.cd.detectChanges();
      }
    };

    // Consulta todos los pedidos.
    this.http
      .get<PedidoEmpleado[]>(
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

          // Muestra un mensaje según el error recibido.
          if (error.status === 403) {
            this.mensajeError =
              'El usuario no tiene permisos de empleado.';
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

    // Consulta todos los envíos.
    this.http
      .get<EnvioEmpleado[]>(
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

    // Consulta todos los productos.
    this.http
      .get<ProductoEmpleado[]>(
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
  }

  // Construye los encabezados con el token.
  private obtenerHeaders(): HttpHeaders {

    // Obtiene el token guardado al iniciar sesión.
    const token =
      localStorage.getItem('token') ??
      localStorage.getItem('authToken') ??
      '';

    // Devuelve el encabezado Authorization.
    return new HttpHeaders({
      Authorization:
        `Bearer ${token}`
    });
  }

  // Devuelve una clase CSS según el estado.
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

    // Cualquier otro estado se muestra como pendiente.
    return 'estado-pendiente';
  }

  // Convierte la fecha al formato local.
  formatearFecha(fecha: string): string {

    // Evita errores si no existe una fecha.
    if (!fecha) {
      return '-';
    }

    // Devuelve la fecha en formato Costa Rica.
    return new Date(fecha)
      .toLocaleDateString('es-CR');
  }

  // Identifica temporalmente el módulo seleccionado.
  abrirModulo(modulo: ModuloEmpleado): void {

    // Después se conectará cada módulo con su ruta real.
    console.log(
      `Módulo seleccionado: ${modulo.titulo}`
    );
  }

  // Recarga toda la información.
  actualizar(): void {
    this.cargarDashboard();
  }

  // Cierra la sesión del empleado.
  cerrarSesion(): void {

    // Elimina los datos de autenticación.
    localStorage.removeItem('token');
    localStorage.removeItem('authToken');

    // Regresa al inicio de sesión.
    this.router.navigate([
      '/login'
    ]);
  }
}
