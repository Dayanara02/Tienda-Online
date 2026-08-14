// Permite usar directivas comunes de Angular.
import { CommonModule } from '@angular/common';

// Permite identificar errores HTTP.
import { HttpErrorResponse } from '@angular/common/http';

// Importa herramientas del componente.
import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

// Permite usar ngModel.
import { FormsModule } from '@angular/forms';

// Permite navegar y usar enlaces.
import {
  Router,
  RouterLink
} from '@angular/router';

// Permite usar campos Material.
import { MatFormFieldModule } from '@angular/material/form-field';

// Permite usar inputs Material.
import { MatInputModule } from '@angular/material/input';

// Permite usar iconos.
import { MatIconModule } from '@angular/material/icon';

// Permite usar botones Material.
import { MatButtonModule } from '@angular/material/button';

// Permite mostrar un spinner.
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

// Importa el servicio de autenticación.
import { Auth } from '../../services/auth';

// Configura el componente Login.
@Component({
  // Nombre del componente.
  selector: 'app-login',

  // Módulos utilizados.
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],

  // Archivo HTML.
  templateUrl: './login.html',

  // Archivo CSS.
  styleUrl: './login.css'
})
export class Login {

  // Guarda el correo.
  correo = '';

  // Guarda la contraseña.
  contrasena = '';

  // Controla la contraseña visible.
  mostrarContrasena = false;

  // Indica si está cargando.
  cargando = false;

  // Guarda mensajes de error.
  mensajeError = '';

  // Recibe los servicios necesarios.
  constructor(
    // Servicio de autenticación.
    private auth: Auth,

    // Permite navegar.
    private router: Router,

    // Actualiza la pantalla.
    private detectorCambios: ChangeDetectorRef
  ) { }

  // Muestra u oculta la contraseña.
  cambiarVisibilidadContrasena(): void {

    // Cambia el valor actual.
    this.mostrarContrasena =
      !this.mostrarContrasena;
  }

  // Inicia el proceso de login.
  iniciarSesion(): void {

    // Limpia errores anteriores.
    this.mensajeError = '';

    // Valida los campos.
    if (
      !this.correo.trim() ||
      !this.contrasena.trim()
    ) {
      // Muestra el error.
      this.mensajeError =
        'Debe escribir el correo y la contraseña.';

      // Actualiza la pantalla.
      this.detectorCambios.detectChanges();

      // Detiene el proceso.
      return;
    }

    // Activa la carga.
    this.cargando = true;

    // Actualiza la pantalla.
    this.detectorCambios.detectChanges();

    // Prepara los datos.
    const datosLogin = {

      // Guarda el correo limpio.
      correo: this.correo.trim(),

      // Guarda la contraseña.
      contrasena: this.contrasena
    };

    // Utiliza el servicio Auth.
    this.auth
      .iniciarSesion(
        datosLogin
      )
      .subscribe({

        // Se ejecuta si funciona.
        next: (respuesta) => {

          // Finaliza la carga.
          this.cargando = false;

          // Comprueba el token.
          if (!respuesta?.token) {

            // Muestra un error.
            this.mensajeError =
              'No se pudo iniciar sesión.';

            // Actualiza la pantalla.
            this.detectorCambios.detectChanges();

            // Detiene el proceso.
            return;
          }

          // Guarda el token.
          this.auth.guardarToken(
            respuesta.token
          );

          // Guarda el rol.
          this.auth.guardarRol(
            respuesta.rol
          );

          // Comprueba el usuario.
          if (respuesta.idUsuario) {

            // Guarda su identificador.
            localStorage.setItem(
              'idUsuario',
              respuesta.idUsuario.toString()
            );
          }

          // Guarda el nombre.
          localStorage.setItem(
            'nombreUsuario',

            respuesta.nombreCompleto ||
            respuesta.nombre ||
            'Usuario'
          );

          // Guarda el correo.
          localStorage.setItem(
            'correoUsuario',
            this.correo.trim()
          );

          // Obtiene el rol.
          const rol =
            respuesta.rol;

          // Verifica Administrador.
          if (
            rol === 'Administrador'
          ) {
            // Abre su dashboard.
            this.router.navigate([
              '/admin-dashboard'
            ]);

            // Detiene el proceso.
            return;
          }

          // Verifica Empleado.
          if (
            rol === 'Empleado'
          ) {
            // Abre su dashboard.
            this.router.navigate([
              '/empleado-dashboard'
            ]);

            // Detiene el proceso.
            return;
          }

          // Verifica Cliente.
          if (
            rol === 'Cliente'
          ) {
            // Abre su dashboard.
            this.router.navigate([
              '/dashboard'
            ]);

            // Detiene el proceso.
            return;
          }

          // Limpia una sesión inválida.
          this.auth.cerrarSesion();

          // Muestra el error.
          this.mensajeError =
            'El rol de esta cuenta no es válido.';

          // Actualiza la pantalla.
          this.detectorCambios.detectChanges();
        },

        // Se ejecuta si falla.
        error: (
          error: HttpErrorResponse
        ) => {

          // Muestra el error en consola.
          console.error(
            'Error de login:',
            error
          );

          // Finaliza la carga.
          this.cargando = false;

          // Credenciales incorrectas.
          if (
            error.status === 401
          ) {
            // Usa el mensaje de la API.
            this.mensajeError =
              error.error?.mensaje ||
              'El correo o la contraseña son incorrectos.';
          }

          // Datos incorrectos.
          else if (
            error.status === 400
          ) {
            // Usa el mensaje recibido.
            this.mensajeError =
              error.error?.mensaje ||
              'Revise el correo y la contraseña.';
          }

          // API sin conexión.
          else if (
            error.status === 0
          ) {
            // Informa el problema.
            this.mensajeError =
              'No se pudo conectar con la API. Verifique que esté ejecutándose.';
          }

          // Cualquier otro error.
          else {
            // Usa un mensaje general.
            this.mensajeError =
              error.error?.mensaje ||
              'Ocurrió un error al iniciar sesión.';
          }

          // Actualiza la pantalla.
          this.detectorCambios.detectChanges();
        }
      });
  }
}
