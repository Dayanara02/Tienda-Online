// Representa un usuario del sistema.
export interface IUsuario {

  // Identificador del usuario.
  idUsuario: number;

  // Nombre del usuario.
  nombre: string;

  // Apellido del usuario.
  apellido: string;

  // Correo electrónico.
  correo: string;

  // Nombre del rol.
  rol?: string;

  // Indica si está activo.
  estado?: boolean;
}
