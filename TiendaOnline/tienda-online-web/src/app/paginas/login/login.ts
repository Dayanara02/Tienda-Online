import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  correo = '';
  contrasena = '';
  mostrarContrasena = false;
  cargando = false;
  mensajeError = '';

  private readonly urlLogin =
    'https://localhost:7196/api/Auth/login';

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  cambiarVisibilidadContrasena(): void {
    this.mostrarContrasena = !this.mostrarContrasena;
  }

  iniciarSesion(): void {
    this.mensajeError = '';

    if (!this.correo.trim() || !this.contrasena.trim()) {
      this.mensajeError =
        'Debe escribir el correo y la contraseña.';
      return;
    }

    this.cargando = true;

    const datosLogin = {
      correo: this.correo.trim(),
      contrasena: this.contrasena
    };

    this.http
      .post<any>(this.urlLogin, datosLogin)
      .subscribe({
        next: (respuesta) => {
       localStorage.setItem('token', respuesta.token);

       if (respuesta.rol) {
      localStorage.setItem('rol', respuesta.rol);
       }

       if (respuesta.nombre) {
       localStorage.setItem('nombreUsuario', respuesta.nombre);
      }

      this.cargando = false;
      this.cargando = false;
     this.mensajeError = 'Inicio de sesión correcto.';
      },

        error: (error) => {
          this.cargando = false;

          if (error.status === 401) {
            this.mensajeError =
              'El correo o la contraseña son incorrectos.';
          } else if (error.status === 0) {
            this.mensajeError =
              'No se pudo conectar con la API. Verifique que esté ejecutándose.';
          } else {
            this.mensajeError =
              error.error?.mensaje ||
              error.error ||
              'Ocurrió un error al iniciar sesión.';
          }
        }
      });
  }
}