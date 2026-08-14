// Representa un descuento.
export interface IDescuento {

  // Identificador del descuento.
  idDescuento: number;

  // Nombre del descuento.
  nombre: string;

  // Descripción opcional.
  descripcion: string | null;

  // Porcentaje aplicado.
  porcentaje: number;

  // Fecha de inicio.
  fechaInicio: string;

  // Fecha de finalización.
  fechaFin: string;

  // Indica si está activo.
  estado: boolean;
}
