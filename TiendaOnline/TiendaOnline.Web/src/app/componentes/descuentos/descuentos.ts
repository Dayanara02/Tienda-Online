// Importa CommonModule para usar directivas como *ngIf y *ngFor.
import { CommonModule } from '@angular/common';

// Importa Component para crear la pantalla de Descuentos.
import { Component } from '@angular/core';

// Importa Router para navegar y RouterLink para usar enlaces en el HTML.
import {
  Router,
  RouterLink
} from '@angular/router';

// Importa botones de PrimeNG.
import { ButtonModule } from 'primeng/button';

// Importa iconos de Angular Material.
import { MatIconModule } from '@angular/material/icon';

// Define la estructura de cada promoción.
interface Promocion {
  // Identificador único de la promoción.
  id: number;

  // Nombre que se muestra al cliente.
  nombre: string;

  // Explicación corta del beneficio.
  descripcion: string;

  // Cantidad mínima necesaria para activar el descuento.
  cantidadMinima: number;

  // Porcentaje que se aplica al subtotal.
  porcentaje: number;

  // Nombre del icono de Angular Material.
  icono: string;
}

// Configura el componente de Descuentos.
@Component({
  selector: 'app-descuentos',

  // Registra los módulos utilizados en esta pantalla.
  imports: [
    CommonModule,
    RouterLink,
    ButtonModule,
    MatIconModule
  ],

  // Define los archivos visuales del componente.
  templateUrl: './descuentos.html',
  styleUrl: './descuentos.css'
})
export class Descuentos {

  // Guarda el identificador de la promoción activa.
  promocionActivaId = 0;

  // Guarda un mensaje temporal para informar al cliente.
  mensaje = '';

  // Contiene las promociones disponibles para el cliente.
  promociones: Promocion[] = [
    {
      id: 1,
      nombre: 'Compra Esencial',
      descripcion:
        'Una opción sencilla para obtener un beneficio desde una compra pequeña.',
      cantidadMinima: 2,
      porcentaje: 5,
      icono: 'shopping_bag'
    },
    {
      id: 2,
      nombre: 'Rutina de Cuidado',
      descripcion:
        'Pensada para clientes que desean combinar varios productos de cuidado personal.',
      cantidadMinima: 5,
      porcentaje: 10,
      icono: 'spa'
    },
    {
      id: 3,
      nombre: 'Cliente Frecuente',
      descripcion:
        'Beneficio especial para compras más completas dentro de la tienda.',
      cantidadMinima: 10,
      porcentaje: 15,
      icono: 'loyalty'
    },
    {
      id: 4,
      nombre: 'Compra Especial',
      descripcion:
        'Un descuento mayor para pedidos con una cantidad considerable de productos.',
      cantidadMinima: 20,
      porcentaje: 20,
      icono: 'redeem'
    },
    {
      id: 5,
      nombre: 'Compra Mayorista',
      descripcion:
        'Nuestro beneficio más alto para pedidos grandes realizados en una sola compra.',
      cantidadMinima: 50,
      porcentaje: 30,
      icono: 'inventory_2'
    }
  ];

  // Inyecta Router para navegar desde este componente.
  constructor(
    private router: Router
  ) {
    // Recupera la promoción seleccionada anteriormente.
    this.cargarPromocionActiva();
  }

  // Carga la promoción guardada en localStorage.
  cargarPromocionActiva(): void {
    // Busca la promoción guardada en el navegador.
    const promocionGuardada =
      localStorage.getItem('promocionActiva');

    // Termina si no existe una promoción guardada.
    if (!promocionGuardada) {
      return;
    }

    try {
      // Convierte el texto guardado nuevamente en un objeto.
      const promocion: Promocion =
        JSON.parse(promocionGuardada);

      // Guarda el identificador para marcarla como activa.
      this.promocionActivaId =
        promocion.id;
    } catch {
      // Elimina el dato si su contenido no es válido.
      localStorage.removeItem(
        'promocionActiva'
      );

      // Reinicia la promoción seleccionada.
      this.promocionActivaId = 0;
    }
  }

  // Activa la promoción seleccionada por el cliente.
  activarPromocion(
    promocion: Promocion
  ): void {
    // Guarda la promoción completa en localStorage.
    localStorage.setItem(
      'promocionActiva',
      JSON.stringify(promocion)
    );

    // Marca visualmente la promoción seleccionada.
    this.promocionActivaId =
      promocion.id;

    // Informa cuál promoción fue seleccionada.
    this.mostrarMensaje(
      `${promocion.nombre} fue seleccionada correctamente.`
    );
  }

  // Desactiva la promoción actualmente seleccionada.
  desactivarPromocion(): void {
    // Elimina la promoción del almacenamiento.
    localStorage.removeItem(
      'promocionActiva'
    );

    // Indica que ya no existe una promoción activa.
    this.promocionActivaId = 0;

    // Informa al cliente sobre el cambio.
    this.mostrarMensaje(
      'La promoción fue desactivada.'
    );
  }

  // Indica si una promoción es la seleccionada actualmente.
  estaActiva(
    promocion: Promocion
  ): boolean {
    // Compara ambos identificadores.
    return (
      this.promocionActivaId ===
      promocion.id
    );
  }

  // Muestra un mensaje durante unos segundos.
  mostrarMensaje(
    texto: string
  ): void {
    // Guarda el mensaje recibido.
    this.mensaje = texto;

    // Limpia el mensaje después de tres segundos.
    setTimeout(
      () => {
        this.mensaje = '';
      },
      3000
    );
  }

  // Navega directamente al Carrito.
  irAlCarrito(): void {
    this.router.navigate([
      '/carrito'
    ]);
  }

  // Navega de regreso al Dashboard.
  volverDashboard(): void {
    this.router.navigate([
      '/dashboard'
    ]);
  }

  // Cierra la sesión actual del usuario.
  cerrarSesion(): void {
    // Elimina los datos principales de autenticación.
    localStorage.removeItem('token');
    localStorage.removeItem('rol');
    localStorage.removeItem('idUsuario');
    localStorage.removeItem('nombreUsuario');
    localStorage.removeItem('correoUsuario');

    // Envía nuevamente al Login.
    this.router.navigate([
      '/login'
    ]);
  }
}
