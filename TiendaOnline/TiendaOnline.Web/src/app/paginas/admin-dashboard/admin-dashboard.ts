import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-admin-dashboard',
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboard {
  nombreUsuario =
    localStorage.getItem('nombreUsuario') || 'Administrador';

  constructor(private router: Router) {}

  cerrarSesion(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('rol');
    localStorage.removeItem('nombreUsuario');

    this.router.navigate(['/login']);
  }
}