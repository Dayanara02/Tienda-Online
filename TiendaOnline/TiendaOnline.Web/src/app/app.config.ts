// Importa la configuración principal de Angular.
// Aquí se registran los servicios globales de la aplicación.
import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners
} from '@angular/core';


// Permite utilizar las rutas definidas
// dentro del archivo app.routes.ts.
import { provideRouter } from '@angular/router';


// Permite realizar peticiones HTTP
// hacia la API de TiendaOnline.
import { provideHttpClient } from '@angular/common/http';


// Permite utilizar animaciones necesarias
// para varios componentes de PrimeNG.
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';


// Importa la configuración global de PrimeNG.
import { providePrimeNG } from 'primeng/config';


// Importa uno de los temas oficiales de PrimeNG.
//
// Aura será la base visual.
// Después podemos personalizarlo con los colores de Esencia.
import Aura from '@primeuix/themes/aura';


// Importa las rutas actuales de la aplicación.
import { routes } from './app.routes';


// Configuración general de la aplicación Angular.
export const appConfig: ApplicationConfig = {
  providers: [

    // Maneja errores globales del navegador.
    provideBrowserGlobalErrorListeners(),


    // Mantiene funcionando todas las rutas
    // que ya tenemos creadas.
    provideRouter(routes),


    // Mantiene funcionando las peticiones
    // hacia nuestra API.
    provideHttpClient(),


    // Activa las animaciones necesarias
    // para componentes como diálogos,
    // menús y otros controles de PrimeNG.
    provideAnimationsAsync(),


    // Configuración global de PrimeNG.
    providePrimeNG({

      // Activa el efecto visual de ondas
      // al presionar ciertos componentes.
      ripple: true,

      // Configura el tema visual.
      theme: {

        // Utilizamos Aura como base.
        preset: Aura,

        options: {

          // Evita que PrimeNG cambie automáticamente
          // a modo oscuro dependiendo del sistema.
          //
          // Así mantenemos el diseño claro de Esencia.
          darkModeSelector: false
        }
      }
    })
  ]
};
