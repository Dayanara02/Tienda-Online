// Importa CommonModule para poder utilizar
// directivas de Angular como *ngIf.
import { CommonModule } from '@angular/common';


// Importa HttpClient para poder realizar
// peticiones HTTP hacia la API.
import { HttpClient } from '@angular/common/http';


// Importa las herramientas principales
// necesarias para crear el componente.
import {
  ChangeDetectorRef,
  Component
} from '@angular/core';


// Importa FormsModule para poder utilizar
// [(ngModel)] en los campos del formulario.
import { FormsModule } from '@angular/forms';


// Importa Router para navegar entre páginas
// y RouterLink para utilizar enlaces en el HTML.
import {
  Router,
  RouterLink
} from '@angular/router';


// Importa MatFormFieldModule de Angular Material.
// Permite utilizar <mat-form-field>
// para organizar visualmente los campos.
import { MatFormFieldModule } from '@angular/material/form-field';


// Importa MatInputModule de Angular Material.
// Permite utilizar la directiva matInput
// dentro de los campos de correo y contraseña.
import { MatInputModule } from '@angular/material/input';


// Importa MatIconModule de Angular Material.
// Permite utilizar iconos con <mat-icon>.
import { MatIconModule } from '@angular/material/icon';


// Importa MatButtonModule de Angular Material.
// Permite utilizar botones Material.
import { MatButtonModule } from '@angular/material/button';


// Importa MatProgressSpinnerModule de Angular Material.
// Permite mostrar un spinner mientras
// se está procesando el inicio de sesión.
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';


// Define la configuración del componente Login.
@Component({

  // Define el selector utilizado por Angular
  // para identificar este componente.
  selector: 'app-login',

  // Registra todos los módulos que
  // este componente puede utilizar.
  imports: [

    // Permite utilizar directivas comunes de Angular.
    CommonModule,

    // Permite utilizar [(ngModel)].
    FormsModule,

    // Permite utilizar routerLink.
    RouterLink,

    // Permite utilizar campos visuales de Angular Material.
    MatFormFieldModule,

    // Permite utilizar inputs de Angular Material.
    MatInputModule,

    // Permite utilizar iconos de Angular Material.
    MatIconModule,

    // Permite utilizar botones de Angular Material.
    MatButtonModule,

    // Permite utilizar el indicador circular de carga.
    MatProgressSpinnerModule
  ],

  // Define el archivo HTML correspondiente al Login.
  templateUrl: './login.html',

  // Define el archivo CSS correspondiente al Login.
  styleUrl: './login.css'
})
export class Login {


  // Guarda el correo escrito por el usuario.
  correo = '';


  // Guarda la contraseña escrita por el usuario.
  contrasena = '';


  // Controla si la contraseña
  // se muestra o se mantiene oculta.
  mostrarContrasena = false;


  // Indica si el sistema está
  // procesando actualmente el inicio de sesión.
  cargando = false;


  // Guarda el mensaje de error
  // que se mostrará en el HTML.
  mensajeError = '';


  // Guarda la dirección del endpoint
  // utilizado para iniciar sesión.
  private readonly urlLogin =
    'https://localhost:7196/api/Auth/login';


  // Constructor del componente.
  constructor(

    // Permite realizar peticiones HTTP.
    private http: HttpClient,

    // Permite navegar entre rutas.
    private router: Router,

    // Permite forzar la actualización
    // visual del componente cuando sea necesario.
    private detectorCambios: ChangeDetectorRef
  ) { }


  // Cambia entre mostrar
  // y ocultar la contraseña.
  cambiarVisibilidadContrasena(): void {

    // Invierte el valor actual.
    // Si era false pasa a true
    // y si era true pasa a false.
    this.mostrarContrasena =
      !this.mostrarContrasena;
  }


  // Ejecuta el proceso de inicio de sesión.
  iniciarSesion(): void {


    // Limpia cualquier mensaje
    // de error mostrado anteriormente.
    this.mensajeError = '';


    // Comprueba que el usuario
    // haya escrito correo y contraseña.
    if (
      !this.correo.trim() ||
      !this.contrasena.trim()
    ) {


      // Muestra un mensaje si falta algún dato.
      this.mensajeError =
        'Debe escribir el correo y la contraseña.';


      // Fuerza la actualización del HTML
      // para mostrar inmediatamente el mensaje.
      this.detectorCambios.detectChanges();


      // Detiene el método.
      return;
    }


    // Indica que comenzó
    // el proceso de inicio de sesión.
    this.cargando = true;


    // Actualiza el HTML
    // para mostrar el estado de carga.
    this.detectorCambios.detectChanges();


    // Crea el objeto que se enviará
    // hacia la API.
    const datosLogin = {


      // Envía el correo sin espacios
      // innecesarios al inicio o al final.
      correo: this.correo.trim(),


      // Envía la contraseña escrita.
      contrasena: this.contrasena
    };


    // Utiliza HttpClient para realizar
    // la petición hacia la API.
    this.http


      // Realiza una petición POST.
      .post<any>(


        // Envía la petición al endpoint de Login.
        this.urlLogin,


        // Envía los datos del usuario.
        datosLogin
      )


      // Se suscribe para recibir
      // la respuesta de la API.
      .subscribe({


        // Se ejecuta cuando
        // la API responde correctamente.
        next: (respuesta) => {


          // Finaliza el estado de carga.
          this.cargando = false;


          // Comprueba que la respuesta
          // realmente contenga un token.
          if (!respuesta?.token) {


            // Muestra un error
            // si el token no fue recibido.
            this.mensajeError =
              'No se pudo iniciar sesión.';


            // Actualiza inmediatamente el HTML.
            this.detectorCambios.detectChanges();


            // Detiene el proceso.
            return;
          }


          // Guarda el token JWT
          // dentro del navegador.
          localStorage.setItem(
            'token',
            respuesta.token
          );


          // Guarda el rol del usuario.
          localStorage.setItem(
            'rol',
            respuesta.rol
          );


          // Comprueba que exista
          // un identificador de usuario.
          if (respuesta.idUsuario) {


            // Guarda el identificador
            // convertido a texto.
            localStorage.setItem(
              'idUsuario',
              respuesta.idUsuario.toString()
            );
          }


          // Guarda el nombre del usuario.
          localStorage.setItem(
            'nombreUsuario',


            // Utiliza nombreCompleto si existe.
            respuesta.nombreCompleto ||


            // Si no existe, utiliza nombre.
            respuesta.nombre ||


            // Si tampoco existe,
            // utiliza un valor predeterminado.
            'Usuario'
          );

          // Guarda el correo utilizado para iniciar sesión.
          localStorage.setItem(
            'correoUsuario',
            this.correo.trim()
          );


          // Guarda temporalmente
          // el rol recibido de la API.
          const rol = respuesta.rol;


          // Comprueba si el usuario
          // tiene rol Administrador.
          if (rol === 'Administrador') {


            // Navega al Dashboard del Administrador.
            this.router.navigate([
              '/admin-dashboard'
            ]);


            // Detiene el método.
            return;
          }


          // Comprueba si el usuario
          // tiene rol Empleado.
          if (rol === 'Empleado') {


            // Navega al Dashboard del Empleado.
            this.router.navigate([
              '/empleado-dashboard'
            ]);


            // Detiene el método.
            return;
          }


          // Comprueba si el usuario
          // tiene rol Cliente.
          if (rol === 'Cliente') {


            // Navega al Dashboard del Cliente.
            this.router.navigate([
              '/dashboard'
            ]);


            // Detiene el método.
            return;
          }


          // Si el rol recibido no corresponde
          // a ninguno de los permitidos,
          // elimina los datos de la sesión.
          this.limpiarSesion();


          // Muestra un mensaje de error.
          this.mensajeError =
            'El rol de esta cuenta no es válido.';


          // Actualiza el HTML.
          this.detectorCambios.detectChanges();
        },


        // Se ejecuta cuando
        // ocurre un error en la petición.
        error: (error) => {


          // Muestra el error completo
          // en la consola para facilitar pruebas.
          console.error(
            'Error de login:',
            error
          );


          // Finaliza el estado de carga.
          this.cargando = false;


          // Comprueba si el servidor
          // respondió con error 401.
          if (error.status === 401) {


            // Indica que las credenciales son incorrectas.
            this.mensajeError =
              'El correo o la contraseña son incorrectos.';


            // Comprueba si el servidor
            // respondió con error 400.
          } else if (error.status === 400) {


            // Utiliza el mensaje enviado por la API
            // si está disponible.
            this.mensajeError =
              error.error?.mensaje ||


              // Si no existe un mensaje,
              // utiliza este texto.
              'Revise el correo y la contraseña.';


            // El código 0 generalmente indica
            // que no fue posible conectarse con la API.
          } else if (error.status === 0) {


            // Muestra un mensaje indicando
            // que debe verificarse la API.
            this.mensajeError =
              'No se pudo conectar con la API. Verifique que esté ejecutándose.';


            // Se ejecuta para cualquier
            // otro error recibido.
          } else {


            // Utiliza el mensaje de la API
            // cuando esté disponible.
            this.mensajeError =
              error.error?.mensaje ||


              // Utiliza un mensaje general
              // cuando la API no envía uno.
              'Ocurrió un error al iniciar sesión.';
          }


          // Fuerza a Angular a actualizar
          // inmediatamente el contenido del HTML.
          this.detectorCambios.detectChanges();
        }
      });
  }


  // Elimina los datos relacionados
  // con la sesión actual.
  private limpiarSesion(): void {


    // Elimina el token JWT.
    localStorage.removeItem('token');


    // Elimina el rol guardado.
    localStorage.removeItem('rol');


    // Elimina el identificador del usuario.
    localStorage.removeItem('idUsuario');


    // Elimina el nombre del usuario.
    localStorage.removeItem('nombreUsuario');
  }
}
