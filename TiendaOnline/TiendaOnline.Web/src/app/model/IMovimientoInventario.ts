// Representa un movimiento de inventario.
export interface IMovimientoInventario {

  // Identificador del movimiento.
  idMovimiento: number;

  // Inventario relacionado.
  idInventario: number;

  // Usuario que realizó el movimiento.
  idUsuario: number;

  // Tipo de movimiento.
  tipoMovimiento: string;

  // Cantidad modificada.
  cantidad: number;

  // Motivo del movimiento.
  motivo: string | null;

  // Fecha del movimiento.
  fechaMovimiento: string;
}
