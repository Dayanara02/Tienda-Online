import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-registro',
  imports: [
    CommonModule,
    FormsModule,
    RouterLink
  ],
  templateUrl: './registro.html',
  styleUrl: './registro.css'
})
export class Registro {
  nombre = '';
  apellido = '';
  correo = '';
  telefono = '';
  contrasena = '';
  confirmarContrasena = '';

  cargando = false;
  mensajeError = '';
  mensajeExito = '';

  private readonly urlRegistro =
    'https://localhost:7196/api/Auth/registrar';

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  registrarse(): void {
    this.mensajeError = '';
    this.mensajeExito = '';

    if (
      !this.nombre.trim() ||
      !this.apellido.trim() ||
      !this.correo.trim() ||
      !this.telefono.trim() ||
      !this.contrasena.trim() ||
      !this.confirmarContrasena.trim()
    ) {
      this.mensajeError =
        'Debe completar todos los campos.';
      return;
    }

    if (this.contrasena !== this.confirmarContrasena) {
      this.mensajeError =
        'Las contraseñas no coinciden.';
      return;
    }

    if (this.contrasena.length < 6) {
      this.mensajeError =
        'La contraseña debe tener al menos 6 caracteres.';
      return;
    }

    this.cargando = true;

    const datosRegistro = {
      nombre: this.nombre.trim(),
      apellido: this.apellido.trim(),
      correo: this.correo.trim(),
      contrasena: this.contrasena,
      telefono: this.telefono.trim()
    };

    this.http
      .post<any>(
        this.urlRegistro,
        datosRegistro
      )
      .subscribe({
        next: () => {
          this.cargando = false;

          this.mensajeExito =
            'Cuenta creada correctamente. Ahora puedes iniciar sesión.';

          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 1500);
        },

        error: (error) => {
          this.cargando = false;

          if (error.status === 0) {
            this.mensajeError =
              'No se pudo conectar con la API. Verifique que esté ejecutándose.';
          } else if (error.status === 400) {
            this.mensajeError =
              error.error?.mensaje ||
              error.error ||
              'No se pudo crear la cuenta. Revise los datos.';
          } else {
            this.mensajeError =
              error.error?.mensaje ||
              error.error ||
              'Ocurrió un error al registrar la cuenta.';
          }
        }
      });
  }
}