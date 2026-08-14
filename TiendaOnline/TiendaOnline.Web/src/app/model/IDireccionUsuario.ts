// Representa una dirección del usuario.
export interface IDireccionUsuario {

  // Identificador de la dirección.
  idDireccion: number;

  // Usuario propietario.
  idUsuario: number;

  // Provincia.
  provincia: string;

  // Cantón.
  canton: string;

  // Distrito.
  distrito: string;

  // Dirección exacta.
  direccionExacta: string;

  // Código postal opcional.
  codigoPostal: string | null;

  // Indica si es la dirección principal.
  principal: boolean;

  // Indica si está activa.
  estado: boolean;
}
