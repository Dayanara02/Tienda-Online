// Importa la configuración principal de Angular.
// Aquí se registran los servicios globales de la aplicación.
import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners
} from '@angular/core';


// Permite utilizar las rutas definidas
// dentro del archivo app.routes.ts.
import {
  provideRouter
} from '@angular/router';


// Permite realizar peticiones HTTP
// hacia la API de TiendaOnline.
import {
  provideHttpClient
} from '@angular/common/http';


// Permite utilizar animaciones necesarias
// para varios componentes de PrimeNG.
import {
  provideAnimationsAsync
} from '@angular/platform-browser/animations/async';


// Importa la configuración global de PrimeNG.
import {
  providePrimeNG
} from 'primeng/config';


// Importa el tema Aura de PrimeNG.
import Aura from '@primeuix/themes/aura';


// Importa las rutas actuales de la aplicación.
import {
  routes
} from './app.routes';


// Configuración general de la aplicación Angular.
export const appConfig: ApplicationConfig = {

  providers: [

    // Maneja errores globales del navegador.
    provideBrowserGlobalErrorListeners(),


    // Mantiene funcionando todas las rutas.
    provideRouter(
      routes
    ),


    // Permite realizar peticiones
    // hacia nuestra API.
    provideHttpClient(),


    // Activa las animaciones
    // utilizadas por PrimeNG.
    provideAnimationsAsync(),


    // Configuración global de PrimeNG.
    providePrimeNG({

      // =================================================
      // LICENCIA PRIMEUI
      // =================================================

      // Aquí se coloca la licencia
      // Community obtenida en PrimeStore.
      license:
        'eyJpZCI6IjMyZTE0NTQ5LWFiYTQtNDcxYi04NjgxLWI1YjBkMDc3OWRmMCIsInByb2R1Y3QiOiJwcmltZXVpIiwidGllciI6ImNvbW11bml0eSIsInR5cGUiOiJkZXYiLCJpYXQiOjE3ODY3Mzk5MjgsImV4cCI6MTgxODI3NTkyOH0.CNNwo3oV5Pd_48TuVNNqKSxsxkyQWcJbE8Qi832uqqsSQL5LoS_Y8WJRED9ApykcpnugXkvKslMNKz3aBLCoAA',


      // Activa el efecto visual
      // al presionar algunos componentes.
      ripple:
        true,


      // Configura el tema visual.
      theme: {

        // Utiliza Aura como tema base.
        preset:
          Aura,


        // Opciones del tema.
        options: {

          // Mantiene el diseño claro
          // sin cambiar automáticamente
          // según el sistema operativo.
          darkModeSelector:
            false
        }
      }
    })
  ]
};
