// Permite utilizar directivas básicas como *ngIf.
import { CommonModule } from '@angular/common';

// Importa las herramientas principales del componente.
import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

// Permite trabajar con formularios usando ngModel.
import { FormsModule } from '@angular/forms';

// Permite realizar solicitudes HTTP al backend.
import {
  HttpClient,
  HttpErrorResponse
} from '@angular/common/http';

// Permite navegar hacia otras páginas.
import { Router } from '@angular/router';

// Permite utilizar iconos de Angular Material.
import { MatIconModule } from '@angular/material/icon';

// Permite utilizar botones de Angular Material.
import { MatButtonModule } from '@angular/material/button';

// Permite utilizar campos de Angular Material.
import { MatFormFieldModule } from '@angular/material/form-field';

// Permite utilizar inputs de Angular Material.
import { MatInputModule } from '@angular/material/input';

// Permite utilizar botones de PrimeNG.
import { ButtonModule } from 'primeng/button';

// Representa los datos enviados al backend.
interface RegistroUsuario {
  nombre: string;
  apellido: string;
  correo: string;
  contrasena: string;
  telefono: string | null;
}

@Component({
  selector: 'app-registro',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    ButtonModule
  ],
  templateUrl: './registro.html',
  styleUrl: './registro.css'
})
export class Registro {

  // Dirección del endpoint de registro.
  private readonly apiUrl =
    'https://localhost:7196/api/Auth/registrar';

  // Datos del formulario.
  nombre = '';
  apellido = '';
  correo = '';
  telefono = '';
  contrasena = '';
  confirmarContrasena = '';

  // Permite mostrar u ocultar la contraseña.
  mostrarContrasena = false;

  // Controla el estado del botón.
  registrando = false;

  // Guarda mensajes mostrados al usuario.
  mensajeError = '';
  mensajeExito = '';

  constructor(
    private http: HttpClient,
    private router: Router,
    private cd: ChangeDetectorRef
  ) { }

  // Cambia la visibilidad de la contraseña.
  cambiarVisibilidad(): void {
    this.mostrarContrasena =
      !this.mostrarContrasena;
  }

  // Valida y registra al nuevo cliente.
  registrar(): void {

    // Limpia mensajes anteriores.
    this.mensajeError = '';
    this.mensajeExito = '';

    // Valida los campos obligatorios.
    if (
      !this.nombre.trim() ||
      !this.apellido.trim() ||
      !this.correo.trim() ||
      !this.contrasena
    ) {
      this.mensajeError =
        'Complete todos los campos obligatorios.';

      return;
    }

    // Valida el formato básico del correo.
    if (!this.correo.includes('@')) {
      this.mensajeError =
        'Ingrese un correo electrónico válido.';

      return;
    }

    // Valida la longitud mínima de contraseña.
    if (this.contrasena.length < 6) {
      this.mensajeError =
        'La contraseña debe tener al menos 6 caracteres.';

      return;
    }

    // Comprueba que ambas contraseñas coincidan.
    if (
      this.contrasena !==
      this.confirmarContrasena
    ) {
      this.mensajeError =
        'Las contraseñas no coinciden.';

      return;
    }

    // Construye los datos enviados a la API.
    const usuario: RegistroUsuario = {
      nombre: this.nombre.trim(),
      apellido: this.apellido.trim(),
      correo: this.correo.trim(),
      contrasena: this.contrasena,
      telefono:
        this.telefono.trim() || null
    };

    // Bloquea el botón mientras se registra.
    this.registrando = true;

    // Envía el registro al backend.
    this.http
      .post(
        this.apiUrl,
        usuario
      )
      .subscribe({
        next: () => {

          // Muestra confirmación.
          this.mensajeExito =
            'Cuenta creada correctamente.';

          this.registrando = false;

          this.cd.detectChanges();

          // Envía al usuario al login.
          setTimeout(
            () => {
              this.router.navigate([
                '/login'
              ]);
            },
            1200
          );
        },

        error: (
          error: HttpErrorResponse
        ) => {

          this.registrando = false;

          // Correo ya registrado u otro conflicto.
          if (
            error.status === 400 ||
            error.status === 409
          ) {
            this.mensajeError =
              typeof error.error === 'string'
                ? error.error
                : 'No fue posible registrar la cuenta.';
          }

          // Error de conexión.
          else if (error.status === 0) {
            this.mensajeError =
              'No se pudo conectar con la API.';
          }

          // Error general.
          else {
            this.mensajeError =
              'Ocurrió un error al registrar la cuenta.';
          }

          this.cd.detectChanges();
        }
      });
  }

  // Regresa a la pantalla de inicio de sesión.
  volverLogin(): void {
    this.router.navigate([
      '/login'
    ]);
  }
}
