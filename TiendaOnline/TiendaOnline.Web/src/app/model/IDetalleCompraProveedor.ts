// Representa un producto de una compra a proveedor.
export interface IDetalleCompraProveedor {

  // Identificador del detalle.
  idDetalleCompra: number;

  // Compra relacionada.
  idCompraProveedor: number;

  // Producto comprado.
  idProducto: number;

  // Cantidad comprada.
  cantidad: number;

  // Precio por unidad.
  precioUnitario: number;

  // Subtotal del detalle.
  subtotal: number;
}
