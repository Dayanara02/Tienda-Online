import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import {
  ChangeDetectorRef,
  Component
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  Router,
  RouterLink
} from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [
    CommonModule,
    FormsModule,
    RouterLink
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
    private router: Router,
    private detectorCambios: ChangeDetectorRef
  ) {}

  cambiarVisibilidadContrasena(): void {
    this.mostrarContrasena =
      !this.mostrarContrasena;
  }

  iniciarSesion(): void {

    this.mensajeError = '';

    if (
      !this.correo.trim() ||
      !this.contrasena.trim()
    ) {
      this.mensajeError =
        'Debe escribir el correo y la contraseña.';

      this.detectorCambios.detectChanges();

      return;
    }

    this.cargando = true;

    this.detectorCambios.detectChanges();

    const datosLogin = {
      correo: this.correo.trim(),
      contrasena: this.contrasena
    };

    this.http
      .post<any>(
        this.urlLogin,
        datosLogin
      )
      .subscribe({

        next: (respuesta) => {

          this.cargando = false;

          if (!respuesta?.token) {
            this.mensajeError =
              'No se pudo iniciar sesión.';

            this.detectorCambios.detectChanges();

            return;
          }

          localStorage.setItem(
            'token',
            respuesta.token
          );

          localStorage.setItem(
            'rol',
            respuesta.rol
          );

          if (respuesta.idUsuario) {
            localStorage.setItem(
              'idUsuario',
              respuesta.idUsuario.toString()
            );
          }

          localStorage.setItem(
            'nombreUsuario',
            respuesta.nombreCompleto ||
            respuesta.nombre ||
            'Usuario'
          );

          const rol = respuesta.rol;

          if (rol === 'Administrador') {

            this.router.navigate([
              '/admin-dashboard'
            ]);

            return;
          }

          if (rol === 'Empleado') {

            this.router.navigate([
              '/empleado-dashboard'
            ]);

            return;
          }

          if (rol === 'Cliente') {

            this.router.navigate([
              '/dashboard'
            ]);

            return;
          }

          this.limpiarSesion();

          this.mensajeError =
            'El rol de esta cuenta no es válido.';

          this.detectorCambios.detectChanges();
        },

        error: (error) => {

          console.error(
            'Error de login:',
            error
          );

          this.cargando = false;

          if (error.status === 401) {

            this.mensajeError =
              'El correo o la contraseña son incorrectos.';

          } else if (error.status === 400) {

            this.mensajeError =
              error.error?.mensaje ||
              'Revise el correo y la contraseña.';

          } else if (error.status === 0) {

            this.mensajeError =
              'No se pudo conectar con la API. Verifique que esté ejecutándose.';

          } else {

            this.mensajeError =
              error.error?.mensaje ||
              'Ocurrió un error al iniciar sesión.';
          }

          /*
            Fuerza a Angular a actualizar
            inmediatamente el HTML.
          */
          this.detectorCambios.detectChanges();
        }
      });
  }

  private limpiarSesion(): void {

    localStorage.removeItem('token');
    localStorage.removeItem('rol');
    localStorage.removeItem('idUsuario');
    localStorage.removeItem('nombreUsuario');
  }
}
