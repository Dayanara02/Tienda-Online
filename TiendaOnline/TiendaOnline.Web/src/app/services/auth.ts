// Permite crear e inyectar servicios.
import {
  inject,
  Injectable
} from '@angular/core';

// Permite realizar peticiones HTTP.
import {
  HttpClient
} from '@angular/common/http';

// Permite manejar respuestas asíncronas.
import {
  Observable
} from 'rxjs';

// Importa la configuración del ambiente.
import {
  environment
} from '../../environments/environment';

// Guarda la dirección principal de la API.
const baseUrl =
  environment.apiUrl;

// Representa los datos del inicio de sesión.
export interface LoginPeticion {

  // Correo del usuario.
  correo: string;

  // Contraseña del usuario.
  contrasena: string;
}

// Representa los datos del registro.
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

// Representa la respuesta del login.
export interface LoginRespuesta {

  // Token generado por la API.
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

// Permite utilizar el servicio en toda la aplicación.
@Injectable({
  providedIn: 'root'
})
export class AuthService {

  // Inyecta HttpClient.
  private readonly http =
    inject(HttpClient);

  // Inicia sesión.
  iniciarSesion(
    datos: LoginPeticion
  ): Observable<LoginRespuesta> {

    // Envía los datos al endpoint login.
    return this.http.post<LoginRespuesta>(
      `${baseUrl}/Auth/login`,
      datos
    );
  }

  // Registra un nuevo usuario.
  registrar(
    datos: RegistroPeticion
  ): Observable<any> {

    // Envía los datos al endpoint registrar.
    return this.http.post<any>(
      `${baseUrl}/Auth/registrar`,
      datos
    );
  }

  // Guarda el token.
  guardarToken(
    token: string
  ): void {

    // Guarda el token en el navegador.
    localStorage.setItem(
      'token',
      token
    );
  }

  // Obtiene el token guardado.
  obtenerToken():
    string | null {

    // Devuelve el token.
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

  // Obtiene el rol guardado.
  obtenerRol():
    string | null {

    // Devuelve el rol.
    return localStorage.getItem(
      'rol'
    );
  }

  // Comprueba si existe sesión.
  estaAutenticado():
    boolean {

    // Verifica si existe token.
    return !!this.obtenerToken();
  }

  // Cierra la sesión.
  cerrarSesion(): void {

    // Elimina el token.
    localStorage.removeItem(
      'token'
    );

    // Elimina el rol.
    localStorage.removeItem(
      'rol'
    );

    // Elimina el identificador.
    localStorage.removeItem(
      'idUsuario'
    );

    // Elimina el nombre.
    localStorage.removeItem(
      'nombreUsuario'
    );

    // Elimina el correo.
    localStorage.removeItem(
      'correoUsuario'
    );
  }
}

// Mantiene el nombre usado por los componentes.
export {
  AuthService as Auth
};
