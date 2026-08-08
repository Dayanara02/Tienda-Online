import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-perfil',
  imports: [CommonModule],
  templateUrl: './perfil.html',
  styleUrl: './perfil.css'
})
export class Perfil {
  idUsuario = '';
  nombreCompleto = '';
  correo = '';
  rol = '';

  constructor(private router: Router) {
    const token = localStorage.getItem('token');

    if (!token) {
      this.router.navigate(['/login']);
      return;
    }

    this.idUsuario =
      localStorage.getItem('idUsuario') || '';

    this.nombreCompleto =
      localStorage.getItem('nombreUsuario') || 'Usuario';

    this.correo =
      localStorage.getItem('correoUsuario') || '';

    this.rol =
      localStorage.getItem('rol') || 'Cliente';
  }

  volver(): void {
    if (this.rol === 'Administrador') {
      this.router.navigate(['/admin-dashboard']);
      return;
    }

    if (this.rol === 'Empleado') {
      this.router.navigate(['/empleado-dashboard']);
      return;
    }

    this.router.navigate(['/dashboard']);
  }

  cerrarSesion(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('rol');
    localStorage.removeItem('idUsuario');
    localStorage.removeItem('nombreUsuario');
    localStorage.removeItem('correoUsuario');

    this.router.navigate(['/login']);
  }
}