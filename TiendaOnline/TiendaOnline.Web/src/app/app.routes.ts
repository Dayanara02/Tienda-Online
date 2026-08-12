// Importa el tipo Routes para definir las rutas de Angular.
import { Routes } from '@angular/router';

// Importa la pantalla de registro.
import { Registro } from './paginas/registro/registro';

// Importa la pantalla de lista de deseos.
import { ListaDeseos } from './paginas/lista-deseos/lista-deseos';

// Importa la pantalla de inicio de sesión.
import { Login } from './paginas/login/login';

// Importa el Dashboard del Cliente.
import { Dashboard } from './paginas/dashboard/dashboard';

// Importa la pantalla de productos.
import { Productos } from './paginas/productos/productos';

// Importa la pantalla del carrito.
import { Carrito } from './paginas/carrito/carrito';

// Importa la pantalla para confirmar el pedido.
import { ConfirmarPedido } from './paginas/confirmar-pedido/confirmar-pedido';

// Importa el historial de pedidos.
import { MisPedidos } from './paginas/mis-pedidos/mis-pedidos';

// Importa el detalle de un pedido.
import { DetallePedido } from './paginas/detalle-pedido/detalle-pedido';

// Importa la pantalla para pagar un pedido.
import { PagoPedido } from './paginas/pago-pedido/pago-pedido';

// Importa la nueva pantalla de seguimiento.
import { SeguimientoPedido } from './paginas/seguimiento-pedido/seguimiento-pedido';

// Importa el perfil del Cliente.
import { Perfil } from './paginas/perfil/perfil';

// Importa la pantalla de descuentos.
import { Descuentos } from './paginas/descuentos/descuentos';

// Importa el Dashboard del Empleado.
import { EmpleadoDashboard } from './paginas/empleado-dashboard/empleado-dashboard';

// Importa la pantalla de inventario.
import { Inventario } from './paginas/inventario/inventario';

// Importa los movimientos de inventario.
import { MovimientosInventario } from './paginas/movimientos-inventario/movimientos-inventario';

// Importa la gestión de pedidos.
import { GestionPedidos } from './paginas/gestion-pedidos/gestion-pedidos';

// Importa el Dashboard del Administrador.
import { AdminDashboard } from './paginas/admin-dashboard/admin-dashboard';

// Importa la gestión de productos.
import { GestionProductos } from './paginas/gestion-productos/gestion-productos';

// Importa la gestión de categorías.
import { GestionCategorias } from './paginas/gestion-categorias/gestion-categorias';

// Importa la gestión de usuarios.
import { GestionUsuarios } from './paginas/gestion-usuarios/gestion-usuarios';

// Contiene todas las rutas disponibles de la aplicación.
export const routes: Routes = [

  // Ruta para iniciar sesión.
  {
    path: 'login',
    component: Login
  },

  // Ruta para registrar un usuario.
  {
    path: 'registro',
    component: Registro
  },

  // Ruta para consultar favoritos.
  {
    path: 'lista-deseos',
    component: ListaDeseos
  },

  // Ruta para consultar descuentos.
  {
    path: 'descuentos',
    component: Descuentos
  },

  // Dashboard principal del Cliente.
  {
    path: 'dashboard',
    component: Dashboard
  },

  // Catálogo de productos.
  {
    path: 'productos',
    component: Productos
  },

  // Carrito de compras.
  {
    path: 'carrito',
    component: Carrito
  },

  // Confirmación antes de crear el pedido.
  {
    path: 'confirmar-pedido',
    component: ConfirmarPedido
  },

  // Historial de pedidos del Cliente.
  {
    path: 'mis-pedidos',
    component: MisPedidos
  },

  // Muestra el detalle de un pedido específico.
  {
    path: 'detalle-pedido/:id',
    component: DetallePedido
  },

  // Permite pagar un pedido específico.
  {
    path: 'pago-pedido/:id',
    component: PagoPedido
  },

  // Muestra el seguimiento de un pedido específico.
  {
    path: 'seguimiento-pedido/:id',
    component: SeguimientoPedido
  },

  // Muestra el perfil del Cliente.
  {
    path: 'perfil',
    component: Perfil
  },

  // Dashboard principal del Empleado.
  {
    path: 'empleado-dashboard',
    component: EmpleadoDashboard
  },

  // Pantalla de inventario.
  {
    path: 'inventario',
    component: Inventario
  },

  // Pantalla de movimientos de inventario.
  {
    path: 'movimientos-inventario',
    component: MovimientosInventario
  },

  // Pantalla para gestionar pedidos.
  {
    path: 'gestion-pedidos',
    component: GestionPedidos
  },

  // Dashboard principal del Administrador.
  {
    path: 'admin-dashboard',
    component: AdminDashboard
  },

  // Gestión de productos.
  {
    path: 'gestion-productos',
    component: GestionProductos
  },

  // Gestión de categorías.
  {
    path: 'gestion-categorias',
    component: GestionCategorias
  },

  // Gestión de usuarios.
  {
    path: 'gestion-usuarios',
    component: GestionUsuarios
  },

  // Redirige al Login cuando no se indica ninguna ruta.
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },

  // Redirige al Login cualquier dirección inexistente.
  {
    path: '**',
    redirectTo: 'login'
  }
];
