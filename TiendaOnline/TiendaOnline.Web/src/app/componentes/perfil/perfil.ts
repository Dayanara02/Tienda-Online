// Importa CommonModule para poder utilizar
// directivas comunes de Angular dentro del HTML.
import { CommonModule } from '@angular/common';


// Importa Component para crear
// el componente Perfil.
import { Component } from '@angular/core';


// Importa Router para poder navegar
// hacia otras páginas del sistema.
import { Router } from '@angular/router';


// Importa MatIconModule de Angular Material
// para utilizar iconos dentro del Perfil.
import { MatIconModule } from '@angular/material/icon';


// Importa MatButtonModule de Angular Material
// para utilizar botones Material.
import { MatButtonModule } from '@angular/material/button';


// Configura el componente Perfil.
@Component({

  // Define el selector del componente.
  selector: 'app-perfil',

  // Registra los módulos que
  // vamos a utilizar en el HTML.
  imports: [

    // Permite utilizar funciones comunes de Angular.
    CommonModule,

    // Permite utilizar iconos Material.
    MatIconModule,

    // Permite utilizar botones Material.
    MatButtonModule
  ],

  // Indica el archivo HTML del Perfil.
  templateUrl: './perfil.html',

  // Indica el archivo CSS del Perfil.
  styleUrl: './perfil.css'
})
export class Perfil {


  // Guarda el identificador del usuario conectado.
  idUsuario = '';


  // Guarda el nombre completo del usuario.
  nombreCompleto = '';


  // Guarda el correo del usuario.
  correo = '';


  // Guarda el rol del usuario conectado.
  rol = '';


  // Constructor del componente Perfil.
  constructor(

    // Permite navegar hacia otras páginas.
    private router: Router
  ) {


    // Obtiene el token guardado
    // cuando el usuario inició sesión.
    const token =
      localStorage.getItem('token');


    // Comprueba si existe una sesión activa.
    if (!token) {


      // Si no existe token,
      // envía al usuario al Login.
      this.router.navigate([
        '/login'
      ]);


      // Detiene el constructor.
      return;
    }


    // Obtiene el identificador
    // guardado del usuario.
    this.idUsuario =
      localStorage.getItem(
        'idUsuario'
      ) || '';


    // Obtiene el nombre del usuario.
    this.nombreCompleto =
      localStorage.getItem(
        'nombreUsuario'
      ) || 'Usuario';


    // Obtiene el correo guardado.
    this.correo =
      localStorage.getItem(
        'correoUsuario'
      ) || '';


    // Obtiene el rol del usuario.
    this.rol =
      localStorage.getItem(
        'rol'
      ) || 'Cliente';
  }


  // Regresa al Dashboard correspondiente
  // según el rol del usuario.
  volver(): void {


    // Comprueba si el usuario
    // es Administrador.
    if (
      this.rol ===
      'Administrador'
    ) {


      // Navega al Dashboard del Administrador.
      this.router.navigate([
        '/admin-dashboard'
      ]);


      // Detiene el método.
      return;
    }


    // Comprueba si el usuario
    // es Empleado.
    if (
      this.rol ===
      'Empleado'
    ) {


      // Navega al Dashboard del Empleado.
      this.router.navigate([
        '/empleado-dashboard'
      ]);


      // Detiene el método.
      return;
    }


    // Si no es Administrador ni Empleado,
    // regresa al Dashboard del Cliente.
    this.router.navigate([
      '/dashboard'
    ]);
  }


  // Cierra la sesión actual.
  cerrarSesion(): void {


    // Elimina el token JWT.
    localStorage.removeItem(
      'token'
    );


    // Elimina el rol.
    localStorage.removeItem(
      'rol'
    );


    // Elimina el identificador del usuario.
    localStorage.removeItem(
      'idUsuario'
    );


    // Elimina el nombre del usuario.
    localStorage.removeItem(
      'nombreUsuario'
    );


    // Elimina el correo guardado.
    localStorage.removeItem(
      'correoUsuario'
    );


    // Redirige nuevamente al Login.
    this.router.navigate([
      '/login'
    ]);
  }
}
