// Permite realizar peticiones HTTP.
import { HttpClient } from '@angular/common/http';

// Permite crear servicios e inyectar dependencias.
import {
  inject,
  Injectable
} from '@angular/core';

// Permite manejar respuestas asíncronas.
import { Observable } from 'rxjs';

// Datos necesarios para iniciar sesión.
export interface LoginPeticion {

  // Correo del usuario.
  correo: string;

  // Contraseña del usuario.
  contrasena: string;
}

// Datos necesarios para registrarse.
export interface RegistroPeticion {

  // Nombre del usuario.
  nombre: string;

  // Apellido del usuario.
  apellido: string;

  // Correo del usuario.
  correo: string;

  // Contraseña del usuario.
  contrasena: string;
}

// Respuesta del inicio de sesión.
export interface LoginRespuesta {

  // Token generado.
  token: string;

  // Identificador del usuario.
  idUsuario: number;

  // Nombre del usuario.
  nombre?: string;

  // Nombre completo.
  nombreCompleto?: string;

  // Rol del usuario.
  rol: string;
}

// Permite usar el servicio en toda la aplicación.
@Injectable({
  providedIn: 'root'
})
export class Auth {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Dirección principal de Auth.
  private readonly apiUrl =
    'https://localhost:7196/api/Auth';

  // Envía los datos del login.
  iniciarSesion(
    datos: LoginPeticion
  ): Observable<LoginRespuesta> {

    // Llama al endpoint login.
    return this.http.post<LoginRespuesta>(
      `${this.apiUrl}/login`,
      datos
    );
  }

  // Registra un nuevo usuario.
  registrar(
    datos: RegistroPeticion
  ): Observable<any> {

    // Llama al endpoint registrar.
    return this.http.post(
      `${this.apiUrl}/registrar`,
      datos
    );
  }

  // Guarda el token.
  guardarToken(
    token: string
  ): void {

    // Guarda el token.
    localStorage.setItem(
      'token',
      token
    );
  }

  // Obtiene el token.
  obtenerToken(): string | null {

    // Devuelve el token guardado.
    return localStorage.getItem(
      'token'
    );
  }

  // Guarda el rol.
  guardarRol(
    rol: string
  ): void {

    // Guarda el rol.
    localStorage.setItem(
      'rol',
      rol
    );
  }

  // Obtiene el rol.
  obtenerRol(): string | null {

    // Devuelve el rol guardado.
    return localStorage.getItem(
      'rol'
    );
  }

  // Comprueba si existe sesión.
  estaAutenticado(): boolean {

    // Comprueba si existe token.
    return !!this.obtenerToken();
  }

  // Cierra la sesión.
  cerrarSesion(): void {

    // Elimina el token.
    localStorage.removeItem('token');

    // Elimina el rol.
    localStorage.removeItem('rol');

    // Elimina el usuario.
    localStorage.removeItem('idUsuario');

    // Elimina el nombre.
    localStorage.removeItem('nombreUsuario');

    // Elimina el correo.
    localStorage.removeItem('correoUsuario');
  }
}
