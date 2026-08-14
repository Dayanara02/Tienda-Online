// Representa un proveedor del sistema.
export interface IProveedor {

  // Identificador del proveedor.
  idProveedor: number;

  // Nombre del proveedor.
  nombre: string;

  // Identificación del proveedor.
  identificacion: string;

  // Correo electrónico.
  correo: string | null;

  // Número de teléfono.
  telefono: string | null;

  // Dirección del proveedor.
  direccion: string | null;

  // Indica si está activo.
  estado: boolean;
}
