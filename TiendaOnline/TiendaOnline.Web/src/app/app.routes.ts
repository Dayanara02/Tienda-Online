// Importa el tipo Routes de Angular.
//
// Routes permite definir todas las rutas
// disponibles dentro de la aplicación.
import { Routes } from '@angular/router';


// =========================================================
// IMPORTACIONES DE PÁGINAS GENERALES
// =========================================================

// Importa la pantalla de registro de usuarios.
import { Registro } from './paginas/registro/registro';

// Importa la pantalla de lista de deseos.
import { ListaDeseos } from './paginas/lista-deseos/lista-deseos';

// Importa la pantalla de inicio de sesión.
import { Login } from './paginas/login/login';


// =========================================================
// IMPORTACIONES DE PÁGINAS DEL CLIENTE
// =========================================================

// Importa el dashboard principal del Cliente.
import { Dashboard } from './paginas/dashboard/dashboard';

// Importa la página donde se muestran los productos.
import { Productos } from './paginas/productos/productos';

// Importa la página del carrito de compras.
import { Carrito } from './paginas/carrito/carrito';

// Importa la página donde el Cliente
// confirma la información antes de crear el pedido.
import { ConfirmarPedido } from './paginas/confirmar-pedido/confirmar-pedido';

// Importa la página donde se muestra
// el historial de pedidos del Cliente.
import { MisPedidos } from './paginas/mis-pedidos/mis-pedidos';

// Importa la página donde se consulta
// toda la información de un pedido específico.
import { DetallePedido } from './paginas/detalle-pedido/detalle-pedido';

// Importa la nueva pantalla donde el Cliente
// selecciona el método de pago y paga su pedido.
import { PagoPedido } from './paginas/pago-pedido/pago-pedido';

// Importa la página del perfil del Cliente.
import { Perfil } from './paginas/perfil/perfil';

// Importa la página de descuentos y promociones.
import { Descuentos } from './paginas/descuentos/descuentos';


// =========================================================
// IMPORTACIONES DE PÁGINAS DEL EMPLEADO
// =========================================================

// Importa el dashboard principal del Empleado.
import { EmpleadoDashboard } from './paginas/empleado-dashboard/empleado-dashboard';

// Importa la página de inventario.
import { Inventario } from './paginas/inventario/inventario';

// Importa la página donde se consultan
// los movimientos realizados en el inventario.
import { MovimientosInventario } from './paginas/movimientos-inventario/movimientos-inventario';

// Importa la página donde el Empleado
// puede gestionar los pedidos.
import { GestionPedidos } from './paginas/gestion-pedidos/gestion-pedidos';


// =========================================================
// IMPORTACIONES DE PÁGINAS DEL ADMINISTRADOR
// =========================================================

// Importa el dashboard principal del Administrador.
import { AdminDashboard } from './paginas/admin-dashboard/admin-dashboard';

// Importa la página para gestionar productos.
import { GestionProductos } from './paginas/gestion-productos/gestion-productos';

// Importa la página para gestionar categorías.
import { GestionCategorias } from './paginas/gestion-categorias/gestion-categorias';

// Importa la página para gestionar usuarios.
import { GestionUsuarios } from './paginas/gestion-usuarios/gestion-usuarios';


// =========================================================
// CONFIGURACIÓN DE TODAS LAS RUTAS
// =========================================================

// Este arreglo contiene todas las direcciones
// disponibles dentro de la aplicación Angular.
export const routes: Routes = [


  // =======================================================
  // RUTAS GENERALES
  // =======================================================

  // Ruta para iniciar sesión.
  {
    path: 'login',
    component: Login
  },

  // Ruta para registrar un nuevo usuario.
  {
    path: 'registro',
    component: Registro
  },

  // Ruta para consultar la lista de deseos.
  {
    path: 'lista-deseos',
    component: ListaDeseos
  },

  // Ruta para consultar promociones y descuentos.
  {
    path: 'descuentos',
    component: Descuentos
  },


  // =======================================================
  // RUTAS DEL CLIENTE
  // =======================================================

  // Dashboard principal del Cliente.
  {
    path: 'dashboard',
    component: Dashboard
  },

  // Página donde se muestran los productos.
  {
    path: 'productos',
    component: Productos
  },

  // Página del carrito de compras.
  {
    path: 'carrito',
    component: Carrito
  },

  // Página donde se confirma el pedido
  // antes de guardarlo en la base de datos.
  {
    path: 'confirmar-pedido',
    component: ConfirmarPedido
  },

  // Historial de pedidos del Cliente.
  {
    path: 'mis-pedidos',
    component: MisPedidos
  },

  // Detalle de un pedido específico.
  //
  // El parámetro :id representa
  // el identificador del pedido.
  //
  // Ejemplo:
  //
  // /detalle-pedido/5
  {
    path: 'detalle-pedido/:id',
    component: DetallePedido
  },

  // Pantalla para pagar un pedido.
  //
  // También recibe el identificador
  // del pedido dentro de la URL.
  //
  // Ejemplo:
  //
  // /pago-pedido/5
  {
    path: 'pago-pedido/:id',
    component: PagoPedido
  },

  // Página del perfil del Cliente.
  {
    path: 'perfil',
    component: Perfil
  },


  // =======================================================
  // RUTAS DEL EMPLEADO
  // =======================================================

  // Dashboard principal del Empleado.
  {
    path: 'empleado-dashboard',
    component: EmpleadoDashboard
  },

  // Página de inventario.
  {
    path: 'inventario',
    component: Inventario
  },

  // Página donde se muestran
  // los movimientos de inventario.
  {
    path: 'movimientos-inventario',
    component: MovimientosInventario
  },

  // Página donde el Empleado
  // administra los pedidos.
  {
    path: 'gestion-pedidos',
    component: GestionPedidos
  },


  // =======================================================
  // RUTAS DEL ADMINISTRADOR
  // =======================================================

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


  // =======================================================
  // RUTA INICIAL
  // =======================================================

  // Cuando la dirección está vacía,
  // redirige automáticamente al login.
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },


  // =======================================================
  // RUTA PARA DIRECCIONES NO EXISTENTES
  // =======================================================

  // Los dos asteriscos representan
  // cualquier ruta que no exista.
  //
  // Si el usuario escribe una dirección incorrecta,
  // Angular lo devuelve al login.
  {
    path: '**',
    redirectTo: 'login'
  }

];
