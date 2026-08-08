import { Routes } from '@angular/router';
import { Registro } from './paginas/registro/registro';
import { ListaDeseos } from './paginas/lista-deseos/lista-deseos';
import { Login } from './paginas/login/login';
import { Dashboard } from './paginas/dashboard/dashboard';
import { Productos } from './paginas/productos/productos';
import { Carrito } from './paginas/carrito/carrito';
import { ConfirmarPedido } from './paginas/confirmar-pedido/confirmar-pedido';
import { MisPedidos } from './paginas/mis-pedidos/mis-pedidos';
import { DetallePedido } from './paginas/detalle-pedido/detalle-pedido';
import { Perfil } from './paginas/perfil/perfil';

import { EmpleadoDashboard } from './paginas/empleado-dashboard/empleado-dashboard';
import { Inventario } from './paginas/inventario/inventario';
import { MovimientosInventario } from './paginas/movimientos-inventario/movimientos-inventario';
import { GestionPedidos } from './paginas/gestion-pedidos/gestion-pedidos';
import { Descuentos } from './paginas/descuentos/descuentos';
import { AdminDashboard } from './paginas/admin-dashboard/admin-dashboard';
import { GestionProductos } from './paginas/gestion-productos/gestion-productos';
import { GestionCategorias } from './paginas/gestion-categorias/gestion-categorias';
import { GestionUsuarios } from './paginas/gestion-usuarios/gestion-usuarios';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'registro', component: Registro },
  { path: 'lista-deseos', component: ListaDeseos },

// Descuentos
{
  path: 'descuentos',
  component: Descuentos
},
  // Cliente
  { path: 'dashboard', component: Dashboard },
  { path: 'productos', component: Productos },
  { path: 'carrito', component: Carrito },
  { path: 'confirmar-pedido', component: ConfirmarPedido },
  { path: 'mis-pedidos', component: MisPedidos },
  { path: 'detalle-pedido/:id', component: DetallePedido },
  { path: 'perfil', component: Perfil },

  // Empleado
  { path: 'empleado-dashboard', component: EmpleadoDashboard },
  { path: 'inventario', component: Inventario },
  {
    path: 'movimientos-inventario',
    component: MovimientosInventario
  },
  { path: 'gestion-pedidos', component: GestionPedidos },

  // Administrador
  { path: 'admin-dashboard', component: AdminDashboard },
  { path: 'gestion-productos', component: GestionProductos },
  { path: 'gestion-categorias', component: GestionCategorias },
  { path: 'gestion-usuarios', component: GestionUsuarios },

  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }

  
];