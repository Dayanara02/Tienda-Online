// Representa un método de pago.
export interface IMetodoPago {

  // Identificador del método.
  idMetodoPago: number;

  // Nombre del método.
  nombre: string;

  // Descripción opcional.
  descripcion: string | null;

  // Indica si está activo.
  estado?: boolean;
}
