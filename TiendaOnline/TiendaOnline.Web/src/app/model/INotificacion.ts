// Representa una notificación del usuario.
export interface INotificacion {

  // Identificador de la notificación.
  idNotificacion: number;

  // Usuario que recibe la notificación.
  idUsuario: number;

  // Título mostrado al usuario.
  titulo: string;

  // Contenido de la notificación.
  mensaje: string;

  // Tipo de notificación.
  tipo: string | null;

  // Fecha de creación.
  fechaCreacion: string;

  // Indica si fue leída.
  leida: boolean;

  // Indica si está activa.
  estado: boolean;
}
