import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-empleado-dashboard',
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './empleado-dashboard.html',
  styleUrl: './empleado-dashboard.css'
})
export class EmpleadoDashboard {
  nombreUsuario =
    localStorage.getItem('nombreUsuario') || 'Empleado';

  constructor(private router: Router) {}

  cerrarSesion(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('rol');
    localStorage.removeItem('nombreUsuario');

    this.router.navigate(['/login']);
  }
}
