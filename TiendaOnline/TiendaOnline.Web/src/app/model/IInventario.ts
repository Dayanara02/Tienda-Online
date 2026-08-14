// Representa el inventario de un producto.
export interface IInventario {

  // Identificador del inventario.
  idInventario: number;

  // Producto relacionado.
  idProducto: number;

  // Cantidad disponible.
  cantidadDisponible: number;

  // Fecha de actualización.
  fechaActualizacion: string;
}
