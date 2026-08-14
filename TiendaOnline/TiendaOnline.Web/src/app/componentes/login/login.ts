// Permite usar directivas comunes de Angular.
import { CommonModule } from '@angular/common';

// Permite hacer peticiones HTTP.
import { HttpClient } from '@angular/common/http';

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

// Permite usar campos de Angular Material.
import { MatFormFieldModule } from '@angular/material/form-field';

// Permite usar inputs de Angular Material.
import { MatInputModule } from '@angular/material/input';

// Permite usar iconos.
import { MatIconModule } from '@angular/material/icon';

// Permite usar botones Material.
import { MatButtonModule } from '@angular/material/button';

// Permite mostrar un spinner de carga.
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

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

  // Controla si se muestra la contraseña.
  mostrarContrasena = false;

  // Indica si está cargando.
  cargando = false;

  // Guarda el mensaje de error.
  mensajeError = '';

  // Endpoint del login.
  private readonly urlLogin =
    'https://localhost:7196/api/Auth/login';

  // Recibe los servicios necesarios.
  constructor(
    private http: HttpClient,
    private router: Router,
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

    // Valida que los campos tengan datos.
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

    // Activa el estado de carga.
    this.cargando = true;

    // Actualiza la pantalla.
    this.detectorCambios.detectChanges();

    // Prepara los datos del login.
    const datosLogin = {
      // Envía el correo sin espacios.
      correo: this.correo.trim(),

      // Envía la contraseña.
      contrasena: this.contrasena
    };

    // Realiza la petición al backend.
    this.http
      .post<any>(
        this.urlLogin,
        datosLogin
      )
      .subscribe({

        // Se ejecuta si el login funciona.
        next: (respuesta) => {

          // Finaliza la carga.
          this.cargando = false;

          // Verifica que exista token.
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
          localStorage.setItem(
            'token',
            respuesta.token
          );

          // Guarda el rol.
          localStorage.setItem(
            'rol',
            respuesta.rol
          );

          // Verifica que exista IdUsuario.
          if (respuesta.idUsuario) {
            // Guarda el identificador.
            localStorage.setItem(
              'idUsuario',
              respuesta.idUsuario.toString()
            );
          }

          // Guarda el nombre del usuario.
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

          // Obtiene el rol recibido.
          const rol = respuesta.rol;

          // Verifica si es Administrador.
          if (rol === 'Administrador') {
            // Abre el dashboard administrador.
            this.router.navigate([
              '/admin-dashboard'
            ]);

            // Detiene el método.
            return;
          }

          // Verifica si es Empleado.
          if (rol === 'Empleado') {
            // Abre el dashboard empleado.
            this.router.navigate([
              '/empleado-dashboard'
            ]);

            // Detiene el método.
            return;
          }

          // Verifica si es Cliente.
          if (rol === 'Cliente') {
            // Abre el dashboard cliente.
            this.router.navigate([
              '/dashboard'
            ]);

            // Detiene el método.
            return;
          }

          // Limpia la sesión si el rol no sirve.
          this.limpiarSesion();

          // Muestra un error de rol.
          this.mensajeError =
            'El rol de esta cuenta no es válido.';

          // Actualiza la pantalla.
          this.detectorCambios.detectChanges();
        },

        // Se ejecuta si ocurre un error.
        error: (error) => {

          // Muestra el error en consola.
          console.error(
            'Error de login:',
            error
          );

          // Finaliza la carga.
          this.cargando = false;

          // Error por credenciales incorrectas.
          if (error.status === 401) {
            // Usa el mensaje del backend si existe.
            this.mensajeError =
              error.error?.mensaje ||
              'El correo o la contraseña son incorrectos.';
          }

          // Error por datos inválidos.
          else if (error.status === 400) {
            // Usa el mensaje enviado por la API.
            this.mensajeError =
              error.error?.mensaje ||
              'Revise el correo y la contraseña.';
          }

          // Error de conexión con la API.
          else if (error.status === 0) {
            // Informa que la API no responde.
            this.mensajeError =
              'No se pudo conectar con la API. Verifique que esté ejecutándose.';
          }

          // Cualquier otro error.
          else {
            // Usa el mensaje del backend o uno general.
            this.mensajeError =
              error.error?.mensaje ||
              'Ocurrió un error al iniciar sesión.';
          }

          // Actualiza la pantalla.
          this.detectorCambios.detectChanges();
        }
      });
  }

  // Limpia los datos de sesión.
  private limpiarSesion(): void {

    // Elimina el token.
    localStorage.removeItem('token');

    // Elimina el rol.
    localStorage.removeItem('rol');

    // Elimina el IdUsuario.
    localStorage.removeItem('idUsuario');

    // Elimina el nombre.
    localStorage.removeItem('nombreUsuario');

    // Elimina el correo.
    localStorage.removeItem('correoUsuario');
  }
}
