import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

interface Promocion {
  id: number;
  nombre: string;
  descripcion: string;
  cantidadMinima: number;
  porcentaje: number;
  icono: string;
}

@Component({
  selector: 'app-descuentos',
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './descuentos.html',
  styleUrl: './descuentos.css'
})
export class Descuentos {

  promocionActivaId = 0;

  promociones: Promocion[] = [
    {
      id: 1,
      nombre: 'Dúo Esencia',
      descripcion: 'Ideal para una compra pequeña y aprovechar un beneficio rápido.',
      cantidadMinima: 2,
      porcentaje: 5,
      icono: '🌿'
    },
    {
      id: 2,
      nombre: 'Rutina Completa',
      descripcion: 'Arma una rutina más completa y recibe un mejor descuento.',
      cantidadMinima: 5,
      porcentaje: 10,
      icono: '✨'
    },
    {
      id: 3,
      nombre: 'Beauty Lover',
      descripcion: 'Perfecto para quienes disfrutan tener diferentes productos de cuidado.',
      cantidadMinima: 10,
      porcentaje: 15,
      icono: '💗'
    },
    {
      id: 4,
      nombre: 'Esencia Plus',
      descripcion: 'Una compra grande merece un beneficio especial.',
      cantidadMinima: 20,
      porcentaje: 20,
      icono: '🌸'
    },
    {
      id: 5,
      nombre: 'Mega Esencia',
      descripcion: 'Nuestra promoción más grande para compras especiales.',
      cantidadMinima: 50,
      porcentaje: 30,
      icono: '👑'
    }
  ];

  constructor(
    private router: Router
  ) {
    const promocionGuardada =
      localStorage.getItem('promocionActiva');

    if (promocionGuardada) {
      const promocion =
        JSON.parse(promocionGuardada);

      this.promocionActivaId =
        promocion.id;
    }
  }

  activarPromocion(
    promocion: Promocion
  ): void {
    localStorage.setItem(
      'promocionActiva',
      JSON.stringify(promocion)
    );

    this.promocionActivaId =
      promocion.id;
  }

  desactivarPromocion(): void {
    localStorage.removeItem(
      'promocionActiva'
    );

    this.promocionActivaId = 0;
  }

  irAlCarrito(): void {
    this.router.navigate([
      '/carrito'
    ]);
  }

  cerrarSesion(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('rol');
    localStorage.removeItem('idUsuario');
    localStorage.removeItem('nombreUsuario');
    localStorage.removeItem('correoUsuario');

    this.router.navigate([
      '/login'
    ]);
  }
}